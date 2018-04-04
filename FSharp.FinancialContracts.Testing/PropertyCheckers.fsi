namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property

module PropertyCheckers =
    type Configuration = 
            { 
                NumberOfTests : int
                MaxFail : int
                EnvironmentGenerator : EnvironmentGenerator
            }
            
    type Configuration with
            static member Default : Configuration
            
    [<Sealed>]
    type PropertyCheck = 
       static member Check       : (Contract  -> Property -> unit)
       static member CheckWithConfig : (Configuration -> Contract -> Property -> unit)