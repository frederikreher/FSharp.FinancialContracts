namespace FSharp.FinancialContracts

open System
open Contract
open Environment

module Contracts =

    // Zero-coupon discount bond using an observable.
    let zcbO time obs cur = Delay(time, Scale(obs, One(cur)))
    // Zero-coupon discount bond using an constant.
    let zcbC time value cur = Delay(time, Scale(Const value, One(cur)))
    