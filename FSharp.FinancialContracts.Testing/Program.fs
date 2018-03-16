namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Contract

module program =
     
    [<EntryPoint>]
    let main argv =
        let c = If(BoolVal("b"),20,Scale(Add(NumVal "x", NumVal "y"),One(Currency DKK)),Zero)

        let numGenMap = Map.empty
                            .Add(NumVal "x", fun t -> float t)
                            .Add(NumVal "y", NumericGenerators.Default)

        let boolGenMap = Map.empty
                            .Add(BoolVal "b", BoolGenerators.Default)

        let env = EnvironmentGenerators.Default c
        let customEnv = EnvironmentGenerators.WithCustomGenerators numGenMap boolGenMap c

        printfn "%A" env
        let trans = evalC env c

        //let env = generateObservables t c

        
        //let propertyCheck c (property: Environment -> Transaction list -> bool) = 
            //[for t in [1..1000] do 
                 
                //let transactions = evalC env c
                //yield property env transactions
                //]
        
        //let res = propertyCheck c (fun e t -> true)

        printf "%A" trans


        0 // return an integer exit code
