// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

//open System
//open Contract

open System
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let observables = [(Number("DKK/GBP"),NumberValue(99.2))]

        let env: Environment = 2, (observables |> Map.ofList)


        let scaleFactor = Number("DKK/GBP")
        let c1 = And(Delay(2,One(GBP)),Scale(scaleFactor,One(DKK)))

        let res = c1 |> evalC env

        printfn "%A" res
        0 // return an integer exit code

(* Notes from meeting with Patrick  
implement:
	observable type
	horizion function for contracts
	

hybrid between composing contracts and multi party contracts to handle horizon
*)
