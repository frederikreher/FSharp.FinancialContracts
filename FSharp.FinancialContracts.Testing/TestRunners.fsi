namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract

module TestRunners =

    type TestResult = Contract -> Property -> bool * Environment * TransactionResults

    [<Sealed>]
    type Runner = 
       static member Run        : TestResult
       static member RunNTimes  : (int -> TestResult)
