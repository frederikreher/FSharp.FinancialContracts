namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Generators =
    let random = new Random()

    //Generic function type for generating values at a point in time, time might likely be ignored
    type ValueGenerator = Time -> ObservableValue

    //Wrapper for map from either BoolObs or NumberObs to generators
    type ValueGenerators = Map<string,ValueGenerator>

    //Generic function type for generating environment for a contract
    type EnvironmentGenerator = Contract -> Environment

    (*Default bool generators *)
    let rndBoolShiftGen f : ValueGenerator = fun t -> BoolValue (random.NextDouble() <= f);
    let rndBoolGen : ValueGenerator      = rndBoolShiftGen 0.5
    let boolAtDateGen t : ValueGenerator   = fun n -> BoolValue (n = t) 
    let boolNotAtDateGen t : ValueGenerator   = fun n -> BoolValue (n <> t)

    [<Sealed>]
    type BoolGenerators =
       static member RandomBool : ValueGenerator                   = rndBoolGen
       static member RandomBoolShifted : (float -> ValueGenerator) = rndBoolShiftGen
       static member BoolTrueAtDate : (Time -> ValueGenerator )    = boolAtDateGen
       static member BoolFalseAtDate : (Time -> ValueGenerator)    = boolNotAtDateGen
       static member Default : ValueGenerator                      = BoolGenerators.RandomBool

    (*Default numeric generators *)
    let rndNumGen : ValueGenerator             = (fun t -> NumberValue( float(random.NextDouble()))) 
    let rndNumWithinGen min max : ValueGenerator = (fun t -> NumberValue(min + ((max - min) * float(random.NextDouble()))))

    [<Sealed>] 
    type NumericGenerators =
       static member RandomNumber : ValueGenerator                                = rndNumGen
       static member RandomNumberWithinRange : (float -> float -> ValueGenerator) = rndNumWithinGen
       static member Default : ValueGenerator                                     = NumericGenerators.RandomNumberWithinRange 1.0 10.0

    //If generator not present in map use default generator
    let findGen genMap defGen obs : ValueGenerator = 
                              match Map.tryFind obs genMap with
                                 | Some g -> g
                                 | _ -> defGen
    
//    type Observable = 
//        | NumberObservable of NumberObs
//        | BooleanObservable of BoolObs
//    
//    let getKeyFromObs obs = 
//        match obs with 
//        | NumberObservable numObs -> 
//            match numObs with 
//                | NumVal(s) -> s
//                | _ -> failwith "Attepting"
//        | BooleanObservable boolObs ->
//            match boolObs with 
//                | BoolVal(s) -> s
//                | _ -> failwith "Attepting"
    
    //Functions for generating numeric/boolean values to the corresponding observables
    //returns a map containing those values
    let genBoolValues boolObs genMap gen t acc : Map<string,ObservableValue>  = List.fold (fun bMap (BoolVal(obs)) -> bMap |> (addObservable (obs, ((findGen genMap gen obs) t)))) acc boolObs
    let genNumValues numObs genMap gen t acc: Map<string,ObservableValue>   = List.fold (fun bMap (NumVal(obs)) -> bMap |> (addObservable (obs,  ((findGen genMap gen obs) t)))) acc numObs
    
    //let genObservableValue numObs genMap gen t acc: Map<string,ObservableValue>   = List.fold (fun bMap obs -> bMap |> (addObservable ( (getKeyFromObs obs),  ((findGen genMap gen (getKeyFromObs obs)) t)))) acc numObs
    
    //Generates enviroment for the entire horizon of contract. Maps contain optional generators for observables
    let generateEnvironment genMap c : Environment = 
        let horizon = getHorizon c
        let (boolObs, numsObs) = getObservables c
        
        let generateObservables t = (genBoolValues boolObs genMap BoolGenerators.Default t Map.empty) |> genNumValues numsObs genMap NumericGenerators.Default t
        
        let observables = Array.init horizon generateObservables
       
       
        (0, observables)
    
    [<Sealed>] 
    type EnvironmentGenerators =
        static member WithDefaultGenerators : EnvironmentGenerator = generateEnvironment Map.empty
        static member WithCustomGenerators : (ValueGenerators -> EnvironmentGenerator) = generateEnvironment
        static member Default : EnvironmentGenerator = EnvironmentGenerators.WithDefaultGenerators