namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract

module PropertyCheckerInternal =

    type Configuration = 
        { 
            NumberOfTests        : int
            MaxFail              : int
            EnvironmentGenerator : EnvironmentGenerator
            FailSilently         : bool
        }
            
        
    type Configuration with
        static member Default : Configuration
    
    type TestData = 
        { 
            TestsRun      : int
            TestsFailed   : int
            InTime        : float
            InAverageTime : float
        }
        
    type LogFunction = int -> Contract -> Property -> Environment -> TransactionResults -> unit
    
    val checkSuite : Configuration -> LogFunction -> LogFunction -> Contract -> Property -> TestData option
    