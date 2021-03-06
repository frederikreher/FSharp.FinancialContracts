﻿namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open System.Collections.Generic

module Environment =
    
    // Environment contains the value of observables for all times and the current time.
    type Environment = Time * Map<string,ObservableValue> array * (string -> ObservableValue -> Time -> unit)
    
    // Increases the time of the provided environment by the provided value.
    let increaseEnvTime t1 ((t2, obs,f):Environment) : Environment = 
        let newTime = t1 + t2
        if newTime >= obs.Length then failwith "The current time of the environment cannot exceed the horizon of the contract"
        else (newTime, obs,f)
    let (|+) env t : Environment = increaseEnvTime t env
    
     // Get the current time of an Environment.
    let getTime ((t,_,_):Environment) : Time = t
    
    // Add the provided observable to the map of observables.
    let addObservable kv (observables:Map<string, ObservableValue>) : Map<string, ObservableValue> 
        = observables.Add kv

    // Find the current value of a BoolVal observable in the environment.
    let findBoolInEnv s ((t,obs,f):Environment) = 
        let observableValue = Map.tryFind s obs.[t]
        match observableValue with
            | Some(BoolValue boolValue) -> f s (BoolValue boolValue) t
                                           boolValue
            | None                      -> failwith (sprintf "Boolean Observable %A doesn't exist in environment" s)
            | _                         -> failwith "Expected boolean observable"
    
    // Find the current value of a NumVal observable in the environment.
    let findNumberInEnv s ((t,obs,f):Environment) = 
        let observableValue = Map.tryFind s obs.[t]
        match observableValue with
            | Some(NumberValue numValue) -> f s (NumberValue numValue) t
                                            numValue
            | None                       -> failwith (sprintf "Numeric Observable %A doesn't exist in environment" s)
            | _                          -> failwith "Expected numbervalue observable"
            
        
    // Evaluation of boolean observable, returns a boolean value.
    let rec evalBoolObs obs env : bool =
        match obs with
        | BoolVal(s)              -> findBoolInEnv s env
        | Bool(b)                 -> b
        | And(bool1, bool2)       -> (evalBoolObs bool1 env) && (evalBoolObs bool2 env)
        | Or(bool1, bool2)        -> (evalBoolObs bool1 env) || (evalBoolObs bool2 env)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 env) > (evalNumberObs num2 env)
        | LessThan(num1, num2)    -> (evalNumberObs num1 env) < (evalNumberObs num2 env)
        | Equal(num1, num2)       -> (evalNumberObs num1 env) = (evalNumberObs num2 env)
        | Not(bool)               -> not (evalBoolObs bool env)
    // Evaluation of time observable, returns a Time value.
    and evalTimeObs obs env : Time = 
        match obs with 
        | TimeObs.Const t         -> t
        | TimeObs.If(bObs,t1,t2)  -> if evalBoolObs bObs env then evalTimeObs t1 env else evalTimeObs t2 env
        | TimeObs.Add(t1,t2)      -> (evalTimeObs t1 env) + (evalTimeObs t2 env)
        | TimeObs.Mult(t1,t2)     -> (evalTimeObs t1 env) * (evalTimeObs t2 env)
    // Evaluation of float observable, returns a float value.
    and evalNumberObs obs env : float =
        match obs with
        | NumVal(s)               -> findNumberInEnv s env
        | Const(f)                -> f
        | Add(num1, num2)         -> (evalNumberObs num1 env) + (evalNumberObs num2 env)
        | Sub(num1, num2)         -> (evalNumberObs num1 env) - (evalNumberObs num2 env)
        | Mult(num1, num2)        -> (evalNumberObs num1 env) * (evalNumberObs num2 env)
        | If(bool, num1, num2)    -> if (evalBoolObs bool env) then (evalNumberObs num1 env)
                                     else (evalNumberObs num2 env)
        | Average(num, t)         -> if 0 > ((getTime env) - t) then failwith "The observable period has to be within the horizon of the contract"
                                     else 
                                     let res = List.fold (fun sum i -> sum + (evalNumberObs num (increaseEnvTime i env))) 0.0 [(-t)..0]
                                     res / float(t+1)