namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type Filter = (Transaction -> bool)
    type BinOp = (float -> float -> bool)
    
    type Property = 
        | Predicate of (Environment -> Transaction list[] -> bool)
        | And of Property * Property
        | Or of Property * Property
        | SumOf of Filter * BinOp * float 
        | ForAllTimes of Property

    val (&&) : Property -> Property -> Property
    val (||) : Property -> Property -> Property

    val evalProp : Property -> Environment -> Transaction list[] -> bool
    val testProperty : Property -> Environment -> Transaction list[] -> bool