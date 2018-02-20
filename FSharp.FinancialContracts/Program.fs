// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let res = eval (_and_ (after (date "19-02-2018") (one GBP)) (scale 100.0 (one DKK))) (date "20-02-2018")
        printfn "%A" res
        0 // return an integer exit code
