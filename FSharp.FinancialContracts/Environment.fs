namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Environment =
    
    // Environment contains the value of observables for all times and the current time.
    type Environment = Time * Map<string, bool> array * Map<string, float> array
    
    let increaseEnvTime t1 ((t2, obsEnv, numEnv):Environment) : Environment = (t1+t2, obsEnv, numEnv)
    let (|+) (t,m1,m2) td : Environment = increaseEnvTime td (t,m1,m2)   
    
     // Get the current time of an Environment.
    let getTime ((t,_,_):Environment) : Time = t
    // Get the map of boolean observables in an Environment at a specific point in time.
    let getBoolEnv ((_,boolEnv,_):Environment) : Map<string, bool>[] = boolEnv
    // Get the map of numerical observables in an Environment at a specific point in time.
    let getNumEnv ((_,_,numEnv):Environment) : Map<string, float>[] = numEnv
    
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

    // Evaluation of boolean observable, returns a boolean value.
    let rec evalBoolObs obs env : bool =
        let t = getTime env
        match obs with
        | BoolVal(s) -> Map.find s (getBoolEnv env).[t]
        | Bool(b) -> b
        | And(bool1, bool2) -> (evalBoolObs bool1 env) && (evalBoolObs bool2 env)
        | Or(bool1, bool2) -> (evalBoolObs bool1 env) || (evalBoolObs bool2 env)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 env) > (evalNumberObs num2 env)
        | LessThan(num1, num2) -> (evalNumberObs num1 env) < (evalNumberObs num2 env)
        | Not(bool) -> not (evalBoolObs bool env)
    // Evaluation of float observable, returns a float value.
    and evalNumberObs obs env : float =
        let t = getTime env
        match obs with
        | NumVal(s) -> Map.find s (getNumEnv env).[t]
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

    // Identifies the observables, that the provided boolean observable depends on.
    let rec boolObs (obs:BoolObs) boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | BoolVal(_) -> 
            if not (List.contains obs boolAcc) then
                (obs::boolAcc,numAcc)
            else
                (boolAcc, numAcc)
        | Bool(_) -> (boolAcc, numAcc)
        | And(bool1, bool2) -> 
            let (boolAcc1, numAcc1) = boolObs bool1 boolAcc numAcc
            boolObs bool2 boolAcc1 numAcc1
        | Or(bool1, bool2) -> 
            let (boolAcc1, numAcc1) = boolObs bool1 boolAcc numAcc
            boolObs bool2 boolAcc1 numAcc1
        | GreaterThan(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | LessThan(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Not(bool) -> boolObs bool boolAcc numAcc
    // Identifies the observables, that the provided numerical observable depends on.
    and numberObs (obs:NumberObs) boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | NumVal(_) -> 
            if not (List.contains obs numAcc) then
                (boolAcc,obs::numAcc)
            else
                (boolAcc, numAcc)
        | Const(_) -> (boolAcc, numAcc)
        | Add(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Sub(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Mult(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | If(bool, num1, num2) -> 
            let (boolAcc1, numAcc1) = boolObs bool boolAcc numAcc
            let (boolAcc2, numAcc2) = numberObs num1 boolAcc1 numAcc1
            numberObs num2 boolAcc2 numAcc2
        | Average(num, _) -> numberObs num boolAcc numAcc
