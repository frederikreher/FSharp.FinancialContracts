namespace FSharp.FinancialContracts

open Environment
open Contract

module Contracts =

    val zcb : Time -> NumberObs -> Currency -> Contract
    val zcbF : Time -> float -> Currency -> Contract