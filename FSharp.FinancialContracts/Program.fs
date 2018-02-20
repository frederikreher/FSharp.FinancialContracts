// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

//open System
//open Contract

open System
open ContractDataTypes
open Contracts

open System.Reactive.Linq

module program = 

    [<EntryPoint>]
    let main argv =
        //let res = eval (_and_ (after (date "19-02-2018") (one GBP)) (give (scale 100.0 (one DKK)))) (date "20-02-2018")
        //printfn "%A" res
        //0 // return an integer exit code
        
        let t1 = Date(DateTime.Today.AddSeconds(10.0))

        // zero-coupon bond
        let c1 = zcb t1 10.0 USD

        let ret = eval c1
        //printfn "%A" ret
        
        let everySecond = Observable.Interval(TimeSpan.FromSeconds(1.0))//.Subscribe(fun a -> printfn "%A - %A" a DateTime.Now)
        ret.Subscribe(fun a -> printfn "%A - %A" a DateTime.Now)
        
        while true do
            ()
        0
