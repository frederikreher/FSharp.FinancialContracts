namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = Environment -> Transaction list -> bool

    val testProperty : Property -> Environment -> Transaction list -> bool

    val (&&) : Property -> Property -> Property
    val (||) : Property -> Property -> Property
      