// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Documentation
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System.Threading

module program = 
    open FSharp.FinancialContracts.Testing.PropertyCheckers

    [<EntryPoint>]
    let main argv =
        
        //EvaluationPerfomance.Run ()
       // GeneratorPerformance.Run ()
        PropertyCheckerPerformance.Run () 
            
        0 // return an integer exit code
    