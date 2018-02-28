﻿// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Contract

module program = 

    [<EntryPoint>]
    let main argv =
        let boolObservables = [(BoolVal("true"), true);
                               (BoolVal("false"), false)]
        let numObservables = [(NumVal("GBP/DKK"), getExchangeRate(GBP, DKK));
                              (NumVal("USD/DKK"), getExchangeRate(USD, DKK));
                              (NumVal("100"), 100.0)]

        let mutable env: Environment = 0, (boolObservables |> Map.ofList), (numObservables |> Map.ofList)
        
        let scaleFactor1 = NumVal("GBP/DKK")
        let scaleFactor2 = NumVal("USD/DKK")
        let scaleFactor3 = NumVal("100")
        let c1 = And(One(GBP), Give(Delay(3,Scale(scaleFactor1,One(DKK)))))
        let c2 = And(Scale(scaleFactor3, One(GBP)), Give(Delay(4,Scale(scaleFactor3, Scale(scaleFactor1,One(DKK))))))
        let c3 = And(One(USD), Give(Delay(3,Scale(scaleFactor2,One(DKK)))))
        let c4 = And(Scale(scaleFactor3, One(USD)), Give(Delay(4,Scale(scaleFactor3, Scale(scaleFactor2,One(DKK))))))

        //let boolT = Bool("true")
        //let boolF = Bool("false")
        //let ifTest = If(boolF, 0, c1, c3)
        //printfn "%A" (evalC env ifTest)

        //printfn "%A" (getExchangeRate (USD, DKK))
        //printfn "%A" (getExchangeRate (DKK, USD))

        //printfn "%A" (evalC env (Zero(10.0, USD)))

        for i = 1 to 5 do
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

            Thread.Sleep(1000)
        0 // return an integer exit code

(* Notes from meeting with Patrick  
implement:
	observable type
	horizion function for contracts
	

hybrid between composing contracts and multi party contracts to handle horizon
*)
