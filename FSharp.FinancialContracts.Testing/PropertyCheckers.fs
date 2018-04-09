namespace FSharp.FinancialContracts.Testing
open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module PropertyCheckers =   
  
    //TODO Determine this on background of chosen testrunner
    let log = fun prop env tsr  -> 
        //let s = sprintf "Property failed" prop
        let s = "Property failed"
        failwith s
    
    let logSuccess : LogFunction = fun i _ _ env tsr -> printfn "Succeeded on iteration %A with tsr %A and env %A" i tsr env
    let logFailed  : LogFunction = fun i _ _ env tsr -> printfn "Failed on iteration %A with tsr %A and env %A" i tsr env
    
    let noLog  : LogFunction = fun _ _ _ _ _ -> ()
    
    let checkProperty (conf:Configuration) c prop = 
        match PropertyCheckerInternal.checkSuite conf noLog logFailed c prop with 
        | Some(res) -> 
            printfn "Succesfully satisfied property in %A ms" res.InTime
            ()
        | None -> failwith "Property failed on the check"
    
    [<Sealed>]
    type PropertyCheck = 
       static member Check           : (Contract -> Property -> unit) = checkProperty Configuration.Default
       static member CheckWithConfig : (Configuration -> Contract -> Property -> unit) = checkProperty