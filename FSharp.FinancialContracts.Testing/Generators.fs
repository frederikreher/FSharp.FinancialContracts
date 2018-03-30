﻿namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract

module Generators =
    let random = new Random()

    //Generic function type for generating values at a point in time, time might likely be ignored
    type ValueGenerator<'a> = Time -> 'a

    //Wrapper for map from either BoolObs or NumberObs to generators
    type ValueGenerators<'a,'b> when 'a : comparison = Map<'a,ValueGenerator<'b>>

    //Generic function type for generating environment for a contract
    type EnvironmentGenerator = Contract -> Environment

    (*Default bool generators *)
    let rndBoolShiftGen f : ValueGenerator<bool> = fun t -> random.NextDouble() <= f;
    let rndBoolGen : ValueGenerator<bool>        = rndBoolShiftGen 0.5
    let boolAtDateGen t : ValueGenerator<bool>   = fun n -> n = t 
    let boolNotAtDateGen t : ValueGenerator<bool>   = fun n -> n <> t

    [<Sealed>]
    type BoolGenerators =
       static member RandomBool : ValueGenerator<bool>                   = rndBoolGen
       static member RandomBoolShifted : (float -> ValueGenerator<bool>) = rndBoolShiftGen
       static member BoolTrueAtDate : (Time -> ValueGenerator<bool>)     = boolAtDateGen
       static member BoolFalseAtDate : (Time -> ValueGenerator<bool>)    = boolNotAtDateGen
       static member Default : ValueGenerator<bool>                      = BoolGenerators.RandomBool

    (*Default numeric generators *)
    let rndNumGen : ValueGenerator<float>               = (fun t -> float(random.NextDouble())) 
    let rndNumWithinGen min max : ValueGenerator<float> = (fun t -> min + ((max - min) * float(random.NextDouble())))

    [<Sealed>] 
    type NumericGenerators =
       static member RandomNumber : ValueGenerator<float>                                = rndNumGen
       static member RandomNumberWithinRange : (float -> float -> ValueGenerator<float>) = rndNumWithinGen
       static member Default : ValueGenerator<float>                                     = NumericGenerators.RandomNumberWithinRange 1.0 10.0

    //If generator not present in map use default generator
    let findGen genMap defGen obs : ValueGenerator<'a> = 
                              match Map.tryFind obs genMap with
                                 | Some g -> g
                                 | _ -> defGen

    //Functions for generating numeric/boolean values to the corresponding observables
    //returns a map containing those values
    let genBoolValues boolObs genMap gen t : Map<string,bool>  = List.fold (fun bMap obs -> bMap |> (addBoolObs (obs, ((findGen genMap gen obs) t)))) Map.empty boolObs
    let genNumValues numObs genMap gen t : Map<string,float>   = List.fold (fun bMap obs -> bMap |> (addNumObs (obs,  ((findGen genMap gen obs) t)))) Map.empty numObs

    //Generates enviroment for the entire horizon of contract. Maps contain optional generators for observables
    let generateEnvironment numGenMap boolGenMap c : Environment = 
        let horizon = getHorizon c
        let (boolObs, numsObs) = getObservables c
       
        let boolArray = Array.init horizon (genBoolValues boolObs boolGenMap BoolGenerators.Default)
        let numArray  = Array.init horizon (genNumValues numsObs numGenMap NumericGenerators.Default)
       
        (0, boolArray, numArray)
    
    [<Sealed>] 
    type EnvironmentGenerators =
        static member WithDefaultGenerators : EnvironmentGenerator = generateEnvironment Map.empty Map.empty
        static member WithCustomGenerators : (ValueGenerators<NumberObs,float> -> ValueGenerators<BoolObs,bool> -> EnvironmentGenerator) = generateEnvironment
        static member Default : EnvironmentGenerator = EnvironmentGenerators.WithDefaultGenerators