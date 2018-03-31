namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Environment
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Testing.Generators

module PropertyCheckers =
    open Generators
    open Generators

    //TODO Determine this on background of chosen testrunner
    let ass = Assert.IsTrue

    let checkProperty n (envGen : EnvironmentGenerator) c prop = 
        for t in [0..n] do
            let env =  envGen c
            let tsr = evalC env c
            ass (prop env tsr)
            ()
    
    [<Sealed>]
    type PropertyCheck = 
       static member Check : (Contract -> Property -> unit) = checkProperty 100 EnvironmentGenerators.Default
       static member CheckNTimes : (int -> EnvironmentGenerator -> Contract -> Property -> unit) = checkProperty