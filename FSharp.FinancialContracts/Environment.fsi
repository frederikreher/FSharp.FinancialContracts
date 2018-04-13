namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Environment =
    
    /// <summary> Environment contains the value of observables for all times and the current time. </summary>
    type Environment = Time * Map<string,ObservableValue> array
    
    val (|+)  : Environment -> Time -> Environment
    
    /// <summary> Get the current time of an Environment. </summary>
    /// <param name="env"> The Environment to get the time of. </param>
    /// <returns> The current time of the Environment. </returns>
    val getTime: env:Environment -> Time
    
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
    
    val addObservable : (string*ObservableValue) -> Map<string, ObservableValue> -> Map<string, ObservableValue>
    
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