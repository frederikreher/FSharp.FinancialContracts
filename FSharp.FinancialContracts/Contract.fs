namespace FSharp.FinancialContracts

open System

module Contract =

    type Currency = GBP | USD | DKK

    // Defines how a contract can be constructed
    type Contract = 
      | Trade of int * Currency
      | After of DateTime * Contract
      | Until of DateTime * Contract
      | Combine of Contract * Contract

    // Evaluate contract on a specific day
    let rec eval contract (day:DateTime) = 
      [ match contract with
        | Trade(a, n) -> yield (a, n) 
        | Combine(c1, c2) -> 
            yield! eval c1 day
            yield! eval c2 day
        | After(dt, c) when day > dt -> yield! eval c day
        | Until(dt, c) when day < dt -> yield! eval c day
        | _ -> () ]

    // Functions for creating contracts
    let trade (amount, what) = Trade (amount, what)
    let after dt contract = After (dt, contract)
    let until dt contract = Until (dt, contract)
    let ($) c1 c2 = Combine(c1, c2)