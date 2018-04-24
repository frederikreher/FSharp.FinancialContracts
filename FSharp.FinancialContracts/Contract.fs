namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Environment

module Contract =

    // Currency used in an transaction.
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    // Transaction represents a certain amount of a currency.
    type Transaction = Transaction of float * Currency
    
    // TransactionResults is the current time and an array of lists of transactions
    // with each list representing a day. 
    type TransactionResults = Time * Transaction list []
    
    // Returns the transactions of a TransactionResults element.
    let getTransactions (t,ts) : Transaction list [] = 
        let length = Array.length ts
        Array.sub ts t (length-t)
    
    // Increase the time of a TransactionResults element.
    let increaseTime ((currentTime,ts):TransactionResults) t : TransactionResults = 
        let newTime = t + currentTime
        if newTime >= ts.Length then failwith "The current time of the transactions cannot exceed the horizon of the contract"
        else (newTime,ts)
        
    // Defines how a contract can be constructed.
    type Contract = 
        | Zero                                             // Contract that has no rights or obligations.
        | One      of Currency                             // Contract paying one unit of the provided currency.
        | Delay    of TimeObs * Contract                      // Acquire the contract at the provided time.
        | Scale    of NumberObs * Contract                 // Acquire the provided contract, but all rights and obligations 
                                                           // are scaled by the provided observable according to its value 
                                                           // at the day of each transaction.
        | ScaleNow of NumberObs * Contract                 // Acquire the provided contract, but all rights and obligations 
                                                           // are scaled by the provided observable evualated when the 
                                                           // contract is aqcuired.
        | And      of Contract * Contract                  // Immediately acquire both contracts.
        | If       of BoolObs * TimeObs * Contract * Contract // Acquire the first contract at the point where the observable 
                                                           // becomes true within the given time else acquire the second 
                                                           // contract at the provided time.
        | Give     of Contract                             // Contract giving away the provided contract. 
                                                           // E.g. X acquiring c from Y, implies that Y 'give c'.

    // Inline version of And.
    let (&-&) c1 c2 = And(c1,c2)

    // Return the horizon at which all elements of a contract can be evaluated.
    let rec horizon c (t:Time) : Time =
        match c with
        | Zero | One(_)                          -> max t 0
        | Delay(t1, c)                           -> let t1' = timeHorizon t1
                                                    if t1' >= 0 then t1' + (horizon c t)
                                                    else failwith "Can only delay with non-negative integers"
        | And(c1, c2)                            -> max (horizon c1 t) (horizon c2 t)
        | If(_, t1, c1, c2)                      -> let t1' = timeHorizon t1
                                                    if t1' >= 0 then t1' + (max (horizon c1 t) (horizon c2 t))
                                                    else failwith "The 'within' argument has to be a non-negative integer"
        | Scale(_, c) | ScaleNow(_, c) | Give(c) -> horizon c t
    and timeHorizon timeObs = 
        match timeObs with
        | TimeObs.Const t     -> t
        | TimeObs.If(_,t1,t2) -> max (timeHorizon t1) (timeHorizon t2)
        | TimeObs.Add(t1,t2)  -> (timeHorizon t1) + (timeHorizon t2)
        | TimeObs.Mult(t1,t2) -> (timeHorizon t1) * (timeHorizon t2)

    let getHorizon c : Time = (horizon c 0) + 1

    // Return a tuple of BoolObs list and NumberObs list, 
    // containing the observables needed to evaluate all elements of a contract.
    let rec observables c boolAcc numAcc : BoolObs list * NumberObs list =
        match c with
        | And(c1, c2)                        -> let (boolAcc1, numAcc1) = observables c2 boolAcc numAcc
                                                observables c1 boolAcc1 numAcc1
        | If(obs, tObs, c1, c2)              -> let (boolAcc1,numAcc1) = boolObs obs boolAcc numAcc
                                                let (boolAcc2,numAcc2) = timeObs tObs boolAcc numAcc
                                                let (boolAcc3,numAcc3) = observables c1 boolAcc1 numAcc1
                                                observables c2 boolAcc3 numAcc3
        | Zero | One(_)                      -> (boolAcc, numAcc)
        | Delay(obs, c)                      -> let (boolAcc1,numAcc1) = timeObs obs boolAcc numAcc
                                                observables c boolAcc1 numAcc1
        | Give(c)                            -> observables c boolAcc numAcc
        | Scale(obs, c1) | ScaleNow(obs, c1) -> let (boolAcc1,numAcc1) = numberObs obs boolAcc numAcc
                                                observables c1 boolAcc1 numAcc1
    
    let getObservables c : BoolObs list * NumberObs list = observables c [] []
    
    // Evaluates a contract and returns an array of list of Transactions.   
    let evaluateContract environment contract : TransactionResults =       
        let transactions = Array.create (getTime environment + (getHorizon contract)) List.empty
        let (ct,observables) = environment
        
        let rec evalContract now factor c : unit = 
            match c with
            | Zero               -> ()
            | One(currency)      -> let transaction = Transaction((evalNumberObs factor (now,observables)) * 1.0,currency)
                                    transactions.[now] <- transaction::(transactions.[now])
            | Delay(t, c)        -> let t' = evalTimeObs t environment
                                    if t' >= 0 then evalContract (now+t') factor c
                                    else failwith "Can only delay with non-negative integers"
            | Scale(obs, c)      -> evalContract now (Mult(obs,factor)) c
            | ScaleNow(obs, c)   -> let currentFactor = (evalNumberObs obs (now,observables))
                                    evalContract now (Mult(Const currentFactor,factor)) c
            | And(c1, c2)        -> evalContract now factor c1
                                    evalContract now factor c2
            | If(obs, t, c1, c2) -> let t' = evalTimeObs t environment
                                    if t' >= 0 then
                                        let boolValue = evalBoolObs obs (now,observables)
                                        if t' = 0 && not boolValue then evalContract now factor c2   //Time has gone and boolvalue is false return c2
                                        else if boolValue then evalContract now factor c1 //If bool value is true and time hasn't gone return 2
                                        else evalContract (now+1) factor (If(obs, TimeObs.Const(t'-1), c1, c2))    //Else IncreaseEnvironment time and decrease t
                                    else failwith "The 'within' argument has to be a non-negative integer"
            | Give(c)            -> evalContract now (Mult(Const -1.0,factor)) c
    
        evalContract ct (Const 1.0) contract
        (ct,transactions)
        