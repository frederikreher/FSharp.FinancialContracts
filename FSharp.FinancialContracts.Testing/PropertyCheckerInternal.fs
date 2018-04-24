﻿namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract

module PropertyCheckerInternal =   

    //Function that takes the outcome of a single propertycheck and does something with a sideeffect
    type LogFunction = int -> Contract -> Property -> Environment -> TransactionResults -> unit

    //Configuration types used to run check suites
    type Configuration = { 
        NumberOfTests        : int
        MaxFail              : int
        EnvironmentGenerator : EnvironmentGenerator
        FailSilently         : bool
        ContractEvaluator    : Environment -> Contract -> TransactionResults
        RunInParallel        : bool }
        
    type Configuration with
        static member Default = { 
            NumberOfTests        = 100
            MaxFail              = 0
            EnvironmentGenerator = EnvironmentGenerators.Default
            FailSilently         = true
            ContractEvaluator    = fun env c -> evaluateContract env c
            RunInParallel        = true }
    
    //Type used to store the results of the tests
    type TestData = { 
        Passed        : bool
        TestsRun      : int
        TestsFailed   : int
        InTime        : float
        InAverageTime : float 
        AccessLog     : (string * ObservableValue * Time) list list }
        
    type TestData with
        static member Empty = { 
            Passed = false
            TestsRun = 0
            TestsFailed = 0
            InTime = 0.0
            InAverageTime = 0.0 
            AccessLog = [[]] }
    
    //A function used to time the call of a property
    let timedCall f = 
        let stopWatch = System.Diagnostics.Stopwatch.StartNew()
        let res = f ()
        (res,stopWatch.Elapsed.TotalMilliseconds)
    
    
    
 
    
    //Function for running a suite of propertychecks according to the configuration. 
    let checkSuite (conf:Configuration) onSuccess onFail contract (prop:Property) : TestData =
        
        let rec collectTestData i output data : TestData = 
            
            if i >= conf.NumberOfTests || i >= Array.length output|| (data.TestsFailed > conf.MaxFail && not conf.FailSilently) then
                data
            else 
                let ((fullFillsProperty,timeSpent), accessLog) = output.[i]
                let timeSpentAcc = data.InTime + timeSpent
                let nData = { data with TestsRun = data.TestsRun+1; InTime = timeSpentAcc; InAverageTime = (timeSpentAcc/(float (data.TestsRun+1))); AccessLog = accessLog::data.AccessLog   }
                
                if fullFillsProperty then
                    collectTestData (i+1) output nData
                else 
                    collectTestData (i+1) output { nData with TestsFailed = data.TestsFailed+1 }               
        
        let threadId = fun () -> (System.Diagnostics.Process.GetCurrentProcess().Threads.Item 0).Id
        //Internal function used to check a single property
        let checkProp :  int -> unit -> bool = fun i () ->
                let env = conf.EnvironmentGenerator contract
                let tsr = conf.ContractEvaluator env contract
                
                let res = (prop env tsr)            
                if res then (onSuccess i contract prop env tsr)
                else (onFail i contract prop env tsr)
                res
        
        let parallelCheck =
            let output = Array.Parallel.init conf.NumberOfTests (fun i -> 
                
                (timedCall (checkProp i)),  
                getAndClearAccessLog (threadId())) 
            
            collectTestData 0 output TestData.Empty
        
        //Internal function for running the checks according to the configuration        
        let rec check (data:TestData) c =
            if c >= conf.NumberOfTests || (data.TestsFailed > conf.MaxFail && not conf.FailSilently) then 
                data 
            else 
                let (fullFillsProperty,timeSpent) = timedCall (checkProp c)           
                //printfn "Internal:  %A" (System.Diagnostics.Process.GetCurrentProcess().Threads.Item 0).Id
                let timeSpentAcc = data.InTime + timeSpent
                let nData = { data with TestsRun = data.TestsRun+1; InTime = timeSpentAcc; InAverageTime = (timeSpentAcc/(float (data.TestsRun+1))); AccessLog = (getAndClearAccessLog (threadId ()))::data.AccessLog   }
                
                if fullFillsProperty then
                    check nData (c+1) 
                else 
                    check { nData with TestsFailed = data.TestsFailed+1 } (c+1)
        
        let testRes = if conf.RunInParallel then parallelCheck else check TestData.Empty 0
        printfn "Failed tests are %A" testRes.TestsFailed
        { testRes with Passed = not (testRes.TestsFailed > conf.MaxFail) }