namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Environment

module Contract =

    // Currency used in an asset.
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    // Evaluation of a contract result in a Transaction.
    type Transaction = Transaction of float * Currency
    
    //Final result of the evaluation of a contract. 
    type TransactionResults = Time * Transaction list []
    
    let getTransactions (t,ts) : Transaction list [] = 
        let length = Array.length ts
        Array.sub ts t (length-t)
    
    let increaseTime (t,ts) td : TransactionResults = (t+td,ts)
        
    // Defines how a contract can be constructed.
    type Contract = 
        | Zero                                          // Contract that has no rights or obligations.
        | One of Currency                               // Contract paying one unit of the provided currency.
        | Delay of Time * Contract                      // Acquire the contract at the provided time or later.
        | Scale of NumberObs * Contract                 // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | ScaleNow of NumberObs * Contract              // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided observable evualated when contract is aqcuired
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | If of BoolObs * Time * Contract * Contract    // Acquire the first contract if the observable is true 
                                                        // else acquire the second contract. Either contract 
                                                        // is acquired at the provided time or later.
        | Give of Contract                              // Contract giving away the provided contract. 
                                                        // E.g. X acquiring c from Y, implies that Y 'give c'.

    let (&-&) c1 c2 = And(c1,c2)

    // Return the horizon at which all elements of a contract can be evaluated.
    let rec horizon c (t:Time) : Time =
        match c with
        | Zero -> max t 0
        | One(currency) -> max t 0 
        | Delay(t1, c) -> t1 + (horizon c t)
        | Scale(obs, c1) -> horizon c1 t
        | ScaleNow(obs, c1) -> horizon c1 t
        | And(c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | If(obs, t1, c1, c2) -> t1 + (max (horizon c1 t) (horizon c2 t))
        | Give(c) -> horizon c t

    let getHorizon c : Time = (horizon c 0)+1

    // Return a tuple of BoolObs list and NumberObs list, 
    // containing the observables needed to evaluate all elements of a contract.
    let rec observables c boolAcc numAcc : BoolObs list * NumberObs list =
        match c with
        | Zero -> (boolAcc, numAcc)
        | One(_) -> (boolAcc, numAcc)
        | Delay(_, c) -> observables c boolAcc numAcc
        | Scale(obs, c1) -> 
            let (boolAcc1,numAcc1) = (numberObs obs boolAcc numAcc)
            observables c1 boolAcc1 numAcc1
        | ScaleNow(obs, c1) -> 
                    let (boolAcc1,numAcc1) = (numberObs obs boolAcc numAcc)
                    observables c1 boolAcc1 numAcc1
        | And(c1, c2) -> 
            let (boolAcc1, numAcc1) = (observables c2 boolAcc numAcc)
            observables c1 boolAcc1 numAcc1
        | If(obs, _, c1, c2) -> 
            let (boolAcc1,numAcc1) = (boolObs obs boolAcc numAcc)
            let (boolAcc2,numAcc2) = observables c1 boolAcc1 numAcc1
            observables c2 boolAcc2 numAcc2
        | Give(c) -> observables c boolAcc numAcc
    
    let getObservables c : BoolObs list * NumberObs list = observables c [] []
    
    // Evaluates a contract and returns an array of list of Transactions.   
    let evaluateContract environment contract : TransactionResults =       
        let transactions = Array.create (getTime environment + (getHorizon contract)) List.empty
        let (ct,observables) = environment
        
        let rec evalContract now factor c : unit = 
            match c with
            | Zero -> ()
            | One(currency) -> 
                let transaction = Transaction((evalNumberObs factor (now,observables)) * 1.0,currency)
                transactions.[now] <- transaction::(transactions.[now])
            | Delay(t, c) -> evalContract (now+t) factor c
            | Scale(obs, c) -> evalContract now (Mult(obs,factor)) c
            | ScaleNow(obs, c) ->
                        let currentFactor = (evalNumberObs obs (now,observables))
                        evalContract now (Mult(Const currentFactor,factor)) c
            | And(c1, c2) -> 
                evalContract now factor c1
                evalContract now factor c2
            | If(obs, t, c1, c2) -> 
                let boolValue = evalBoolObs obs (now,observables)
                if t = 0 && not boolValue then evalContract now factor c2   //Time has gone and boolvalue is false return c2
                else if t >= 0 && boolValue then evalContract now factor c1 //If bool value is true and time hasn't gone return 2
                else evalContract (now+1) factor (If(obs, t-1, c1, c2))    //Else IncreaseEnvironment time and decrease t
            | Give(c) -> evalContract now (Mult(Const -1.0,factor)) c
    
        evalContract ct (Const 1.0) contract
        (ct,transactions)
        