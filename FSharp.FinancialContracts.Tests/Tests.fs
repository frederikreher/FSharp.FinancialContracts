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
        let list = [Transaction(2.0,DKK)]

        let sumProperty = sumIs 2.0 DKK
        let countProperty = countIs 1

        Assert.IsTrue(list |> testProperty (sumProperty && countProperty))

