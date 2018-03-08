namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module program =
     
    [<EntryPoint>]
    let main argv =
        let c = Scale(Add(NumVal "x", NumVal "y"),One(DKK))

        //wlet env = generateObservables t c

        
        //let propertyCheck c (property: Environment -> Transaction list -> bool) = 
            //[for t in [1..1000] do 
                 
                //let transactions = evalC env c
                //yield property env transactions
                //]
        
        //let res = propertyCheck c (fun e t -> true)

        printf "%A" 1


        0 // return an integer exit code
