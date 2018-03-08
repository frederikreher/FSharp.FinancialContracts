namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =

    type Property = Transaction list -> bool

    val testProperty : Property -> Transaction list -> bool

    val (&&) : Property -> Property -> Property
      