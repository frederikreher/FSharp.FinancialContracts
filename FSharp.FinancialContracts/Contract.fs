namespace FSharp.FinancialContracts

open System
open FSharp.Data
open Environment

module Contract =

    // Currency used in an asset.
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    // Commodity used in an asset.
    type Commodity =
        | Gold
        | Oil
        | Elextricity

    // Used to specify the measurement type of a commodity.
    type Measure =
        | KG
        | WATT
        | BARREL
    
    // Used in contracts to create transactions.
    type Asset =
        | Currency of Currency
        | Commodity of Commodity * Measure

    // Evaluation of a contract result in a Transaction.
    type Transaction = Transaction of float * Asset

    // Defines how a contract can be constructed.
    type Contract = 
        | Zero                                          // Contract that has no rights or obligations.
        | One of Asset                                  // Contract paying one unit of the provided currency.
        | Delay of Time * Contract                      // Acquire the contract at the provided time or later.
        | Scale of NumberObs * Contract                 // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | If of BoolObs * Time * Contract * Contract    // Acquire the first contract if the observable is true 
                                                        // else acquire the second contract. Either contract 
                                                        // is acquired at the provided time or later.
        | Give of Contract                              // Contract giving away the provided contract. 
                                                        // E.g. X acquiring c from Y, implies that Y 'give c'.



    // Return the horizon at which all elements of a contract can be evaluated.
    let rec horizon c (t:Time) : Time =
        match c with
        | Zero -> max t 0
        | One(currency) -> max t 0 
        | Delay(t1, c) -> t1 + (horizon c t)
        | Scale(obs, c1) -> horizon c1 t
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
        | And(c1, c2) -> 
            let (boolAcc1, numAcc1) = (observables c2 boolAcc numAcc)
            observables c1 boolAcc1 numAcc1
        | If(obs, _, c1, c2) -> 
            let (boolAcc1,numAcc1) = (boolObs obs boolAcc numAcc)
            let (boolAcc2,numAcc2) = observables c1 boolAcc1 numAcc1
            observables c2 boolAcc2 numAcc2
        | Give(c) -> observables c boolAcc numAcc
    let getObservables c : BoolObs list * NumberObs list = observables c [] []

    let multiplyTransactions f ts : Transaction list = List.map (fun (Transaction(v,ass)) -> Transaction(f*v,ass)) ts

    // Evaluates a contract and returns an array of list of Transactions.
    let rec evalContract (env:Environment) contract transactions : Transaction list [] = 
        let now = getTime env
        match contract with
        | Zero -> transactions
        | One(currency) -> 
            Array.set transactions now (Transaction(1.0, currency)::(transactions.[now]))
            transactions
        | Delay(t, c) -> evalContract (increaseTime t env) c transactions
        | Scale(obs, c) ->
            let newTrans = evalContract env c (Array.create (transactions.Length) List.empty)
            for i in [0..(transactions.Length-(now+1))] do
                let factor = (evalNumberObs obs (increaseTime i env))
                let updatedTransactions = multiplyTransactions factor newTrans.[now+i]
                Array.set transactions (now+i) (updatedTransactions@(transactions.[now+i]))
            transactions
        | And(c1, c2) -> 
            evalContract env c1 transactions |> ignore
            evalContract env c2 transactions |> ignore
            transactions
        | If(obs, t, c1, c2) -> 
            let boolValue = evalBoolObs obs env
            if t = 0 && not boolValue then evalContract env c2 transactions //Time has gone and boolvalue is false return c2
            else if t >= 0 && boolValue then evalContract env c1 transactions //If bool value is true and time hasn't gone return 2
            else evalContract (increaseTime 1 env) (If(obs, t-1, c1, c2)) transactions //Else IncreaseEnvironment time and decrease t
        | Give(c) -> 
            let newTrans = evalContract env c (Array.create (transactions.Length) List.empty)
            for i in [0..(transactions.Length-(now+1))] do
                let factor = -1.0
                let updatedTransactions = multiplyTransactions factor newTrans.[now+i]
                Array.set transactions (now+i) (updatedTransactions@(transactions.[now+i]))
            transactions
    
    let evalC (env:Environment) contract : Transaction list [] =
        evalContract env contract (Array.create (getTime env + (getHorizon contract)) List.empty)
