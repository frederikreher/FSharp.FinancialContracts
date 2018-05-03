namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Generators =
    //System.Random to use for generating random values.
    let random = fun () -> new Random()
    
    //Generic function type for generating observable values for the government.
    type ValueGenerator  = Random -> Time -> ObservableValue

    //Wrapper for a map from map to Value generator. Used to store custom value generators.
    type ValueGenerators = Map<string,ValueGenerator>

    //Generic function type for generating environment for a contract
    type EnvironmentGenerator = Contract -> Environment

    //Default boolean generators
    let rndBoolShiftedGen f : ValueGenerator = fun r t -> BoolValue (r.NextDouble() <= f);
    let rndBoolGen          : ValueGenerator = rndBoolShiftedGen 0.5
    let boolAtTimeGen t     : ValueGenerator = fun _ n -> BoolValue (n = t) 
    let boolNotAtTimeGen t  : ValueGenerator = fun _ n -> BoolValue (n <> t)

    //Wrapper type for accessing bool generators
    [<Sealed>]
    type BoolGenerators =
        static member RandomBool        : ValueGenerator            = rndBoolGen
        static member RandomBoolShifted : float -> ValueGenerator   = rndBoolShiftedGen
        static member BoolTrueAtTime    : Time -> ValueGenerator    = boolAtTimeGen
        static member BoolFalseAtTime   : Time -> ValueGenerator    = boolNotAtTimeGen
        static member Default           : ValueGenerator            = BoolGenerators.RandomBool

    //Numeric Generators
    let rndNumGen               : ValueGenerator = (fun r t -> NumberValue(float(r.NextDouble()))) 
    let rndNumWithinGen min max : ValueGenerator = (fun r t -> NumberValue(min + ((max - min) * float(r.NextDouble()))))
    
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
    
    let getKeyFromBool obs = 
        match obs with 
        | BoolVal k -> k
        | _ -> failwith "only expecting BoolVal in observables from contracts"
    
    let getKeyFromNum obs = 
        match obs with 
        | NumVal k -> k
        | _ -> failwith "only expecting NumVal in observables from contracts"
    
    //Function for generating numeric/boolean values to the corresponding observables
    //returns a map containing those values  
    let generateObservables random time generators (boolObs,numObs) : Map<string,ObservableValue>=
        
        let genObs observables getString defaultGenerator observableValues: Map<string,ObservableValue> = 
            let generateAndAdd env obs = 
                let generate = (findGenerator generators defaultGenerator (getString obs))
                addObservable (getString obs, generate random time) env
            
            List.fold generateAndAdd observableValues observables
    
        let generatedBools = genObs boolObs getKeyFromBool BoolGenerators.Default Map.empty
        genObs numObs getKeyFromNum NumericGenerators.Default generatedBools
        
    
    //Generates enviroment for the entire horizon of contract. Maps contain optional generators for observables
    let generateEnvironment generators contract : Environment = 
        let random = new Random()
        
        let horizon = getHorizon contract
        let observables = getObservables contract
        
        let generateObservablesForTime t = generateObservables random t generators observables 
        
        let observableValues = Seq.initInfinite generateObservablesForTime
        (0, (Seq.cache observableValues),fun _ _ _ -> ())
    
    //Wrapper type for accessing numeric generators
    [<Sealed>] 
    type EnvironmentGenerators =
        static member WithDefaultGenerators : EnvironmentGenerator = generateEnvironment Map.empty
        static member WithCustomGenerators : (ValueGenerators -> EnvironmentGenerator) = generateEnvironment
        static member Default : EnvironmentGenerator = EnvironmentGenerators.WithDefaultGenerators