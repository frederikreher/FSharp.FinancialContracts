// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Documentation
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System.Threading

module program = 
    open FSharp.FinancialContracts.Testing.PropertyCheckers

    [<EntryPoint>]
    let main argv =
        
//        let work = fun i -> System.Threading.Thread.Sleep 1
//                            i
//        let doParallelWork n = Array.Parallel.init n work 
//        
//        let rec doSequentialWork (n:int) (i:int) =
//            if n = i then
//                ()
//            else 
//                let res = work i
//                doSequentialWork n (i+1)
//        
//        let n = 10000
//        let stopWatch = System.Diagnostics.Stopwatch.StartNew()
//        doParallelWork n
//        let t1 = stopWatch.Elapsed.TotalSeconds
//        
//        stopWatch.Restart()
//        doSequentialWork n 0 
//        let t2 = stopWatch.Elapsed.TotalSeconds
//        
//        printfn "Parallel was %A seconds sequential was %A seconds" t1 t2
        
        
//        let contract = If(BoolObs.And(BoolVal "x",BoolVal "y"),TimeObs.Const 5, One DKK, One GBP)
//        let property = atTime 1 (hasTransactions [Transaction(1.0, DKK)])
//        
//        let simpleConfiguration = {Configuration.Default with NumberOfTests = 10; FailSilently = false;}
//        let testData = PropertyCheck.CheckAndReturnDataWithConfig simpleConfiguration contract property
//        printfn "%A" testData
//        printfn "%A" testData.TestsRun
//        printfn "%A" testData.TestsFailed
//        printfn "%A" testData.AccessLog
            
        EvaluationPerfomance.Run ()
        //GeneratorPerformance.Run
        //PropertyCheckerPerformance.Run      
        
        0 // return an integer exit code
