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
        let contract = And(c3, 
                           And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR)))

        let stopWatch1 = System.Diagnostics.Stopwatch.StartNew()  
        for i in [0..100] do
            let env = EnvironmentGenerators.Default contract
            evalC env contract
        stopWatch1.Stop()
        printfn "Evaluated using EvalC in %f" stopWatch1.Elapsed.TotalMilliseconds
        
        let stopWatch2 = System.Diagnostics.Stopwatch.StartNew()
        for i in [0..100] do
            let env = EnvironmentGenerators.Default contract
            ContractEvaluation.evaluateContract env contract
        stopWatch2.Stop()
        printfn "Evaluated using evaluateContract in%f" stopWatch2.Elapsed.TotalMilliseconds
        
        0 // return an integer exit code
