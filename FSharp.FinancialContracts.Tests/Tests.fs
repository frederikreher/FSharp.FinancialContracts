namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Properties
open FSharp.FinancialContracts.Testing.Property
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestTestingFramework () =
        let list = [Transaction(2.0,(Currency DKK))]

        let sumProperty = sumIs 2.0 (Currency DKK)
        let countProperty = countIs 2

        Assert.IsTrue(list |> testProperty (sumProperty && countProperty))

