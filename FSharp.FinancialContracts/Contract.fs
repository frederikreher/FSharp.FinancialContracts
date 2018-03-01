namespace FSharp.FinancialContracts

open System
open FSharp.Data
open Environment

module Contract =

    // Currency used in contracts
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR
    type Transaction = Transaction of float * Currency

    // Defines how a contract can be constructed
    type Contract = 
        | Zero of float * Currency                      // Contract that has no rights or obligations
        | One of Currency                               // Contract paying one unit of the provided currency.
        | Delay of Time * Contract                      // 
        | Scale of NumberObs * Contract                // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | Or of Contract * Contract                     // Immediately acquire either of the contracts.
        | If of BoolObs * Time * Contract * Contract // 
        | Give of Contract                              // Contract giving away the provided contract. 
                                                        // E.g. X acquiring c from Y, implies that Y 'give c'.

    let getExchangeRate (cur1:Currency, cur2:Currency) = 
        if string(cur1) = string(cur2) then
            1.0
        else
            let url = "https://api.fixer.io/latest?base=" + string(cur1) + "&symbols=" + string(cur2)
            let query = Http.RequestString( url )
            let res = query.[(query.LastIndexOf(":") + 1)..(query.Length - 3)]
            float(res)

    let rec horizon c (t:Time) =
        match c with
        | Zero(a, n) -> max t 0
        | One(currency) -> max t 0 
        | Delay(t1, c) -> max t1 (horizon c t)
        | Scale(obs, c1) -> horizon c1 t
        | And(c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | Or(c1, c2) -> max (horizon c1 t) (horizon c2 t)
        | If(obs, t1, c1, c2) -> max t1 (max (horizon c1 t) (horizon c2 t))
        | Give(c) -> horizon c t

    let getHorizon c = horizon c 0

    // Evaluate contract
    let rec evalC (env:Environment) contract : Transaction list = 
        [ match contract with
          | Zero(a, n) -> ()
          | One(currency) -> yield Transaction(1.0, currency)
          | Delay(t, c) when (env |> getTime) >= t -> yield! evalC env c
          | Scale(obs, c1) ->
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction((evalNumberObs obs (getNumEnv env) (getBoolEnv env)) * a, n)::acc
                                | _ -> failwith "'Scale' contract could not be evaluated"
                               ) [] (evalC env c1)
          | And(c1, c2) -> 
              yield! evalC env c1
              yield! evalC env c2
          | Or(c1, c2) ->
              let rand = new Random()
              if (rand.Next(0,2) = 0) then 
                  yield! evalC env c1
              else
                  yield! evalC env c2
          | If(obs, t, c1, c2) -> 
              if (evalBoolObs obs (getBoolEnv env) (getNumEnv env)) then
                  if (env |> getTime) <= t then
                      yield! evalC env c1
              else
                  if (env |> getTime) <= t then
                      yield! evalC env c2
          | Give(c) -> 
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction(-a, n)::acc
                                | _ -> failwith "'Give' contract could not be evaluated"
                               ) [] (evalC env c)
          | _ -> () ]
