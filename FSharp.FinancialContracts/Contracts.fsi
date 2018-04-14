namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract

module Contracts =

    /// <summary> Zero-coupon discount bond using an observable. </summary>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obs"> The amount in a transaction specified using an observable. </param>
    /// <param name="currency"> The currency used in an transaction. </param>
    /// <returns> ... </returns>
    val zcb: time:Time -> obs:NumberObs -> currency:Currency -> Contract
    
    /// <summary> American option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="time"> The latest time at which the contract can be evaluated. </param>
    /// <param name="contract"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an american option. </returns>
    val american: boolObs:BoolObs -> time:Time -> contract:Contract -> Contract
    
    /// <summary> European option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="contract"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an european option. </returns>
    val european: boolObs:BoolObs -> time:Time -> contract:Contract -> Contract
    
    /// <summary> European option using an observable. </summary>
    /// <param name="boolObs"> The observable that determines if contract should be evaluated. </param>
    /// <param name="numObs"> The observable to get the average of. </param>
    /// <param name="time"> The time at which the contract can be evaluated. </param>
    /// <param name="obsPeriod"> The length of the period used to determine the average of numObs. </param>
    /// <param name="contract"> The contract to include in the option. </param>
    /// <returns> A contract matching the rules of an asian option. </returns>
    val asian: boolObs:BoolObs -> numObs:NumberObs -> time:Time -> obsPeriod:Time -> contract:Contract -> Contract
