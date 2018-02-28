// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let observables = [(Number("GBP/DKK"), NumberValue(getExchangeRate (GBP, DKK)));
                           (Number("USD/DKK"), NumberValue(getExchangeRate (USD, DKK)));
                           (Number("100"), NumberValue(100.0));
                           (Bool("true"), BoolValue(true));
                           (Bool("false"), BoolValue(false))]

        let mutable env: Environment = 0, (observables |> Map.ofList)
        
        let scaleFactor1 = Number("GBP/DKK")
        let scaleFactor2 = Number("USD/DKK")
        let scaleFactor3 = Number("100")
        let c1 = And(One(GBP), Give(Delay(3,Scale(scaleFactor1,One(DKK)))))
        let c2 = And(Scale(scaleFactor3, One(GBP)), Give(Delay(4,Scale(scaleFactor3, Scale(scaleFactor1,One(DKK))))))
        let c3 = And(One(USD), Give(Delay(3,Scale(scaleFactor2,One(DKK)))))
        let c4 = And(Scale(scaleFactor3, One(USD)), Give(Delay(4,Scale(scaleFactor3, Scale(scaleFactor2,One(DKK))))))

        let boolT = Bool("true")
        let boolF = Bool("false")
        let ifTest = If(boolT, 0, c1, c3)
        
        printfn "%A" (evalC env ifTest)

        //printfn "%A" (getExchangeRate (USD, DKK))
        //printfn "%A" (getExchangeRate (DKK, USD))

        //printfn "%A" (evalC env (Zero(10.0, USD)))

        (*for i = 1 to 5 do
            env <- increaseTime env
            let res = c1 |> evalC env
            let res2 = c2 |> evalC env
            let res3 = c3 |> evalC env
            let res4 = c4 |> evalC env
            printfn "%A" (getTime env)
            printfn "Pounds:"
            printfn "%A" res
            printfn "%A" res2
            printfn "Dollars:"
            printfn "%A" res3
            printfn "%A" res4

            Thread.Sleep(1000)*)
        0 // return an integer exit code

(* Notes from meeting with Patrick  
implement:
	observable type
	horizion function for contracts
	

hybrid between composing contracts and multi party contracts to handle horizon
*)
