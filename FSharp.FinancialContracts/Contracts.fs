namespace FSharp.FinancialContracts

open System
open Contract
open Environment

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcbO time obs asset = Delay(time, Scale(obs, One(asset)))
    // Zero-coupon discount bond using an constant.
    let zcbC time value asset = Delay(time, Scale(Const value, One(asset)))
    