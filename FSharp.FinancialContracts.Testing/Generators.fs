namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Generators =
    type ValueGenerator<'a> = Time -> 'a

    let rndBoolShift f : ValueGenerator<bool> = (fun t -> (if (new Random()).NextDouble() <= f then true else false));
    let rndBool : ValueGenerator<bool> = rndBoolShift 0.5

    let rndNum : ValueGenerator<float>= (fun t -> float((new Random()).NextDouble())) 
    let rndNumWitin min max : ValueGenerator<float>= (fun t -> min + ((max - min) * float((new Random()).NextDouble())))

    //If generator not present in map use default generator
    let findGen genMap gen obs : ValueGenerator<'a> = 
                              match Map.tryFind obs genMap with
                                 | Some g -> g
                                 | _ -> gen

    let genBools bools genMap gen t  = List.fold (fun bMap obs -> bMap |> (addBoolObs (obs, ((findGen genMap gen obs) t)))) Map.empty bools
    let genNums nums genMap gen t    = List.fold (fun bMap obs -> bMap |> (addNumObs (obs,  ((findGen genMap gen obs) t)))) Map.empty nums


    let generateEnvironment c numGenMap boolGenMap numGen boolGen : Environment = 
        let horizon = 1+(getHorizon c)
        let (bools, nums) = getObservables c
       
        let boolArray = Array.init horizon (genBools bools boolGenMap boolGen)
        let numArray = Array.init horizon (genNums nums numGenMap numGen)
       
        (0, boolArray, numArray)