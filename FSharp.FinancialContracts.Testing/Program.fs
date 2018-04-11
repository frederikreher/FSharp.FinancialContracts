namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Contract

module program =
    open FSharp.FinancialContracts.Documentation
     
    [<EntryPoint>]
    let main argv =
        let dkkEUR = LessThan(NumVal "DKK/EUR", Const 7.0)
        let buy100EUR = Scale(Const 100.0, One EUR)
        let buy500DKK = Scale(Const 500.0, One DKK)
        let c0 = And(If(BoolVal "x", 10, 
                        Give(ScaleNow(Mult(NumVal "x", Const 100.0), One DKK)), 
                        Give(Scale(Sub(NumVal "y", NumVal "x"), One EUR))),
                     One CNY)
        let c1 = Delay(100, c0)
        let c2 = If(GreaterThan(NumVal "DKK/EUR", Const 7.5), 60, buy500DKK, c1)
        let c3 = If(dkkEUR, 30, buy100EUR, c2)
        let contract = And(And(c3, 
                            And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR))),
                           Delay(365, One DKK))
                           

        let contract = Delay(365, One DKK)

        let run = 100

        let envList = List.init run (fun _ -> EnvironmentGenerators.Default contract)

        let stopWatch1 = System.Diagnostics.Stopwatch.StartNew()  

        for env in envList do
            evalC env contract |> ignore
        stopWatch1.Stop()
        printfn "Evaluated using EvalC in %f" stopWatch1.Elapsed.TotalMilliseconds
        
        let stopWatch2 = System.Diagnostics.Stopwatch.StartNew()
        for env in envList do
            ContractEvaluation.evaluateContract env contract |> ignore
        stopWatch2.Stop()
        printfn "Evaluated using evaluateContract in %f" stopWatch2.Elapsed.TotalMilliseconds
        
        0 // return an integer exit code
