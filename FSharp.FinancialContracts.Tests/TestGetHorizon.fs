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
        
    [<TestMethod>]
    member this.``Test that horizon is correct for complex delay`` () =
        let timeObs = TimeObs.Add((TimeObs.Const 70),
                                            TimeObs.Mult(TimeObs.Const 7,
                                            TimeObs.If(BoolVal "b",TimeObs.Const 2,TimeObs.Const 3)))
        
        let contract = Delay(timeObs,Delay(timeObs,One DKK))
              
        let hor = getHorizon contract
        
        Assert.AreEqual(193,hor);