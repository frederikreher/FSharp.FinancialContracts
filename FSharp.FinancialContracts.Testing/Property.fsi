namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type Filter = (Transaction -> bool)
    
    type Property = 
        | Predicate of (Environment -> Transaction list[] -> bool)
        | And of Property * Property
        | Or of Property * Property
        | Sum of float * (float -> float -> bool) * Filter
        | ForAllTimes of Property

    val evalProp : Property -> Environment -> Transaction list[] -> bool
    val testProperty : Property -> Environment -> Transaction list[] -> bool

    val (&&) : Property -> Property -> Property
    val (||) : Property -> Property -> Property
      