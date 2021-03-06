﻿namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcb time obs currency = Delay(time, Scale(obs, One(currency)))
    
    // American option using an observable.
    let american boolObs time contract : Contract = If(boolObs, time, contract, Zero)
    
    // European option using an observable.
    let european boolObs time contract : Contract = Delay(time, If(boolObs, TimeObs.Const 0, contract, Zero))

    // Asian option using an observable.
    let asian boolObs numObs time obsPeriod contract : Contract = Delay(time, If(boolObs, TimeObs.Const 0, ScaleNow(Average(numObs, obsPeriod), contract), Zero))
    