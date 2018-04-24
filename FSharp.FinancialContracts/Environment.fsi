namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables

module Environment =
    
    /// <summary> An Environment contains the current time and the value of observables for all times. </summary>
    type Environment = Time * Map<string,ObservableValue> array
    
    /// <summary> Increases the current time of an Environment. </summary>
    /// <param name="env"> The Environment to increase the time of. </param>
    /// <param name="t"> The amount of time the current time should be increased by. </param>
    /// <returns> An Environment with the current time increased. </returns>
    val (|+): env:Environment -> t:Time -> Environment
    
    /// <summary> Get the current time of an Environment. </summary>
    /// <param name="env"> The Environment to get the time of. </param>
    /// <returns> The current time of the Environment. </returns>
    val getTime: env:Environment -> Time
    
    /// <summary> Adds a observable to the map of observables. </summary>
    /// <param name="kv"> 
    /// The tuple of the name of the observable and its value which are to be added to the map.
    /// </param>
    /// <param name="observables"> The map of observables to extend. </param>
    /// <returns> The updated map of observables. </returns>   
    val addObservable: kv:(string * ObservableValue) -> observables:Map<string, ObservableValue> -> Map<string, ObservableValue>
    
    /// <summary> Gets the value of an boolean observable. </summary>
    /// <param name="obs"> The observable to evaluate. </param>
    /// <param name="env"> The environment used to look up the value of observables. </param>
    /// <returns> A boolean value representing the current value of the evaluated observable. </returns>
    val evalBoolObs: obs:BoolObs -> env:Environment -> bool
    
    /// <summary> Evaluation of Time observable, returns a Time value. </summary>
    /// <param name="obs"> The observable to evaluate. </param>
    /// <param name="env"> The environment used to look up the value of observables. </param>
    /// <returns> A Time value representing the value of the evaluated observable. </returns>
    val evalTimeObs: obs:TimeObs -> env:Environment -> Time
    
    /// <summary> Evaluation of numerical observable, returns a float value. </summary>
    /// <param name="obs"> The observable to evaluate. </param>
    /// <param name="env"> The environment used to look up the value of observables. </param>
    /// <returns> A float value representing the value of the evaluated observable. </returns>
    val evalNumberObs: obs:NumberObs -> env:Environment -> float