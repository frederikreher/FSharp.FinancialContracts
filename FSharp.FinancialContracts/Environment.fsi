namespace FSharp.FinancialContracts

module Environment =
    /// <summary> Type representing time. </summary>
    type Time = int

    /// <summary> Environment contains the value of observables for all times and the current time. </summary>
    type Environment = Time * Map<string, bool> array * Map<string, float> array
    
    val (|+)  : Environment -> Time -> Environment
    
    /// <summary> Observable of type boolean. </summary>
    type BoolObs =
        | BoolVal of string
        | Bool of bool
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan of NumberObs * NumberObs
        | Not of BoolObs
    /// <summary> Observable of type float. </summary>
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs
        | Average of NumberObs * Time
        
    /// <summary> Get the current time of an Environment. </summary>
    /// <param name="env"> The Environment to get the time of. </param>
    /// <returns> The current time of the Environment. </returns>
    val getTime: env:Environment -> Time
    
    /// <summary> Get the map of boolean observables in an Environment at a specific point in time. </summary>
    /// <param name="t"> The time to get the map at. </param>
    /// <param name="env"> The Environment to get the map of observables from. </param>
    /// <returns> The map of boolean observables at the specific point in time. </returns>
    val getBoolEnv: env:Environment -> Map<string, bool> []
    /// <summary> Get the map of numerical observables in an Environment at a specific point in time. </summary>
    /// <param name="t"> The time to get the map at. </param>
    /// <param name="env"> The Environment to get the map of observables from. </param>
    /// <returns> The map of numerical observables at the specific point in time. </returns>
    val getNumEnv : Environment -> Map<string, float> []
    
    /// <summary> Add a boolean observable to the map of boolean observables. </summary>
    /// <param name="(boolObs, bool)"> The tuple of a boolean observables and a boolean value, to add to the map. </param>
    /// <param name="boolEnv"> The map of boolean observables to extend. </param>
    /// <returns> The updated map of boolean observables. </returns>
    val addBoolObs : BoolObs * bool -> Map<string, bool> -> Map<string, bool>
    
    /// <summary> Add a float observable to the map of numerical observables. </summary>
    /// <param name="(numObs, value)"> The tuple of a numerical observables and a float value, to add to the map. </param>
    /// <param name="numEnv"> The map of numerical observables to extend. </param>
    /// <returns> The updated map of numerical observables. </returns>
    val addNumObs : NumberObs * float -> Map<string, float> -> Map<string, float>

    /// <summary> Evaluation of boolean observable, returns a boolean value. </summary>
    /// <param name="obs"> The observable to evaluate. </param>
    /// <param name="t"> Determines the point in time where the value of BoolVal objects will be looked up. </param>
    /// <param name="env"> Environment for boolean values, used to look up BoolVal objects. </param>
    /// <returns> A boolean value representing the value of the evaluated observable. </returns>
    val evalBoolObs : BoolObs -> Environment -> bool
    /// <summary> Evaluation of boolean observable, returns a boolean value. </summary>
    /// <param name="obs"> The observable to evaluate. </param>
    /// <param name="t"> Determines the point in time where the value of NumVal objects will be looked up. </param>
    /// <param name="env"> Environment for numerical values, used to look up NumVal objects. </param>
    /// <returns> A float value representing the value of the evaluated observable. </returns>
    val evalNumberObs : NumberObs -> Environment -> float
    
    /// <summary> Identifies the observables, that the provided boolean observable depends on. </summary>
    /// <param name="obs"> The observable to find dependent observables for. </param>
    /// <param name="boolAcc"> Accumulator for dependent boolean observables. </param>
    /// <param name="numAcc"> Accumulator for dependent numerical observables. </param>
    /// <returns> 
    /// A tuple containing two list, one for all dependent boolean observables
    /// and one for dependent numerical observables.
    /// </returns>
    val boolObs : BoolObs -> BoolObs list -> NumberObs list -> BoolObs list * NumberObs list 
    /// <summary> Identifies the observables, that the provided numerical observable depends on. </summary>
    /// <param name="obs"> The observable to find dependent observables for. </param>
    /// <param name="boolAcc"> Accumulator for dependent boolean observables. </param>
    /// <param name="numAcc"> Accumulator for dependent numerical observables. </param>
    /// <returns>
    /// A tuple containing two list, one for all dependent boolean observables
    /// and one for dependent numerical observables.
    /// </returns>
    val numberObs : NumberObs -> BoolObs list -> NumberObs list -> BoolObs list * NumberObs list 
