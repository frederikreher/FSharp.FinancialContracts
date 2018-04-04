namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module PropertyCheckers =

    //TODO Determine this on background of chosen testrunner
    let assertion = fun b -> if b then () else failwith "Property failed"

    let checkProperty n (envGen : EnvironmentGenerator) c prop = 
        for t in [0..n] do
            let env =  envGen c
            let tsr = evalC env c
            assertion (prop env tsr)
            ()
    
    [<Sealed>]
    type PropertyCheck = 
       static member Check : (Contract -> Property -> unit) = checkProperty 100 EnvironmentGenerators.Default
       static member CheckNTimes : (int -> EnvironmentGenerator -> Contract -> Property -> unit) = checkProperty