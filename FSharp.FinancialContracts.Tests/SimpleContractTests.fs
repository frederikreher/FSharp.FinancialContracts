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