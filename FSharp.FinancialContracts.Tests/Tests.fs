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
    member this.``Test Testing Framework`` () =
        let list = [Transaction(2.0,(DKK))]

        let sumProperty = sumIs 2.0 (DKK)
        let countProperty = countIs 2

        Assert.IsTrue(true)

    [<TestMethod>]
    member this.``European Option`` () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = And(scale1,scale2)
        let europ = european (Bool true) 20 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty europ
        
        let trans = evalC env europ 

        let allTransactions = fun t -> true

        let p = countOf allTransactions (=) 2
        let atDay20 = atTime 20 p
        
        Assert.IsTrue(atDay20 env trans)

    [<TestMethod>]
    member this.``American Option`` () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
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
    member this.``American Option With Random Environment`` () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
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
    member this.``Asian Option`` () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
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
        
    [<TestMethod>]
    member this.``Contracts Are Equal With Constant Values`` () =
        let c1 = Scale(Const 2.0,One(DKK))
        let c2 = And(One(DKK), One(DKK))

        let env = EnvironmentGenerators.WithDefaultGenerators c1
        
        let trans1 = evalC env c1
        let trans2 = evalC env (Give c2)
        
        let prop = isZero trans1

        Assert.IsTrue(prop env trans2)

    [<TestMethod>]
    member this.``Contracts Are Equal With Random Values`` () =
        let c1 = Scale(Const 2.0, Scale(NumVal "x", One(DKK)))
        let c2 = And(Scale(NumVal "x", One(DKK)), Scale(NumVal "x", One(DKK)))
        let c3 = And(Scale(NumVal "x", One(USD)), Scale(NumVal "x", One(DKK)))

        let env = EnvironmentGenerators.WithDefaultGenerators c1
        
        let trans1 = evalC env c1
        let trans2 = evalC env (Give c2)
        let trans3 = evalC env (Give c3)

        let prop = isZero trans2 &|& !! (isZero trans3)

        Assert.IsTrue(prop env trans1)

