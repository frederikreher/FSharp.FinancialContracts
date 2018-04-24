namespace FSharp.FinancialContracts.Documentation
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract

module ContractEvaluation =
    
    // Creates a list with n amount of empty lists.
    let listOfLength n : Transaction list list = List.init n (fun _ -> [])

    // Scales each transaction in a Transaction list with the provide value.
    let multiply ts factor = List.map (fun (Transaction(x, c)) -> Transaction(x * factor, c)) ts
    
    // Merge two lists.
    // Copied from https://stackoverflow.com/questions/4100251/merge-two-lists-in-f-recursively
    let rec union l1 l2 = 
      match l1,l2 with
      | [],l | l,[] -> l
      | x::xs', y::ys' -> (y@x) :: (union xs' ys')

    // Evaluates a contract and returns an array of list of Transactions.
    let rec evaluateContract environment contract : Transaction list list = 
        match contract with
            | Zero               -> [[]]
            | One(currency)      -> [[Transaction(1.0,currency)]]
            | Delay(t, c)        -> listOfLength (evalTimeObs t environment) @ evaluateContract environment c
            | Scale(obs, c)      -> let subTransactions = evaluateContract environment c
                                    List.mapi (fun i transactions -> multiply transactions (evalNumberObs obs (environment |+ i))) subTransactions
            | ScaleNow(obs, c)   -> let subTransactions = evaluateContract environment c
                                    let currentFactor = evalNumberObs obs environment
                                    List.map (fun transactions -> multiply transactions currentFactor) subTransactions
            | And(c1, c2)        -> union (evaluateContract environment c1) (evaluateContract environment c2)
            | If(obs, t, c1, c2) -> let t' = (evalTimeObs t environment)
                                    if evalBoolObs obs environment then evaluateContract environment c1
                                    else if t' <= 0 then evaluateContract environment c2
                                    else []::(evaluateContract (environment|+1) (If(obs, TimeObs.Const(t'-1), c1, c2)))
            | Give(c)            -> let subTransactions = evaluateContract environment c
                                    List.map (fun transactions -> multiply transactions -1.0) subTransactions
                
