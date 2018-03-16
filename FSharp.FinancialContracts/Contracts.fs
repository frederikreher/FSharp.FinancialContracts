namespace FSharp.FinancialContracts

open System
open Environment
open Contract

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcb time obs asset = Delay(time, Scale(obs, One(asset)))
    
    // American option using an observable.
    let american expr time obs asset : Contract = If(expr, time, Scale(obs, One(asset)), Zero)
    
    // European option using an observable.
    let european expr time obs asset : Contract = Delay(time, If(expr, 0, Scale(obs, One(asset)), Zero))
