namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = 
        | TransactionProperty of (Transaction list -> bool)

    type Property with
       static member (|&&|) (TransactionProperty p1, TransactionProperty p2) = TransactionProperty(fun ts -> p1 ts && p2 ts)
      