namespace FSharp.FinancialContracts

open System
open System.Reactive.Linq

module ContractDataTypes = 
    type Date = Date of DateTime
    type Days = Days of TimeSpan

    type Currency = GBP | USD

    type Money = Money of float * Currency
        with static member (*) (k, Money (v, c)) = Money (k * v, c)

    type Contract = | One of Currency
                    | Scale of (IObservable<double> * Contract)
                    | When of (IObservable<bool> * Contract)

    type RxBuilder() =
        member this.Bind(m:IObservable<'a>, f:'a -> IObservable<'b>) =
            Observable.SelectMany(m, new Func<'a, IObservable<'b>> (f))
        member this.Return x = Observable.Return x
        member this.ReturnFrom x = x
        member this.Zero() = Observable.Empty()

    let rx = RxBuilder()