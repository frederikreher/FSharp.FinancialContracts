namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type TransactionProperty = (Environment -> Transaction list[] -> bool)
    type BinOp = (float -> float -> bool)
    type Filter = (Transaction -> bool)

    let sumByFilter filter = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    let s = op_Equality
    type Property = 
        | Predicate of (Environment -> Transaction list[] -> bool)
        | And of Property * Property
        | Or of Property * Property
        | SumOf of Filter * BinOp * float 
        | ForAllTimes of Property

    let rec evalProp property env (transactionsByDay as t) : bool = 
        match property with
            | Predicate p -> p env t
            | And(p1,p2) -> evalProp p1 env t && evalProp p2 env t
            | Or(p1,p2) -> evalProp p1 env t || evalProp p2 env t
            | SumOf(filter,op,f) -> op f (Array.sumBy (fun ts -> List.sumBy (sumByFilter filter) ts) t)
    
    let (&&) p1 p2 : Property = And(p1,p2)
    let (||) p1 p2 : Property = Or(p1,p2)

    let testProperty prop env ts = evalProp prop env ts
