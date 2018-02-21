// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

//open System
//open Contract

open System
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let env = CurrentTime(DateTime.Now)
        let scaleFactor = Get(DKK, GBP)
        let res = evalC env (_and_ (delay 5 (one GBP)) (give (scale scaleFactor (one DKK))))
        printfn "%A" res
        0 // return an integer exit code

