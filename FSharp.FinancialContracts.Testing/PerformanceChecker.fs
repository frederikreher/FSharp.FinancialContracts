namespace FSharp.FinancialContracts.Testing
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Testing.Generators

module PerformanceChecker =   
    let checkPerformance contracts (label1,f) (label2,g) : unit = 
        let mutable i = 0
        for contract in contracts do
            let count = 100
            let stopWatch = System.Diagnostics.Stopwatch.StartNew()  
            let envList = List.init count (fun _ -> EnvironmentGenerators.Default contract)
            printfn "Generated %A environments in %f" count stopWatch.Elapsed.TotalSeconds
            
            let stopWatch1 = System.Diagnostics.Stopwatch.StartNew()  
            for env in envList do
                f env contract
            stopWatch1.Stop()
            
            let stopWatch2 = System.Diagnostics.Stopwatch.StartNew()
            for env in envList do
                g env contract
            stopWatch2.Stop()
            
            if(stopWatch1.Elapsed > stopWatch2.Elapsed) then printfn "%A was faster by %A ms on contract %A" label2 (stopWatch2.ElapsedMilliseconds-stopWatch1.ElapsedMilliseconds) i
            else printfn "%A was faster by %A ms on contract %A" label1 (stopWatch1.ElapsedMilliseconds-stopWatch2.ElapsedMilliseconds) i
            i <- i+1