namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Documentation.ContractEvaluation
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Contract

module PropertyCheckerInternal =   

    type Configuration = 
        { 
            NumberOfTests        : int
            MaxFail              : int
            EnvironmentGenerator : EnvironmentGenerator
            FailSilently         : bool
            ContractEvaluator    : Environment -> Contract -> TransactionResults
        }
        
        
    type Configuration with
        static member Default = { NumberOfTests        = 100
                                  MaxFail              = 0
                                  EnvironmentGenerator = EnvironmentGenerators.Default
                                  FailSilently         = true
                                  ContractEvaluator    = fun env c -> evaluateContract env c
                                }
    
    type TestData = 
            { 
                TestsRun      : int
                TestsFailed   : int
                InTime        : float
                InAverageTime : float
            }
    
    type TestData with
            static member Empty = { TestsRun = 0
                                    TestsFailed = 0
                                    InTime = 0.0
                                    InAverageTime = 0.0 }
                
    
    type LogFunction = int -> Contract -> Property -> Environment -> TransactionResults -> unit
    
    let timedCall f = 
        let stopWatch = System.Diagnostics.Stopwatch.StartNew()
        let res = f ()
        (res,stopWatch.Elapsed.TotalMilliseconds)
     
    let checkSuite (conf:Configuration) onSuccess onFail contract (prop:Property) : TestData option =
        let checkProp : int -> unit -> bool = fun i () ->
                let env = conf.EnvironmentGenerator contract
                let tsr = conf.ContractEvaluator env contract
                
                let res = (prop env tsr)            
                if res then (onSuccess i contract prop env tsr)
                else (onFail i contract prop env tsr)
                res
                
        let rec check (data:TestData) c =
            if c >= conf.NumberOfTests || (data.TestsFailed > conf.MaxFail && not conf.FailSilently) then 
                data 
            else 
                let (fullFillsProperty,timeSpent) = timedCall (checkProp c)           
                
                let timeSpentAcc = data.InTime + timeSpent
                let nData = { data with TestsRun = data.TestsRun+1; InTime = timeSpentAcc; InAverageTime = (timeSpentAcc/(float (data.TestsRun+1)))  }
                
                if fullFillsProperty then
                    check nData (c+1) 
                else 
                    check { nData with TestsFailed = data.TestsFailed+1 } (c+1)
        
        let testRes = check TestData.Empty 0
        printfn "Failed tests are %A" testRes.TestsFailed
        if testRes.TestsFailed > conf.MaxFail then None
        else Some testRes