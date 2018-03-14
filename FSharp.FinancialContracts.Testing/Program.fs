namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators

module program =
     
    [<EntryPoint>]
    let main argv =
        let c = Contract.If(BoolVal("b"),20,Scale(Add(NumVal "x", NumVal "y"),One(Currency DKK)),Zero)
        let genTime = fun t -> float t

        let numGenMap = (Map.empty.Add (NumVal "x",genTime)).Add(NumVal "y",rndNumWitin 10.0 20.0)

        let boolGenMap = Map.empty.Add (BoolVal "b",fun t -> if t=15 then true else false)

        let env = generateEnvironment c numGenMap boolGenMap rndNum rndBool


        //wlet env = generateObservables t c

        
        //let propertyCheck c (property: Environment -> Transaction list -> bool) = 
            //[for t in [1..1000] do 
                 
                //let transactions = evalC env c
                //yield property env transactions
                //]
        
        //let res = propertyCheck c (fun e t -> true)

        printf "%A" env


        0 // return an integer exit code
