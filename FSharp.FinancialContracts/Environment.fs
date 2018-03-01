namespace FSharp.FinancialContracts

module Environment =

    type BoolObs =
        | BoolVal of string
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs

    let rec evalBoolObs (obs : BoolObs) boolEnv numEnv =
        match obs with
        | BoolVal(_) -> Map.find obs boolEnv
        | And(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) && (evalBoolObs bool2 boolEnv numEnv)
        | Or(bool1, bool2) -> (evalBoolObs bool1 boolEnv numEnv) || (evalBoolObs bool2 boolEnv numEnv)
        | GreaterThan(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) > (evalNumberObs num2 numEnv boolEnv)
    and evalNumberObs obs numEnv boolEnv =
        match obs with
        | NumVal(s) -> Map.find obs numEnv
        | Const(f) -> f
        | Add(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) + (evalNumberObs num2 numEnv boolEnv)
        | Sub(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) - (evalNumberObs num2 numEnv boolEnv)
        | Mult(num1, num2) -> (evalNumberObs num1 numEnv boolEnv) * (evalNumberObs num2 numEnv boolEnv)
        | If(bool, num1, num2) -> 
            if (evalBoolObs bool boolEnv numEnv) then
                (evalNumberObs num1 numEnv boolEnv)
            else
                (evalNumberObs num2 numEnv boolEnv)

    type Time = int

    type Environment = Time * Map<BoolObs, bool> * Map<NumberObs, float>

    let increaseTime ((t, obsEnv, numEnv):Environment) = (t+1, obsEnv, numEnv)
    let getTime ((t,_,_):Environment) = t
    let getBoolEnv ((_,boolEnv,_):Environment) = boolEnv
    let getNumEnv ((_,_,numEnv):Environment) = numEnv

