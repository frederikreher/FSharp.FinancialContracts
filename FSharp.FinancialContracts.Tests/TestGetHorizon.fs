namespace FSharp.FinancialContracts.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Observables

[<TestClass>]
type TestGetHorizon () =

    [<TestMethod>]
    member this.``Test that horizon is correct for simple delay`` () =
        let contract = Delay(TimeObs.Const 5,One DKK)
              
        let hor = getHorizon contract
        
        Assert.AreEqual(6,hor);