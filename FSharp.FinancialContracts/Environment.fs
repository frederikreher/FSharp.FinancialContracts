namespace FSharp.FinancialContracts

module Environment =

    // Observable of type boolean.
    type BoolObs =
        | BoolVal of string
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
    // Observable of type float.
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs

    // Evaluation of boolean observable, returns a boolean value.
    let rec evalBoolObs (obs : BoolObs) boolEnv numEnv : bool =
        match obs with
        | BoolVal(s) -> Map.find s boolEnv
        | And(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) && (evalBoolObs bool2 boolEnv numEnv)
        | Or(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) || (evalBoolObs bool2 boolEnv numEnv)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) > (evalNumberObs num2 numEnv boolEnv)

    // Evaluation of float observable, returns a float value.
    and evalNumberObs obs numEnv boolEnv : float =
        match obs with
        | NumVal(s) -> Map.find s numEnv
        | Const(f) -> f
        | Add(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) + (evalNumberObs num2 numEnv boolEnv)
        | Sub(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) - (evalNumberObs num2 numEnv boolEnv)
        | Mult(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) * (evalNumberObs num2 numEnv boolEnv)
        | If(bool, num1, num2) -> 
            if (evalBoolObs bool boolEnv numEnv) then
                (evalNumberObs num1 numEnv boolEnv)
            else
                (evalNumberObs num2 numEnv boolEnv)

    // Type representing time.
    type Time = int
    // Environment contains the value of observables.
    type Environment = Time * Map<string, bool> * Map<string, float>
    // Pass the time of an Environment.
    let increaseTime ((t, obsEnv, numEnv):Environment) : Environment = (t+1, obsEnv, numEnv)
    // Get the current time of an Environment.
    let getTime ((t,_,_):Environment) : Time = t
    // Get the map containing the values of boolean observables.
    let getBoolEnv ((_,boolEnv,_):Environment) : Map<string, bool> = boolEnv
    // Get the map containing the values of float observables.
    let getNumEnv ((_,_,numEnv):Environment) : Map<string, float> = numEnv

    // Add a boolean observable to the environment.
    let addBoolObs (boolObs, bool) (boolEnv:Map<string, bool>) : Map<string, bool> = 
        match boolObs with 
        | BoolVal(s) -> boolEnv.Add(s, bool)
        | _ -> failwith "Only expects boolVal"
    
    // Add a float observable to the environment.
    let addNumObs (numObs, value) (numEnv:Map<string, float>) : Map<string, float> = 
        match numObs with 
        | NumVal(s) -> numEnv.Add(s, value)
        | _ -> failwith "Only expects boolVal"
