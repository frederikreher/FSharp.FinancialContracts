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
        let list = [Transaction(2.0,(Currency DKK))]

        let sumProperty = sumIs 2.0 (Currency DKK)
        let countProperty = countIs 2

        Assert.IsTrue(true)

    [<TestMethod>]
    member this.EuropeanOption () =
        let scale1 = Scale(NumVal("x"),One(Currency CNY))
        let scale2 = Scale(NumVal("y"),One(Currency DKK))
        let c = And(scale1,scale2)
        let europ = european (Bool true) 20 c

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)

        let env = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty europ


        let trans = evalC env europ 

        printfn "env are %A" env
        printfn "transactions are %A" trans

        let allTransactions = fun ts -> true
        let transactionsOfCurrencies curs : Filter = fun (Transaction(f,c)) -> List.contains c curs

        let sumOf filter (op:(float -> float -> bool)) f = Sum(f,op,filter)

        let p = ((sumOf (transactionsOfCurrencies [Currency CNY]) (<=) 40.0)) || (sumOf (transactionsOfCurrencies [Currency CNY]) (>=) 20.0)

        Assert.IsTrue(testProperty p env trans)
