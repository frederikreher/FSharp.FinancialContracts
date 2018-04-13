namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Environment =
    
    // Environment contains the value of observables for all times and the current time.
    type Environment = Time * Map<string,ObservableValue> array
    
    let increaseEnvTime t1 ((t2, obs):Environment) : Environment = (t1+t2, obs)
    let (|+) env td : Environment = increaseEnvTime td env
    
     // Get the current time of an Environment.
    let getTime ((t,_):Environment) : Time = t
    
    // Add a boolean observable to the map of boolean observables.
    let addBoolObs (boolObs, bool) (boolEnv:Map<string, bool>) : Map<string, bool> = 
        match boolObs with 
        | BoolVal(s) -> boolEnv.Add(s, bool)
        | _ -> failwith "Only expects boolVal"
    // Add a float observable to the map of numerical observables.
    let addNumObs (numObs, value) (numEnv:Map<string, float>) : Map<string, float> = 
        match numObs with 
        | NumVal(s) -> numEnv.Add(s, value)
        | _ -> failwith "Only expects numVal"
    
    let addObservable kv (observables:Map<string, ObservableValue>) : Map<string, ObservableValue> = observables.Add kv
    
    let findBoolInEnv s ((t,obs):Environment) = 
        let observableValue = Map.tryFind s obs.[t]
        match observableValue with
            | Some(BoolValue boolValue) -> boolValue
            | None -> failwith "Observable doesn't exist in environment"
            | _ -> failwith "Expected boolean observable"
    
    let findNumberInEnv s ((t,obs):Environment) = 
        let observableValue = Map.tryFind s obs.[t]
        match observableValue with
            | Some(NumberValue numValue) -> numValue
            | None -> failwith "Observable doesn't exist in environment"
            | _ -> failwith "Expected numbervalue observable"
            
        
    // Evaluation of boolean observable, returns a boolean value.
    let rec evalBoolObs obs env : bool =
        let t = getTime env
        match obs with
        | BoolVal(s) -> findBoolInEnv s env
        | Bool(b) -> b
        | And(bool1, bool2) -> (evalBoolObs bool1 env) && (evalBoolObs bool2 env)
        | Or(bool1, bool2) -> (evalBoolObs bool1 env) || (evalBoolObs bool2 env)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 env) > (evalNumberObs num2 env)
        | LessThan(num1, num2) -> (evalNumberObs num1 env) < (evalNumberObs num2 env)
        | Equal(num1, num2) -> (evalNumberObs num1 env) = (evalNumberObs num2 env)
        | Not(bool) -> not (evalBoolObs bool env)
    // Evaluation of float observable, returns a float value.
    and evalNumberObs obs env : float =
        let t = getTime env
        match obs with
        | NumVal(s) -> findNumberInEnv s env
        | Const(f) -> f
        | Add(num1, num2) -> (evalNumberObs num1 env) + (evalNumberObs num2 env)
        | Sub(num1, num2) -> (evalNumberObs num1 env) - (evalNumberObs num2 env)
        | Mult(num1, num2) -> (evalNumberObs num1 env) * (evalNumberObs num2 env)
        | If(bool, num1, num2) -> 
            if (evalBoolObs bool env) then
                (evalNumberObs num1 env)
            else
                (evalNumberObs num2 env)
        | Average(num, t) -> 
            if t > (getTime env) then
                failwith "The observable period cannot exceed the horizon of the contract"
            else
                //let res = List.fold (fun sum time -> sum + (evalNumberObs num env)) 0.0 [0..(t-1)]
                let res = List.fold (fun sum _ -> sum + (evalNumberObs num (increaseEnvTime -1 env))) 0.0 [0..(t-1)]
                res / float(t)