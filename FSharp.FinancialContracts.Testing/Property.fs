namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type Property = (Environment -> Transaction list -> bool)


    type BinOp = (float -> float -> bool)
    type Filter = (Transaction -> bool)

    let sumByFilter filter = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    let s = op_Equality

    let SumOf filter binop f = binop f (fun ts -> List.sumBy (sumByFilter filter))

    type TransactionProperty = 
        | And of TransactionProperty * TransactionProperty
        | Or of TransactionProperty * TransactionProperty
        | SumOf of Filter * BinOp * float 
        | CountOf of Filter * BinOp * int 
        | ForAllTimes of Property
        | At of Time * Property
    
    ForAllTimes(SumOf())

    let rec evalProp property env (transactionsByDay as t) : bool = 
        match property with
            | And(p1,p2) -> evalProp p1 env t && evalProp p2 env t
            | Or(p1,p2) -> evalProp p1 env t || evalProp p2 env t
            | SumOf(filter,op,f) -> op f (Array.sumBy (fun ts -> List.sumBy (sumByFilter filter) ts) t)
            | ForAllTimes(p) -> 
               match Array.tryFind (fun tlist -> not (p env tlist)) t with
               | Some(_) -> false 
               | None -> true
            | At(t,p) ->
                let ts = transactionsByDay.[t]
                p env ts
    
    let (&&) p1 p2 : TransactionProperty = And(p1,p2)
    let (||) p1 p2 : TransactionProperty = Or(p1,p2)

    let testProperty prop env ts = evalProp prop env ts
