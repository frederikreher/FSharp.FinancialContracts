namespace FSharp.FinancialContracts

open System
open System.Reactive.Linq

module Contract =

    // Currency used in contracts
    type Currency = GBP | USD | DKK | None
    type Transaction = Transaction of float * Currency
    type Time = int
    type Observable = 
        | Get of Currency * Currency
        | Bool of Boolean
        
    type Environment(time:int) = 
        let mutable currentTime = time
        member this.CurrentTime() = currentTime
        member this.IncreaseTime() = currentTime <- currentTime + 1
        new() = Environment(0)

    // Defines how a contract can be constructed
    type Contract = 
        | Zero of float * Currency                     // Contract that has no rights or obligations
        | One of Currency                               // Contract paying one unit of the provided currency. Has an infinite horizon.
        | Delay of Time * Contract
        | Scale of Observable * Contract                // Acquire the provided contract, but all rights and obligations 
                                                        // is scaled by the provided value.
        | And of Contract * Contract                    // Immediately acquire both contracts.
        | Or of Contract * Contract                     // Immediately acquire either of the contracts.
        | If of Observable * Time * Contract * Contract
        | Give of Contract                              // Contract giving away the provided contract. 
                                                        // E.g. X acquiring c from Y, implies that Y 'give c'.

    let zero = Zero(0.0, None)
    let one currency = One(currency)
    let delay t c = Delay(t, c)
    let scale d c1 = Scale(d, c1)
    let _and_ c1 c2 = And(c1, c2)
    let _or_ c1 c2 = Or(c1, c2)
    let _if_ obs t c1 c2 = If(obs, t, c1, c2)
    let give c = Give(c)

    // Evaluate observable
    let rec evalObs (env:Environment) obs = 
        match obs with
        | Get(cur1, cur2) -> 1.0
        | Bool(b) -> b
        | _ -> failwith "Unknown Observable type"

    // Evaluate contract
    let rec evalC (env:Environment) contract = 
        [ match contract with
          | Zero(a, n) -> yield Transaction(a, n)
          | One(currency) -> yield Transaction(1.0, currency)
          | Delay(t, c) when (env.CurrentTime()) > t -> yield! evalC env c
          | Scale(obs, c1) ->
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction((evalObs env obs) * a, n)::acc
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
              if (evalObs env obs) then
                  if (env.CurrentTime()) < t then
                      yield! evalC env c1
              else
                  if (env.CurrentTime()) < t then
                      yield! evalC env c2
          | Give(c) -> 
              yield! List.fold (fun acc trans -> 
                                match trans with
                                | Transaction(a, n) -> Transaction(-a, n)::acc
                                | _ -> failwith "'Give' contract could not be evaluated"
                               ) [] (evalC env c)
          | _ -> () ]
