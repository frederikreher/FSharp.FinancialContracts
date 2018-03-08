// Learn more about F# at http://fsharp.org

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

[<EntryPoint>]
let main argv =
    let c = Scale(Add(NumVal "x", NumVal "y"),One(DKK))

    let generateObservables t c = 
        let rndBool = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
        let rndNum = (fun () -> float((new Random()).NextDouble())) 

        let (bools, nums) = getObservables c
        let newBoolEnv = List.fold (fun acc obs -> (addBoolObs (obs, rndBool()) acc)) Map.empty bools
        let newNumEnv = List.fold (fun acc obs -> 
                                   match obs with
                                   | NumVal(s) ->
                                       (addNumObs (obs, rndNum()) acc)
                                   | _ -> failwith "Not yet implemented"
                                 ) Map.empty nums
        (t, newBoolEnv, newNumEnv)

    
    //let propertyCheck c (property: Environment -> Transaction list -> bool) = 
        //[for t in [1..1000] do 
            //let env = generateObservables t c 
            //let transactions = evalC env c
            //yield property env transactions
            //]
    
    //let res = propertyCheck c (fun e t -> true)

    printf "%A" 1

    0 // return an integer exit code
