namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Properties
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract

[<TestClass>]
type SimpleContractTests () =

    [<TestMethod>]
    member this.TestIfWithin () =
        let contract = If(BoolVal("x"),5,One (Currency DKK),One (Currency CNY))
        let gens = [BoolGenerators.BoolTrueAtDate 5;BoolGenerators.BoolTrueAtDate 10;BoolGenerators.BoolTrueAtDate 0;BoolGenerators.BoolTrueAtDate 3]

        for gen in gens do 
            let boolGen = (Map.empty.Add(BoolVal("x"),gen))
            let env = EnvironmentGenerators.WithCustomGenerators Map.empty boolGen contract
            //printfn "Environment is %A" env   
            printfn "Result is %A" (evalC env contract)

        Assert.IsTrue(true)

    [<TestMethod>]
    member this.TestAnd () =
        let contract = Scale(Const 4.1,And(And(And(One (Currency DKK),One (Currency DKK)),Delay(2,One (Currency GBP))),Delay(2,One (Currency USD))))

        let env = EnvironmentGenerators.WithDefaultGenerators contract
        //printfn "Environment is %A" env   
        printfn "Result of TestAnd is %A" (evalC env contract)


        Assert.IsTrue(true)
    
    [<TestMethod>]
    member this.TestAndScale () =
        let scaleObs : ValueGenerator<float> = fun t -> if t = 0 then 3.0 else if t = 2 then 4.0 else 0.0 

        let contract = And(Scale(NumVal("x"),And(And(And(One (Currency DKK),One (Currency DKK)),Delay(2,One (Currency GBP))),Scale(Const 2.0,Delay(2,One (Currency USD))))),Delay(2,One (Currency CZK)))

        let env = contract |> EnvironmentGenerators.WithCustomGenerators (Map.empty.Add(NumVal("x"),scaleObs)) Map.empty
        //printfn "Environment is %A" env   
        printfn "Result of TestAndScale is %A" (evalC env contract)


        Assert.IsTrue(true)

    [<TestMethod>]
    member this.TestGive() =
        let scaleObs : ValueGenerator<float> = fun t -> if t = 0 then 3.0 else if t = 2 then 4.0 else 0.0 

        let contract = And(Scale(NumVal("x"),And(And(And(One (Currency DKK),One (Currency DKK)),Delay(2,One (Currency GBP))),Give(Scale(Const 2.0,Delay(2,One (Currency USD)))))),Delay(2,One (Currency CZK)))

        let env = contract |> EnvironmentGenerators.WithCustomGenerators (Map.empty.Add(NumVal("x"),scaleObs)) Map.empty
        //printfn "Environment is %A" env   
        printfn "Result of TestAndScale is %A" (evalC env contract)


        Assert.IsTrue(true)