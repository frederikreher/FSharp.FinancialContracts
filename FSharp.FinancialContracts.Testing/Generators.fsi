namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Generators =
    type ValueGenerator<'a> = Time -> 'a

    val rndNum : ValueGenerator<float>
    val rndNumWitin : float -> float -> ValueGenerator<float>

    val rndBool : ValueGenerator<bool>
    val rndBoolShift : float -> ValueGenerator<bool>

    val generateEnvironment : Contract -> Map<NumberObs,ValueGenerator<float>> -> Map<BoolObs,ValueGenerator<bool>> -> ValueGenerator<float> -> ValueGenerator<bool> -> Environment

