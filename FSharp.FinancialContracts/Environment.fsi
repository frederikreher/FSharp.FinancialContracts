namespace FSharp.FinancialContracts

module Environment =

    type BoolObs =
        | BoolVal of string
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan of NumberObs * NumberObs
        | Not of BoolObs
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs

    val evalBoolObs : BoolObs -> Map<string, bool> -> Map<string, float> -> bool
    val evalNumberObs : NumberObs -> Map<string, float> -> Map<string, bool> -> float

    val boolObs : BoolObs -> BoolObs list -> NumberObs list -> BoolObs list * NumberObs list 
    val numberObs : NumberObs -> BoolObs list -> NumberObs list -> BoolObs list * NumberObs list 

    type Time = int

    type Environment = Time * Map<string, bool> array * Map<string, float> array

    val increaseTime : Time -> Environment -> Environment
    val getTime : Environment -> Time
    val getBoolEnv : Time -> Environment -> Map<string, bool>
    val getNumEnv : Time -> Environment -> Map<string, float>
    val addBoolObs : BoolObs * bool -> Map<string, bool> -> Map<string, bool>
    val addNumObs : NumberObs * float -> Map<string, float> -> Map<string, float>
