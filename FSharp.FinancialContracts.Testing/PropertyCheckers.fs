namespace FSharp.FinancialContracts.Testing
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module PropertyCheckers =   
    let logSuccess : LogFunction = fun i _ _ env tsr -> printfn "Succeeded on iteration %A with tsr %A and env %A" i tsr env
    let logFailed  : LogFunction = fun i _ _ env tsr -> printfn "Failed on iteration %A with tsr %A and env %A" i tsr env
    
    let noLog  : LogFunction = fun _ _ _ _ _ -> ()
    
    let containData data = if data.Passed then ()
                           else failwith "Property wasn't successfully satisfied"
    let returnData data = data
    
    let checkProperty f (conf:Configuration) c prop = 
        let data = PropertyCheckerInternal.checkSuite conf noLog noLog c prop
        f data
            
    
    [<Sealed>]
    type PropertyCheck = 
       static member Check                        : (Contract -> Property -> unit) = checkProperty containData Configuration.Default 
       static member CheckWithConfig              : (Configuration -> Contract -> Property -> unit) = checkProperty containData
       static member CheckAndReturnData           : (Contract -> Property -> TestData) = checkProperty returnData Configuration.Default 
       static member CheckAndReturnDataWithConfig : (Configuration -> Contract -> Property -> TestData) = checkProperty returnData