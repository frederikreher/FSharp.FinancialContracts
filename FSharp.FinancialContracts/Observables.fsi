namespace FSharp.FinancialContracts
open FSharp.FinancialContracts.Time

module Observables =
        
    /// <summary> Observable of type boolean. </summary>
    type BoolObs =
        | BoolVal     of string
        | Bool        of bool
        | And         of BoolObs   * BoolObs
        | Or          of BoolObs   * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan    of NumberObs * NumberObs
        | Equal       of NumberObs * NumberObs
        | Not         of BoolObs
    /// <summary> Observable of type float. </summary>
    and NumberObs =
        | NumVal  of string
        | Const   of float
        | Add     of NumberObs * NumberObs
        | Sub     of NumberObs * NumberObs
        | Mult    of NumberObs * NumberObs
        | If      of BoolObs   * NumberObs * NumberObs
        | Average of NumberObs * Time

    /// <summary> Wrapper for the observables of BoolVal and NumVal used to simplify the environment. </summary>
    type ObservableValue = 
        | BoolValue   of bool
        | NumberValue of float

    /// <summary> Identifies the observables, that the provided boolean observable depends on. </summary>
    /// <param name="obs"> The observable to find dependent observables for. </param>
    /// <param name="boolAcc"> Accumulator for dependent boolean observables. </param>
    /// <param name="numAcc"> Accumulator for dependent numerical observables. </param>
    /// <returns> 
    /// A tuple containing two list, one for all dependent boolean observables
    /// and one for dependent numerical observables.
    /// </returns>
    val boolObs: obs:BoolObs -> boolAcc:BoolObs list -> numAcc:NumberObs list -> BoolObs list * NumberObs list
    
    /// <summary> Identifies the observables, that the provided numerical observable depends on. </summary>
    /// <param name="obs"> The observable to find dependent observables for. </param>
    /// <param name="boolAcc"> Accumulator for dependent boolean observables. </param>
    /// <param name="numAcc"> Accumulator for dependent numerical observables. </param>
    /// <returns>
    /// A tuple containing two list, one for all dependent boolean observables
    /// and one for dependent numerical observables.
    /// </returns>
    val numberObs: obs:NumberObs -> boolAcc:BoolObs list -> numAcc:NumberObs list -> BoolObs list * NumberObs list 
