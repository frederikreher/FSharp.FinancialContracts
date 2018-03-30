namespace FSharp.FinancialContracts.Testing

open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module TestRunners =

    type TestResult = Contract -> Property -> bool * Environment * TransactionResults

    let Run = 
        fun c (p: Property) -> 
            let env = EnvironmentGenerators.WithDefaultGenerators c
            let tsr = evalC env c
            (p env tsr, env, tsr) 

    let nRunner n =
        fun c p ->
            let res = List.fold (fun acc _ -> (Run c p)::acc) [] [1..n]
            let error = 
                List.tryFind (fun pRes -> 
                                match pRes with
                                | (success, _, _) -> not success
                             ) res
            match error with
            | Some(err) -> err
            | None -> (true, (0, [||], [||]), (0, [||]))
    
    [<Sealed>]
    type Runner = 
       static member Run : TestResult = Run
       static member RunNTimes : (int -> TestResult) = nRunner
        
