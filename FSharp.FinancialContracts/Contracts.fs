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
    let americanO time obs asset : Contract = If(Bool(true), time, Scale(obs, One(asset)), Zero)
    // American option using an constant.
    let americanC time value asset : Contract = If(Bool(true), time, Scale(Const(value), One(asset)), Zero)
    
    // European option using an observable.
    //let europeanO time obs asset : Contract = If(Bool(true), time, Scale(obs, One(asset)), Zero)
    // European option using an constant.
    //let europeanC time value asset : Contract = If(Bool(true), time, Scale(Const(value), One(asset)), Zero)
