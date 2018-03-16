namespace FSharp.FinancialContracts

open Environment
open Contract

module Contracts =

    /// <summary> Zero-coupon discount bond using an observable. </summary>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="asset"> The asset used in an transaction. </param>
    /// <returns> ... </returns>
    val zcb : Time -> NumberObs -> Asset -> Contract
    
    /// <summary> American option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="time"> The latest time at which the contract can be evaluated. </param>
    /// <param name="c"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an american option. </returns>
    val american : BoolObs -> Time -> Contract -> Contract
    
    /// <summary> European option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="c"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an european option. </returns>
    val european : BoolObs -> Time -> Contract -> Contract
    
    /// <summary> European option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="numObs"> The observable to get the average of. </param>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obsPeriod"> The length of the period used to determine the average of numObs. </param>
    /// <param name="c"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an asian option. </returns>
    val asian : BoolObs -> NumberObs -> Time -> Time -> Contract -> Contract
