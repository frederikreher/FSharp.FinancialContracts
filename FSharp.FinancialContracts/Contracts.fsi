namespace FSharp.FinancialContracts

open Environment
open Contract

module Contracts =

    /// <summary> Zero-coupon discount bond using an observable. </summary>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="asset"> The asset used in an transaction. </param>
    /// <returns>  </returns>
    val zcb : Time -> NumberObs -> Asset -> Contract
    
    /// <summary> American option using an observable. </summary>
    /// <param name="expr"> The expr that determines which contract to evaluate. </param>
    /// <param name="time"> The latest time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="asset"> The asset used in an transaction. </param>
    /// <returns> The currency used in an transaction. </returns>
    val american : BoolObs -> Time -> NumberObs -> Asset -> Contract
    
    /// <summary> European option using an observable. </summary>
    /// <param name="expr"> The expr that determines which contract to evaluate. </param>
    /// <param name="time"> The latest time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="asset"> The asset used in an transaction. </param>
    /// <returns> The currency used in an transaction. </returns>
    val european : BoolObs -> Time -> NumberObs -> Asset -> Contract
