namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract

module PropertyCheckerInternal =
    /// <summary>Function that takes the outcome of a single propertycheck and likely produces a sideeffect ie. prints to outcome </summary>
    type LogFunction = int -> Contract -> Property -> Environment -> TransactionResults -> unit 
    
    /// <summary>A type containing the configuration of running a test suite </summary>
    type Configuration = { 
        /// <summary>The number of time the suite should run</summary>
        NumberOfTests        : int
        /// <summary>The number of allowed failed tests</summary>
        MaxFail              : int
        /// <summary>The generator for the environment to be used</summary>
        EnvironmentGenerator : EnvironmentGenerator
        /// <summary>Determines if the suite should run til the end if the suite fails</summary>
        FailSilently         : bool
        /// <summary>Determines which contract evaluator to use. Likely not changed.</summary>
        ContractEvaluator    : Environment -> Contract -> TransactionResults }
            
    /// <summary>A type containing the default configuration for a checksuite</summary>    
    type Configuration with
        static member Default : Configuration
    
    /// <summary>A type containing the data returned from a testsuite</summary>    
    type TestData =  { 
        /// <summary>A boolean that are true if the checks were successfull</summary>
        Passed        : bool
        /// <summary>Count of the checks run</summary>
        TestsRun      : int
        /// <summary>Count of the checks failed</summary>
        TestsFailed   : int
        /// <summary>The accumulated time that was spent on evaluating the property </summary>
        InTime        : float
        /// <summary>The time that was spent on each iteration in average </summary>
        InAverageTime : float 
        /// <summary>The list of variables that has been accessed </summary>
        AccessLog     : (string * ObservableValue) list }
        
    /// <summary> Runs a suite of checks of the property </summary>
    /// <param name="conf"> The configuration of the suite </param>
    /// <param name="onSuccess"> Function that are invoked on each successfull iteration of the suite</param>
    /// <param name="onFail"> Function that are invoked on each failed iteration of the suite</param>
    /// <param name="contract"> The contract to evaluate. </param>
    /// <param name="prop"> The property that are checked foreach evaluation of the conract </param>
    /// <returns>The resulting TestData of the testsuite</returns>
    val checkSuite : conf: Configuration -> onSuccess: LogFunction -> onFail: LogFunction -> contract: Contract -> prop: Property -> TestData
    