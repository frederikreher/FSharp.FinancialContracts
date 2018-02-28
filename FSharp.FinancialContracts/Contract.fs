namespace FSharp.FinancialContracts

open System
open System.Reactive.Linq
open FSharp.Data

module Contract =

    // Currency used in contracts
    type Currency = GBP | USD | DKK | None
    type Transaction = Transaction of float * Currency
    type Time = int

    type Observable = 
        | Number of string
        | Bool of string

    type ObservableValue = 
        | NumberValue of float
        | BoolValue of bool    

    type Environment = Time * Map<Observable,ObservableValue>

    let getNumber k (t,m) = 
        match m |> Map.find k with
            | NumberValue f -> f
            | _ -> failwith ("Type is not a valid number type")
    
    let getBool k (t,m) = 
        match m |> Map.find k with
            | BoolValue b -> b
            | _ -> failwith ("Type is not a valid boolean type")

    let increaseTime ((t,v):Environment) = (t+1,v)
    let getTime ((t,_):Environment) = t

    // Defines how a contract can be constructed
    type Contract = 
        | Zero of float * Currency                      // Contract that has no rights or obligations
        | One of Currency                               // Contract paying one unit of the provided currency. Has an infinite horizon.
        | Delay of Time * Contract
        | Scale of Observable * Contract                // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | Or of Contract * Contract                     // Immediately acquire either of the contracts.
        | If of Observable * Time * Contract * Contract
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

    // Evaluate contract
    let rec evalC (env:Environment) contract  = 
        [ match contract with
          | Zero(a, n) -> ()
          | One(currency) -> yield Transaction(1.0, currency)
          | Delay(t, c) when (env |> getTime) >= t -> yield! evalC env c
          | Scale(obs, c1) ->
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction((env |> (getNumber obs)) * a, n)::acc
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
              if env |> (getBool obs) then
                  if (env |> getTime) < t then
                      yield! evalC env c1
              else
                  if (env |> getTime) < t then
                      yield! evalC env c2
          | Give(c) -> 
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction(-a, n)::acc
                                | _ -> failwith "'Give' contract could not be evaluated"
                               ) [] (evalC env c)
          | _ -> () ]
