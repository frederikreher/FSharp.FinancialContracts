namespace FSharp.FinancialContracts

open Environment
open Contract

module Contracts =

    /// <summary> Zero-coupon discount bond using an observable. </summary>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="cur"> The currency used in an transaction. </param>
    /// <returns>  </returns>
    val zcbO : Time -> NumberObs -> Currency -> Contract
    /// <summary> Zero-coupon discount bond using an constant. </summary>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="value"> The amount in a transaction specified using an constant. </param>
    /// <param name="cur">  </param>
    /// <returns> The currency used in an transaction. </returns>
    val zcbC : Time -> float -> Currency -> Contract