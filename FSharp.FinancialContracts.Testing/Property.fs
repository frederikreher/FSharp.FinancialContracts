namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type TransactionProperty = (Environment -> Transaction list[] -> bool)
    type Filter = (Transaction -> bool)

    let sumByFilter filter = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    let s = op_Equality
    type Property = 
        | Predicate of (Environment -> Transaction list[] -> bool)
        | And of Property * Property
        | Or of Property * Property
        | Sum of float * (float -> float -> bool) * Filter
        | ForAllTimes of Property

    let rec evalProp property env (transactionsByDay as t) : bool = 
        match property with
            | Predicate p -> p env t
            | And(p1,p2) -> evalProp p1 env t && evalProp p2 env t
            | Or(p1,p2) -> evalProp p1 env t || evalProp p2 env t
            | Sum(f,op,filter) -> op f (Array.sumBy (fun ts -> List.sumBy (sumByFilter filter) ts) t)
    
    let (&&) p1 p2 : Property = And(p1,p2)
    let (||) p1 p2 : Property = Or(p1,p2)
     

    let allTransactions = fun ts -> true
    let transactionsOfCurrencies curs : Filter = fun (Transaction(f,c)) -> List.contains c curs

    let sumOf filter (op:(float -> float -> bool)) f = Sum(f,op,filter)

    let b = ((sumOf allTransactions (=) 20.0) 
            && (sumOf (transactionsOfCurrencies [Currency DKK; Currency GBP ]) (<) 100.0))
            || (sumOf allTransactions (=) 42.0)

    let testProperty (property:Property) env ts : bool = evalProp property env ts


