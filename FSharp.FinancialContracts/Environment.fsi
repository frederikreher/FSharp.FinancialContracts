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

    val evalBoolObs : BoolObs -> Map<BoolObs, bool> -> Map<NumberObs, float> -> bool
    val evalNumberObs : NumberObs -> Map<NumberObs, float> -> Map<BoolObs, bool> -> float
    
    type Time = int

    type Environment = Time * Map<BoolObs, bool> * Map<NumberObs, float>

    val increaseTime : Environment -> Environment
    val getTime : Environment -> Time
    val getBoolEnv : Environment -> Map<BoolObs, bool>
    val getNumEnv : Environment -> Map<NumberObs, float>
