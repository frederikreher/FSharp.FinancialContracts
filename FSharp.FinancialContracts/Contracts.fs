namespace FSharp.FinancialContracts

open System
open Contract
open Environment

module Contracts =

    // Zero-coupon discount bond
    let zcb time obs cur = Delay(time, Scale(obs, One(cur)))
    let zcbF time value cur = Delay(time, Scale(Const value, One(cur)))
    