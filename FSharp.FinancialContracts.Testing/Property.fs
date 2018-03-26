﻿namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Property =
    open System.Collections.Generic

    //Type definitions
    type Property  = (Environment -> TransactionResults -> bool)
    type BinOp<'a> = ('a -> 'a -> bool)
    type Filter    = (Transaction -> bool)
    
    //Default filters:
    let allTransactions : Filter       = fun t -> true
    let transactionsOfAsset a : Filter = fun (Transaction(f,ass)) -> a = ass
    
    //Simple property combinators 
    let notProperty p : Property         = fun env ts -> not (p env ts)
    let andProperty p1 p2 : Property     = fun env ts -> (p1 env ts) && (p2 env ts)
    let orProperty p1 p2 : Property      = fun env ts -> (p1 env ts) || (p2 env ts)
    let impliesProperty p1 p2 : Property = fun env ts -> (orProperty (notProperty p1) p2) env ts
    
    //Syntactic sugar for better readability
    let (|!)  p     : Property = notProperty p
    let (&|&) p1 p2 : Property = andProperty p1 p2
    let (|||) p1 p2 : Property = orProperty p1 p2
    let (=|>) p1 p2 : Property = impliesProperty p1 p2
    
    //Helper function mapping true or false values to value of a transaction
    let sumByFilter filter : Transaction -> float = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    
    //Advanced properties
    let sumOf filter binop (f:float) : Property = fun _ transResults -> binop f (Array.sumBy (List.sumBy (sumByFilter filter)) (getTransactions transResults))
    let countOf filter binop (n:int) : Property = fun _ transResults -> binop n (Array.sumBy (fun t -> List.length (List.where filter t)) (getTransactions transResults))
    
    //Advanced Combinators
    let atTime (t:Time) p : Property = fun env ts  -> 
        let ts' = increaseTime ts t
        p (env|+t) ts'
    
    let forAllTimes p : Property = fun env tsr ->
        let length = Array.length (snd tsr)
        let timeNotfulfulling = (fun t -> 
            let tsr' = increaseTime tsr t
            not (p (env|+t) tsr'))
        match List.tryFind timeNotfulfulling [0..length-1] with 
        | Some(_) -> false
        | None -> true 
    
    let forSomeTime p : Property = fun env (tst,ts) ->
        match List.tryFind (fun t -> p (env|+t) (tst,ts)) [0..(Array.length ts)-1] with 
        | Some(_) -> true
        | None -> false 
    
    let addSums transactions acc : Map<Currency,float> =
        let addToMap = fun map (Transaction(v,cur)) -> 
            if Map.containsKey cur map then map.Add (cur,(v+map.[cur]))
            else map.Add (cur,v)
        List.fold addToMap acc transactions
    
    let isZero : Property = fun _ transactionResults -> 
        let transactions = getTransactions transactionResults
        let sums = addSums transactions.[0] Map.empty
        match Map.tryFindKey (fun k v -> v <> 0.0) sums with
        | Some(_) -> false
        | None -> true
    
    let satisfyBoolObs obs : Property        = fun env _ -> evalBoolObs obs env
    let satisfyNumObs obs (binOp:BinOp<float>) f : Property = fun env _ -> binOp (evalNumberObs obs env) f
    
    //And, Or, Implies, Not, IsZero, AtTime, ForAllTimes, ForSomeTime, (Satisfy BoolObs)
    