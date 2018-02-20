namespace FSharp.FinancialContracts

open System
open ContractDataTypes

module Contracts =
    val date: String -> Date

    val one : Currency -> Contract

    val scale : IObservable<double> -> Contract -> Contract

    val konst : 'a -> IObservable<'a>

    val (==*) : IObservable<'a> -> IObservable<'a> -> IObservable<bool> when 'a : equality

    val obsTime : IObservable<Date>

    val at: Date -> IObservable<bool>

    val cWhen: IObservable<bool> -> Contract -> Contract

    // zero-coupon bond
    val zcb : Date -> Double -> Currency -> Contract

    val eval: Contract -> IObservable<Money>
