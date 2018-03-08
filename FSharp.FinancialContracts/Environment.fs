namespace FSharp.FinancialContracts

module Environment =

    // Observable of type boolean.
    type BoolObs =
        | BoolVal of string
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

    // Evaluation of boolean observable, returns a boolean value.
    let rec evalBoolObs (obs : BoolObs) boolEnv numEnv : bool =
        match obs with
        | BoolVal(s) -> Map.find s boolEnv
        | And(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) && (evalBoolObs bool2 boolEnv numEnv)
        | Or(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) || (evalBoolObs bool2 boolEnv numEnv)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) > (evalNumberObs num2 numEnv boolEnv)
        | LessThan(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) < (evalNumberObs num2 numEnv boolEnv)
        | Not(bool) -> not (evalBoolObs bool boolEnv numEnv)

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
    
    //
    let rec boolObs (obs:BoolObs) boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | BoolVal(_) -> 
            if not (List.contains obs boolAcc) then
                (obs::boolAcc,numAcc)
            else
                (boolAcc, numAcc)
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
    // 
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
        | NumberObs.If(bool, num1, num2) -> 
            let (boolAcc1, numAcc1) = boolObs bool boolAcc numAcc
            let (boolAcc2, numAcc2) = numberObs num1 boolAcc1 numAcc1
            numberObs num2 boolAcc2 numAcc2


    // Type representing time.
    type Time = int
    // Environment contains the value of observables.
    type Environment = Time * Map<string, bool> array * Map<string, float> array
    // Pass the time of an Environment.
    let increaseTime t1 ((t2, obsEnv, numEnv):Environment) : Environment = (t1+t2, obsEnv, numEnv)
    // Get the current time of an Environment.
    let getTime ((t,_,_):Environment) : Time = t
    // Get the map containing the values of boolean observables.
    let getBoolEnv t ((_,boolEnv,_):Environment) : Map<string, bool> = boolEnv.[t]
    // Get the map containing the values of float observables.
    let getNumEnv t ((_,_,numEnv):Environment) : Map<string, float> = numEnv.[t]

    // Add a boolean observable to the environment.
    let addBoolObs (boolObs, bool) (boolEnv:Map<string, bool>) : Map<string, bool> = 
        match boolObs with 
        | BoolVal(s) -> boolEnv.Add(s, bool)
        | _ -> failwith "Only expects boolVal"
    
    // Add a float observable to the environment.
    let addNumObs (numObs, value) (numEnv:Map<string, float>) : Map<string, float> = 
        match numObs with 
        | NumVal(s) -> numEnv.Add(s, value)
        | _ -> failwith "Only expects numval"
