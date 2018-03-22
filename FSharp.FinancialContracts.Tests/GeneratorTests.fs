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
type GeneratorTests () =

    [<TestMethod>]
    member this.TestNumberWithinRange () =
        let scale1 = Scale(NumVal("x"),One(Currency CNY))
        let scale2 = Scale(NumVal("y"),One(Currency DKK))
        let c = Delay(200,And(scale1,scale2))

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)
                            .Add(NumVal "y", NumericGenerators.RandomNumberWithinRange 40.0 60.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty c
        printfn "env is %A" env
        
        let xProperty = (satisfyNumObs (NumVal "x") (>=) 20.0) &|& (satisfyNumObs (NumVal "x") (<=) 30.0)
        let yProperty = (satisfyNumObs (NumVal "y") (>=) 40.0) &|& (satisfyNumObs (NumVal "y") (<=) 60.0)
        let p = forAllTimes xProperty &|& yProperty
        
        Assert.IsTrue(p env Array.empty)

    [<TestMethod>]
    member this.AmericanOption () =
        let scale1 = Scale(NumVal("x"),One(Currency CNY))
        let scale2 = Scale(NumVal("y"),One(Currency DKK))
        let c = And(scale1,scale2)
        let amer = american (Bool true) 20 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty amer
        
        let trans = evalC env amer 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay0 = atTime 0 p
        
        Assert.IsTrue(atDay0 env trans)
        
    [<TestMethod>]
    member this.AmericanOptionWithRandomEnvironment () =
        let scale1 = Scale(NumVal("x"),One(Currency CNY))
        let scale2 = Scale(NumVal("y"),One(Currency DKK))
        let c = And(scale1,scale2)
        let amer = american (BoolVal "test") 20 c

        let env = EnvironmentGenerators.WithDefaultGenerators amer
        
        let trans = evalC env amer 

        let allTransactions = fun _ -> true
        
        let checkBoolVal = fun en _ -> evalBoolObs (BoolVal "test") en
        let boolValAt0 = atTime 0 checkBoolVal
        let checkTransactions = countOf allTransactions (=) 2
        let transactionsAt0 = atTime 0 checkTransactions
        let test = boolValAt0 &|& transactionsAt0
        let atDay0 = test =|> boolValAt0
        
        Assert.IsTrue(atDay0 env trans)

    [<TestMethod>]
    member this.AsianOption () =
        let scale1 = Scale(NumVal("x"),One(Currency CNY))
        let scale2 = Scale(NumVal("y"),One(Currency DKK))
        let c = And(scale1,scale2)
        let asi = asian (Bool true) (NumVal("x")) 20 5 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty asi
        
        let trans = evalC env asi 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay20 = atTime 20 p
        
        Assert.IsTrue(atDay20 env trans)
