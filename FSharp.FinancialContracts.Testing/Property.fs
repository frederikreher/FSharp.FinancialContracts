namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = Transaction list -> bool

    let testProperty (property:Property) ts : bool = property ts

    let (&&) p1 p2 : Property = (fun ts -> p1 ts && p2 ts)
    let (||) p1 p2 : Property = (fun ts -> p1 ts || p2 ts)
     