namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = Environment -> Transaction list -> bool


    let testProperty (property:Property) env ts : bool = property env ts

    let (&&) p1 p2 : Property = (fun env ts -> p1 env ts && p2 env ts)
    let (||) p1 p2 : Property = (fun env ts -> p1 env ts || p2 env ts)
     