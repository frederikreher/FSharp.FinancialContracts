namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Generators =
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