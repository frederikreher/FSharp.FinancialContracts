namespace FSharp.FinancialContracts.Testing

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
            NumberOfTests        = 1000
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
        (res,stopWatch.Elapsed.TotalSeconds)

    //Function for running a suite of propertychecks according to the configuration. 
    let checkSuite (conf:Configuration) onSuccess onFail contract (prop:Property) : TestData =
        
        //Collect testdata from parallel Invokation.
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
        
        //Internal function used to check a single property
        let checkProp f i : unit -> bool = fun () ->
                let (t,obs,_) = conf.EnvironmentGenerator contract
                let env = (t,obs,f)
                //printfn "Env is %A" env
                let tsr = conf.ContractEvaluator env contract
                
                let res = (prop env tsr)            
                if res then (onSuccess i contract prop env tsr)
                else (onFail i contract prop env tsr)
                res
        
        //Internal function for running the checks according to the configuration in parallel
        let checkParallel : unit -> TestData = fun () ->
            //printfn "parallelcheck"
            let output = Array.Parallel.init conf.NumberOfTests (fun i -> 
                let mutable accessLog = []
                let updateLog = fun s obsval t ->
                    accessLog <- (s,obsval,t)::accessLog
                    ()
                
                let (fullFillsProperty,timeSpent) = timedCall (checkProp updateLog i)  
                
                ((fullFillsProperty,timeSpent),accessLog))
            
            collectTestData 0 output TestData.Empty
        
        //Internal function for running the checks according to the configuration linearly        
        let rec check data c : TestData =
            //printfn "simplecheck"
            if c >= conf.NumberOfTests || (data.TestsFailed > conf.MaxFail && not conf.FailSilently) then 
                data 
            else 
                let mutable accessLog = []
                let updateLog = fun s obsval t ->
                    accessLog <- (s,obsval,t)::accessLog
                    ()
                    
                let (fullFillsProperty,timeSpent) = timedCall (checkProp updateLog c)           
                
                let timeSpentAcc = data.InTime + timeSpent
                let nData = { data with TestsRun = data.TestsRun+1; InTime = timeSpentAcc; InAverageTime = (timeSpentAcc/(float (data.TestsRun+1))); AccessLog = accessLog::data.AccessLog  }
                
                if fullFillsProperty then
                    check nData (c+1) 
                else 
                    check { nData with TestsFailed = data.TestsFailed+1 } (c+1)        
        
        let testRes = if conf.RunInParallel 
                        then checkParallel ()
                        else check TestData.Empty 0
        
        //printfn "Failed tests are %A" testRes.TestsFailed
        { testRes with Passed = not (testRes.TestsFailed > conf.MaxFail) }