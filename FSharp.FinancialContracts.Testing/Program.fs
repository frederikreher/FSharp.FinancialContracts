namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Contract

module program =
     
    [<EntryPoint>]
    let main argv =
        let c = If(BoolVal("b"),10,Scale(Add(NumVal "x", NumVal "y"),One(DKK)),One(USD))

        let numGenMap = Map.empty
                            .Add(NumVal "x", fun t -> float t)
                            .Add(NumVal "y", NumericGenerators.Default)

        let boolGenMap = Map.empty
                            .Add(BoolVal "b", fun t -> t = 10)

        let env = EnvironmentGenerators.Default c
        let customEnv = EnvironmentGenerators.WithCustomGenerators numGenMap boolGenMap c

        printfn "%A" customEnv
        let trans = evalC customEnv c
        let t10 = trans.[0];

        //let env = generateObservables t c

        
        //let propertyCheck c (property: Environment -> Transaction list -> bool) = 
            //[for t in [1..1000] do 
                 
                //let transactions = evalC env c
                //yield property env transactions
                //]
        
        //let res = propertyCheck c (fun e t -> true)

        printf "%A" trans
        printfn "Element 0 isNull %A" t10


        0 // return an integer exit code
