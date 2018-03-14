namespace FSharp.FinancialContracts

open System
open Environment
open Contract

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcbO time obs asset = Delay(time, Scale(obs, One(asset)))
    // Zero-coupon discount bond using an constant.
    let zcbC time value asset = Delay(time, Scale(Const(value), One(asset)))
    
    // American option using an observable.
    let americanO expr time obs asset : Contract = If(expr, time, Scale(obs, One(asset)), Zero)
    // American option using an constant.
    let americanC expr time value asset : Contract = If(expr, time, Scale(Const(value), One(asset)), Zero)
    
    // European option using an observable.
    let europeanO expr time obs asset : Contract = Delay(time, If(expr, 0, Scale(obs, One(asset)), Zero))
    // European option using an constant.
    let europeanC expr time value asset : Contract = Delay(time, If(expr, 0, Scale(Const(value), One(asset)), Zero))
