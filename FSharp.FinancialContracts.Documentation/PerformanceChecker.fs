namespace FSharp.FinancialContracts.Documentation
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Testing.Generators

module PerformanceChecker =  
    let n = 100
 
    let repeat n f c : unit =
        for i in [0..n-1] do 
            f c
            

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