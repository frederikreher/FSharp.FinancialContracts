// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        for i = 0 to 10 do
            let env = Map.empty.
                        Add("DKK", 100.0).
                        Add("GBP", 10.0).
                        Add("USD", 13.0).
                        Add("CurrentTime", float(i))
            let scaleFactor = Get(USD)
            let res = evalC env (_and_ (delay 5.0 (one GBP)) (give (scale scaleFactor (one DKK))))
            printfn "%A" res
        
        0 // return an integer exit code

(* Notes from meeting with Patrick  
implement:
	observable type
	horizion function for contracts
	

hybrid between composing contracts and multi party contracts to handle horizon
*)
