namespace FSharp.FinancialContracts.Documentation
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Testing.Generators

module PerformanceChecker =  
    open FSharp.FinancialContracts.Testing.PropertyCheckers

    let n = 100
 
    let repeat n f c : unit =
        for i in [0..n-1] do 
            f c |> ignore
    
    let checkPropertyPerformance contracts = 
        let mutable i = 0
        for (count,contract,generator,property) in contracts do
                    let config = {Configuration.Default with EnvironmentGenerator = generator; NumberOfTests = count;RunInParallel = false }
                    let stopWatch = System.Diagnostics.Stopwatch.StartNew()  
                    PropertyCheck.CheckWithConfig config contract property
                    printfn "Checked property %A, %A times with different environments in %A seconds" (i+1) count stopWatch.Elapsed.TotalSeconds
                    i <- i+1
    
    let checkGeneratorPerformance contracts = 
        let mutable i = 0
        
        for (count,contract,generator) in contracts do
            let horizon = getHorizon contract
            let stopWatch = System.Diagnostics.Stopwatch.StartNew()  
            repeat count (generator) contract
            printfn "On contract %A generated %A environments of length %A in %f" (i+1) count horizon stopWatch.Elapsed.TotalSeconds
            i <- i+1
            
    let checkParallelPerformance contract property = 
        
        let config = { Configuration.Default with NumberOfTests = 4}
        
        let stopWatch = System.Diagnostics.Stopwatch.StartNew()
        let d1 = PropertyCheck.CheckAndReturnDataWithConfig {config with RunInParallel = true} contract property 
        let t1 = stopWatch.Elapsed.TotalSeconds
        stopWatch.Restart()
        
        let d2 = PropertyCheck.CheckAndReturnDataWithConfig {config with RunInParallel = false} contract property 
        let t2 = stopWatch.Elapsed.TotalSeconds
        
        printfn "Parallel evaluated in %A seconds and sequential evaluated in %A seconds. Difference is %A%%" t1 t2 ((t2/t1)*100.0)
        printfn "Parallel states in %A seconds and sequential states %A seconds." d1.InAverageTime d2.InAverageTime
        printfn "Parallel states in %A seconds and sequential states %A seconds." d1.InTime d2.InTime 
        
        
    let checkPerformance contracts (label1,f) (label2,g) : unit = 
        let mutable i = 0
        for (count,contract) in contracts do
            let stopWatch = System.Diagnostics.Stopwatch.StartNew()  
            let envList = List.init count (fun _ -> EnvironmentGenerators.Default contract)
            printfn "Generated %A environments in %f" count stopWatch.Elapsed.TotalSeconds
            
            stopWatch.Restart()
            
            for env in envList do
                repeat n (f env) contract
            
            let t1 = stopWatch.Elapsed.TotalSeconds
            stopWatch.Restart()
            
            for env in envList do
                repeat n (g env) contract
                
            let t2 = stopWatch.Elapsed.TotalSeconds
            
            printfn "On contract %A %A run in time %A and %A run in time %A difference is %A%%" (i+1) label1 t1 label2 t2 ((t2/t1)*100.0)       
            i <- i+1