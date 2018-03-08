namespace FSharp.FinancialContracts

open System
open FSharp.Data
open Environment

module Contract =

    // Currency used in contracts.
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    // Evaluation of a contract result in a Transaction.
    type Transaction = Transaction of float * Currency

    // Defines how a contract can be constructed
    type Contract = 
        | Zero                                          // Contract that has no rights or obligations.
        | One of Currency                               // Contract paying one unit of the provided currency.
        | Delay of Time * Contract                      // Acquire the contract at the provided time or later.
        | Scale of NumberObs * Contract                 // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | Or of BoolObs * Contract * Contract           // Immediately acquire either of the contracts.
        | If of BoolObs * Contract * Contract           // Acquire the first contract if the observable is true 
                                                        // else acquire the second contract. Either contract 
                                                        // is acquired at the provided time or later.
        | Give of Contract                              // Contract giving away the provided contract. 
                                                        // E.g. X acquiring c from Y, implies that Y 'give c'.

    // Provides the current exchange rate between two currencies.
    let getExchangeRate (cur1:Currency, cur2:Currency) : float = 
        if string(cur1) = string(cur2) then
            1.0
        else
            let url = "https://api.fixer.io/latest?base=" + string(cur1) + "&symbols=" + string(cur2)
            let query = Http.RequestString( url )
            let res = query.[(query.LastIndexOf(":") + 1)..(query.Length - 3)]
            float(res)

    // Return the horizon at which all elements of a contract can be evaluated.
    let rec horizon c (t:Time) : Time =
        match c with
        | Zero -> max t 0
        | One(currency) -> max t 0 
        | Delay(t1, c) -> max t1 (horizon c t)
        | Scale(obs, c1) -> horizon c1 t
        | And(c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | Or(obs, c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | If(obs, c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | Give(c) -> horizon c t

    let getHorizon c : Time = horizon c 0

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
        | Or(obs, c1, c2) -> 
            let (boolAcc1,numAcc1) = (boolObs obs boolAcc numAcc)
            let (boolAcc2,numAcc2) = observables c2 boolAcc1 numAcc1
            observables c2 boolAcc2 numAcc2
        | If(obs, c1, c2) -> 
            let (boolAcc1,numAcc1) = (boolObs obs boolAcc numAcc)
            let (boolAcc2,numAcc2) = observables c2 boolAcc1 numAcc1
            observables c2 boolAcc2 numAcc2
        | Give(c) -> observables c boolAcc numAcc
   
    let getObservables c : BoolObs list * NumberObs list = observables c [] []
    
    // Evaluates a contract and returns a list of Transactions.
    let rec evalC (env:Environment) contract : Transaction list = 
        [ match contract with
          | Zero -> ()
          | One(currency) -> yield Transaction(1.0, currency)
          | Delay(t, c)  -> yield! evalC (increaseTime t env) c
          | Scale(obs, c1) ->
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> 
                                    Transaction((evalNumberObs obs (getNumEnv (getTime env) env) (getBoolEnv (getTime env) env)) * a, n)::acc
              //                  | _ -> failwith "'Scale' contract could not be evaluated"
                               ) [] (evalC env c1)
          | And(c1, c2) -> 
              yield! evalC env c1
              yield! evalC env c2
          | Or(obs,c1, c2) ->
              let b = (evalBoolObs obs (getBoolEnv (getTime env) env) (getNumEnv (getTime env) env))
              printfn "Or bool was %A" b
              if b then 
                  match evalC env c1 with
                  [] -> yield! evalC env c2
                  | trans -> yield! trans
              else
                  match evalC env c2 with
                  [] -> yield! evalC env c1
                  | trans -> yield! trans
          | If(obs, c1, c2) -> 
              let temp = (evalBoolObs obs (getBoolEnv (getTime env) env) (getNumEnv (getTime env) env))
              printfn "%A" temp
              if temp then
                  yield! evalC env c1
              else
                  yield! evalC env c2
          | Give(c) -> 
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction(-a, n)::acc
                                //| _ -> failwith "'Give' contract could not be evaluated"
                               ) [] (evalC env c)]
