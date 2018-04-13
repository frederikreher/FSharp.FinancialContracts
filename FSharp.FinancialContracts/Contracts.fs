namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcb time obs asset = Delay(time, Scale(obs, One(asset)))
    
    // American option using an observable.
    let american boolObs time c : Contract = If(boolObs, time, c, Zero)
    
    // European option using an observable.
    let european boolObs time c : Contract = Delay(time, If(boolObs, 0, c, Zero))

    // Asian option using an observable.
    let asian boolObs numObs time obsPeriod c : Contract = Delay(time, If(boolObs, 0, Scale(Average(numObs, obsPeriod), c), Zero))
