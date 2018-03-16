namespace FSharp.FinancialContracts

module Environment =
            
    // Type representing time.
    type Time = int

    // Environment contains the value of observables for all times and the current time.
    type Environment = Time * Map<string, bool> array * Map<string, float> array

    // Observable of type boolean.
    type BoolObs =
        | BoolVal of string
        | Bool of bool
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan of NumberObs * NumberObs
        | Not of BoolObs
    // Observable of type float.
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs
        | Average of NumberObs * Time

    // Pass the time of an Environment.
    let increaseTime t1 ((t2, obsEnv, numEnv):Environment) : Environment = (t1+t2, obsEnv, numEnv)
    // Get the current time of an Environment.
    let getTime ((t,_,_):Environment) : Time = t
    // Get the map of boolean observables in an Environment at a specific point in time.
    let getBoolEnv t ((_,boolEnv,_):Environment) : Map<string, bool> = boolEnv.[t]
    // Get the map of numerical observables in an Environment at a specific point in time.
    let getNumEnv t ((_,_,numEnv):Environment) : Map<string, float> = numEnv.[t]

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
    let rec evalBoolObs obs t env : bool =
        match obs with
        | BoolVal(s) -> Map.find s (getBoolEnv t env)
        | Bool(b) -> b
        | And(bool1, bool2) -> (evalBoolObs bool1 t env) && (evalBoolObs bool2 t env)
        | Or(bool1, bool2) -> (evalBoolObs bool1 t env) || (evalBoolObs bool2 t env)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 t env) > (evalNumberObs num2 t env)
        | LessThan(num1, num2) -> (evalNumberObs num1 t env) < (evalNumberObs num2 t env)
        | Not(bool) -> not (evalBoolObs bool t env)
    // Evaluation of float observable, returns a float value.
    and evalNumberObs obs t env : float =
        match obs with
        | NumVal(s) -> Map.find s (getNumEnv t env)
        | Const(f) -> f
        | Add(num1, num2) -> (evalNumberObs num1 t env) + (evalNumberObs num2 t env)
        | Sub(num1, num2) -> (evalNumberObs num1 t env) - (evalNumberObs num2 t env)
        | Mult(num1, num2) -> (evalNumberObs num1 t env) * (evalNumberObs num2 t env)
        | If(bool, num1, num2) -> 
            if (evalBoolObs bool t env) then
                (evalNumberObs num1 t env)
            else
                (evalNumberObs num2 t env)
        | Average(num, t) -> 
            if t > (getTime env) then
                failwith "The observable period cannot exceed the horizon of the contract"
            else
                let res = List.fold (fun sum time -> sum + (evalNumberObs num ((getTime env) - time) env)) 0.0 [0..(t-1)]
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
