namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Generators =
    //System.Random to use for generating random values.
    let random = new Random()
    
    //Generic function type for generating observable values for the government.
    type ValueGenerator  = Time -> ObservableValue

    //Wrapper for a map from map to Value generator. Used to store custom value generators.
    type ValueGenerators = Map<string,ValueGenerator>

    //Generic function type for generating environment for a contract
    type EnvironmentGenerator = Contract -> Environment

    //Default boolean generators
    let rndBoolShiftedGen f  : ValueGenerator = fun t -> BoolValue (random.NextDouble() <= f);
    let rndBoolGen           : ValueGenerator = rndBoolShiftedGen 0.5
    let boolAtTimeGen t      : ValueGenerator = fun n -> BoolValue (n = t) 
    let boolNotAtTimeGen t   : ValueGenerator = fun n -> BoolValue (n <> t)

    //Wrapper type for accessing bool generators
    [<Sealed>]
    type BoolGenerators =
        static member RandomBool        : ValueGenerator            = rndBoolGen
        static member RandomBoolShifted : float -> ValueGenerator   = rndBoolShiftedGen
        static member BoolTrueAtTime    : Time -> ValueGenerator    = boolAtTimeGen
        static member BoolFalseAtTime   : Time -> ValueGenerator    = boolNotAtTimeGen
        static member Default           : ValueGenerator            = BoolGenerators.RandomBool

    //Numeric Generators
    let rndNumGen               : ValueGenerator = (fun t -> NumberValue(float(random.NextDouble()))) 
    let rndNumWithinGen min max : ValueGenerator = (fun t -> NumberValue(min + ((max - min) * float(random.NextDouble()))))
    
    //Wrapper type for accessing numeric generators
    [<Sealed>] 
    type NumericGenerators =
        static member RandomNumber        : ValueGenerator                     = rndNumGen
        static member RandomNumberInRange : float -> float -> ValueGenerator   = rndNumWithinGen
        static member Default             : ValueGenerator                     = NumericGenerators.RandomNumberInRange 1.0 10.0

    //If generator not present in map use default generator
    let findGenerator genMap defaultGenerator obs : ValueGenerator = 
        match Map.tryFind obs genMap with
            | Some generator -> generator
            | _ -> defaultGenerator
    
    //Functions for generating numeric/boolean values to the corresponding observables
    //returns a map containing those values
    let genBoolValues boolObs generators defaultGenerator t observableValues : Map<string,ObservableValue> = List.fold (fun bMap (BoolVal(obs)) -> bMap |> (addObservable (obs, ((findGenerator generators defaultGenerator obs) t)))) observableValues boolObs
    let genNumValues numObs generators defaultGenerator t observableValues   : Map<string,ObservableValue> = List.fold (fun bMap (NumVal(obs))  -> bMap |> (addObservable (obs, ((findGenerator generators defaultGenerator obs) t)))) observableValues numObs
    
    //Generates enviroment for the entire horizon of contract. Maps contain optional generators for observables
    let generateEnvironment genMap c : Environment = 
        let horizon = getHorizon c
        let (boolObservables, numberObservables) = getObservables c
        
        let generateObservablesForTime t = (genBoolValues boolObservables genMap BoolGenerators.Default t Map.empty) |> genNumValues numberObservables genMap NumericGenerators.Default t
        
        let observables = Array.init horizon generateObservablesForTime
        (0, observables)
    
    //Wrapper type for accessing numeric generators
    [<Sealed>] 
    type EnvironmentGenerators =
        static member WithDefaultGenerators : EnvironmentGenerator = generateEnvironment Map.empty
        static member WithCustomGenerators : (ValueGenerators -> EnvironmentGenerator) = generateEnvironment
        static member Default : EnvironmentGenerator = EnvironmentGenerators.WithDefaultGenerators