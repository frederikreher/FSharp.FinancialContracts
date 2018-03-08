namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = 
        | TransactionProperty of (Transaction list -> bool)

    type Property with
       static member (|&&|) : Property * Property -> Property
      