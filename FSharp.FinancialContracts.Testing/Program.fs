// Learn more about F# at http://fsharp.org

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

[<EntryPoint>]
let main argv =
    let c = Scale(Add(NumVal "x", NumVal "y"),One(DKK))

    let generateObservables t hori c : Environment = 
        let rndBool = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
        let rndNum = (fun () -> float((new Random()).NextDouble())) 

        let (bools, nums) = getObservables c
        let newBoolEnv = List.fold (fun acc index -> 
                                     let boolEnv = List.fold (fun acc1 obs -> (addBoolObs (obs, rndBool()) acc1)) Map.empty bools
                                     Array.set acc index boolEnv
                                     acc
                                   ) (Array.create (hori+1) Map.empty) [0..(hori+1)]
        let newNumEnv = List.fold (fun acc index -> 
                                     let numEnv = List.fold (fun acc1 obs -> (addNumObs (obs, rndNum()) acc1)) Map.empty nums
                                     Array.set acc index numEnv
                                     acc
                                   ) (Array.create (hori+1) Map.empty) [0..(hori+1)]
        (t, newBoolEnv, newNumEnv)

    
    let propertyCheck c (property: Environment -> Transaction list -> bool) = 
        [for t in [1..1000] do 
            let env = generateObservables t (getHorizon c) c 
            let transactions = evalC env c
            yield property env transactions
            ]
    
    let res = propertyCheck c (fun e t -> true)

    printf "%A" res

    0 // return an integer exit code
