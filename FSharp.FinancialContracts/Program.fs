// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        printfn "Give me money"
        //eval (_and_ (after (date "20-02-2018") test) test2) (date "20-02-2018")
        0 // return an integer exit code
