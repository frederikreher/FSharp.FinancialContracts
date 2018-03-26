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
type TestClass () =

    [<TestMethod>]
    member this.TestTestingFramework () =
        let list = [Transaction(2.0,(DKK))]

        let sumProperty = sumIs 2.0 (DKK)
        let countProperty = countIs 2

        Assert.IsTrue(true)

    [<TestMethod>]
    member this.EuropeanOption () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = And(scale1,scale2)
        let europ = european (Bool true) 20 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty europ
        
        let transResults = evalC env europ 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay20 = atTime 20 p
        
        Assert.IsTrue(atDay20 env transResults )

    [<TestMethod>]
    member this.AmericanOption () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = And(scale1,scale2)
        let amer = american (Bool true) 20 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty amer
        
        let transResults = evalC env amer 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay0 = atTime 0 p
        
        Assert.IsTrue(atDay0 env transResults)
        
    [<TestMethod>]
    member this.AmericanOptionWithRandomEnvironment () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = And(scale1,scale2)
        let amer = american (BoolVal "test") 20 c

        let env = EnvironmentGenerators.WithDefaultGenerators amer
        
        let transResults = evalC env amer 

        let allTransactions = fun _ -> true
        
        let checkBoolVal = fun en _ -> evalBoolObs (BoolVal "test") en
        let boolValAt0 = atTime 0 checkBoolVal
        let checkTransactions = countOf allTransactions (=) 2
        let transactionsAt0 = atTime 0 checkTransactions
        let test = boolValAt0 &|& transactionsAt0
        let atDay0 = test =|> boolValAt0
        
        Assert.IsTrue(atDay0 env transResults)

    [<TestMethod>]
    member this.AsianOption () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = And(scale1,scale2)
        let asi = asian (Bool true) (NumVal("x")) 20 5 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty asi
        
        let transResults = evalC env asi 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay20 = atTime 20 p
        
        Assert.IsTrue(atDay20 env transResults)
