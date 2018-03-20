namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type Property = (Environment -> Transaction list -> bool)
    type BinOp<'a> = ('a -> 'a -> bool)
    type Filter = (Transaction -> bool)
    
    type TransactionProperty = 
        | And of TransactionProperty * TransactionProperty
        | Or of TransactionProperty * TransactionProperty
        | ForAllTimes of Property
        | At of Time * Property
        | Not of TransactionProperty

    val SumOf : Filter -> BinOp<float> -> float -> Property
    val CountOf : Filter -> BinOp<int> -> int -> Property

    val (&&) : TransactionProperty -> TransactionProperty -> TransactionProperty
    val (||) : TransactionProperty -> TransactionProperty -> TransactionProperty

    val evalProp : TransactionProperty -> Environment -> Transaction list[] -> bool
    val testProperty : TransactionProperty -> Environment -> Transaction list[] -> bool