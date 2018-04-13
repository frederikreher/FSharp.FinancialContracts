namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open System.Collections.Generic

module Property =
    
    //Type definitions
    type Property  = (Environment -> TransactionResults -> bool)
    type Filter    = (Transaction -> bool)
    type BinOp<'a> = ('a -> 'a -> bool)
    
    //Default filters:
    let allTransactions : Filter        = fun t -> true
    let transactionsOfCurrency a : Filter = fun (Transaction(f,ass)) -> a = ass
    
    //Simple property combinators 
    let notProperty p : Property         = fun env ts -> not (p env ts)
    let andProperty p1 p2 : Property     = fun env ts -> (p1 env ts) && (p2 env ts)
    let orProperty p1 p2 : Property      = fun env ts -> (p1 env ts) || (p2 env ts)
    let impliesProperty p1 p2 : Property = fun env ts -> (orProperty (notProperty p1) p2) env ts
    
    //Syntactic sugar for better readability
    let (!!) p      : Property = notProperty p
    let (&|&) p1 p2 : Property = andProperty p1 p2
    let (|||) p1 p2 : Property = orProperty p1 p2
    let (=|>) p1 p2 : Property = impliesProperty p1 p2
    
    //Helper function mapping true or false values to value of a transaction
    let sumByFilter filter : Transaction -> float = fun (Transaction(f,c)) -> if filter (Transaction(f,c)) then f else 0.0
    
    //Advanced properties
    let sumOf filter compare (f:float) : Property = fun _ transResults -> compare f (Array.sumBy (List.sumBy (sumByFilter filter)) (getTransactions transResults))
    let countOf filter compare (n:int) : Property = fun _ transResults -> compare n (Array.sumBy (fun t -> List.length (List.where filter t)) (getTransactions transResults))
    
    let sumOfDKKAre20  : Property = sumOf (transactionsOfCurrency DKK) (=) 20.0
    let countOfAllAre1 : Property = countOf allTransactions (=) 2
    
    //Advanced Combinators
    let atTime (t:Time) p : Property = fun env ts  -> 
        let ts' = increaseTime ts t
        p (env|+t) ts'
    
    let listOfTransactionTimes (_,tsr) = 
        let length = Array.length tsr
        [0..length-1]
    
    let timefulfilling = (fun p tsr env t -> 
                    let tsr' = increaseTime tsr t
                    p (env|+t) tsr')
    
    let forAllTimes p : Property = fun env tsr -> List.forall (timefulfilling p tsr env)  (listOfTransactionTimes tsr)
    let forSomeTime p : Property = fun env tsr -> List.exists (timefulfilling p tsr env) (listOfTransactionTimes tsr)
    let forOneTime p  : Property = fun env tsr -> 1 = List.length (List.where (timefulfilling p tsr env) (listOfTransactionTimes tsr))
    
    let addSums transactions acc : Map<Currency,float> =
        let addToMap = fun map (Transaction(v,cur)) -> 
            if Map.containsKey cur map then map.Add (cur,(v+map.[cur]))
            else map.Add (cur,v)
        List.fold addToMap acc transactions
    
    //Check if the sums of the transactions of the current time are of zero sum
    let isZero : Property = fun _ transactionResults -> 
        let transactions = getTransactions transactionResults
        let sums = addSums transactions.[0] Map.empty
        match Map.tryFindKey (fun k v -> v <> 0.0) sums with
        | Some(_) -> false
        | None -> true
    
    let satisfyBoolObs obs : Property                       = fun env _ -> evalBoolObs obs env
    let satisfyNumObs obs (binOp:BinOp<float>) f : Property = fun env _ -> binOp (evalNumberObs obs env) f
    
    //Check if transactions of current day contains specific transaction
    let hasTransactions trans : Property = fun _ transactionResults -> 
        let transactions = getTransactions transactionResults
        List.forall (fun tr -> List.contains tr transactions.[0]) trans
        
    let hasNoTransactions : Property = fun _ transactionResults -> 
       let transactions = getTransactions transactionResults
       (List.length transactions.[0]) <= 0
    