namespace FSharp.FinancialContracts.Testing
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module PropertyCheckers =   
    
    type Configuration = 
        { 
            NumberOfTests : int
            MaxFail : int
            EnvironmentGenerator : EnvironmentGenerator
        }
    
    type Configuration with
        static member Default = { NumberOfTests        = 100
                                  MaxFail              = 0
                                  EnvironmentGenerator = EnvironmentGenerators.Default
                                }
    
    //TODO Determine this on background of chosen testrunner
    let log = fun prop env tsr  -> 
        //let s = sprintf "Property failed" prop
        let s = "Property failed"
        failwith s

    let checkProperty (conf:Configuration) c prop = 
        for t in [0..conf.NumberOfTests] do
            let env =  conf.EnvironmentGenerator c
            let tsr = evalC env c
            if prop env tsr then () else log prop env tsr
            ()
    
    [<Sealed>]
    type PropertyCheck = 
       static member Check           : (Contract -> Property -> unit) = checkProperty Configuration.Default
       static member CheckWithConfig : (Configuration -> Contract -> Property -> unit) = checkProperty