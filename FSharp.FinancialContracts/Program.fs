// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let observables = [(Number("DKK/GBP"), NumberValue(99.2));
                           (Number("DKK/USD"), NumberValue(80.5));
                           (Number("100"), NumberValue(100.0))]

        let mutable env: Environment = 0, (observables |> Map.ofList)
        
        let scaleFactor = Number("DKK/GBP")
        let scaleFactor2 = Number("DKK/USD")
        let scaleFactor3 = Number("100")
        let c1 = And(Delay(3,One(GBP)),Scale(scaleFactor,One(DKK)))
        let c2 = And(Delay(8, Scale(scaleFactor3, One(GBP))),Scale(scaleFactor2,One(DKK)))

        for i = 1 to 10 do
            env <- increaseTime env
            let res = c1 |> evalC env
            let res2 = c2 |> evalC env
            printfn "%A" (getTime env)
            printfn "%A" res
            printfn "%A" res2

            Thread.Sleep(2000)
        0 // return an integer exit code

(* Notes from meeting with Patrick  
implement:
	observable type
	horizion function for contracts
	

hybrid between composing contracts and multi party contracts to handle horizon
*)
