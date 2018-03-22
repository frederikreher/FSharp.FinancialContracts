namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    type Property = (Environment -> Transaction list -> bool)
    
    type BinOp<'a> = ('a -> 'a -> bool)
    type Filter = (Transaction -> bool)

    let sumByFilter filter = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    let s = op_Equality

    let SumOf filter binop (f:float) : Property = fun _ ts -> binop f (List.sumBy (sumByFilter filter) ts)
    let CountOf filter binop (n:int) : Property = fun _ ts -> binop n ((List.where filter ts) |> List.length)

    type TransactionProperty = 
        | And of TransactionProperty * TransactionProperty
        | Or of TransactionProperty * TransactionProperty
        | ForAllTimes of Property
        | At of Time * Property
        | Not of TransactionProperty

    let rec evalProp property env (transactionsByDay as t) : bool = 
        match property with
            | And(p1,p2) -> evalProp p1 env t && evalProp p2 env t
            | Or(p1,p2) -> evalProp p1 env t || evalProp p2 env t
            | ForAllTimes(p) -> 
               match Array.tryFind (fun tlist -> not (p env tlist)) t with
               | Some(_) -> false 
               | None -> true
            | At(t,p) ->
                let ts = transactionsByDay.[t]
                p env ts
            | Not(p) -> not (evalProp p env t)
    
    let (&&) p1 p2 : TransactionProperty = And(p1,p2)
    let (||) p1 p2 : TransactionProperty = Or(p1,p2)

    let testProperty prop env ts = evalProp prop env ts

    //And, Or, Implies, Not, IsZero, AtTime, ForAllTimes, ForSomeTime, (Satisfy BoolObs)