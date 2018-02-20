namespace FSharp.FinancialContracts

open System
open System.Reactive.Linq
open ContractDataTypes

module Contracts =
    let date s = Date (DateTime.Parse s)

    let one c = One c

    let scale k c = Scale(k, c)

    let konst x = rx { return x }

    let (==*) l r = rx { let! x = l
                         let! y = r
                         return x = y
                       }

    let obsTime = rx { let! x = Observable.Interval(TimeSpan.FromSeconds(1.0))
                       return Date(DateTime.Today.AddSeconds (float x)) }

    let at t = obsTime ==* (konst t)

    let cWhen t c = When (t, c)

    // zero-coupon bond
    let zcb t x k = cWhen (at t) (scale (konst x) (one k))

    let rec eval c = match c with
                     | When (t, c) -> rx { let! x = t
                                           if x then
                                                return! (eval c)  }
                     | Scale (k, c) -> rx { let! x = k
                                            let! y = eval c
                                            return x * y }
                     | One c -> match c with
                                | GBP -> rx { return Money (1.0, GBP) }
                                | USD -> rx { return Money (0.675310643, GBP) }