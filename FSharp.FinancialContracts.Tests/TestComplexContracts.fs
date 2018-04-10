namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.PropertyCheckers
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal

[<TestClass>]
type TestComplexContracts () =

    [<TestMethod>]
    member this.``Test Horizon of Complex Contract`` () = 
        let Zcb = zcb 0 (Const 1.0) DKK
        let American c = american (BoolVal "x") 30 (Scale(NumVal "y", c))
        let Asian = asian (BoolVal "x") (NumVal "y") 120 30
        let European = european (LessThan(NumVal "a", NumVal "b")) 180

        let cc = Zcb |> American |> Asian |> European

        let horizon = getHorizon cc
        
        Assert.AreEqual(331, horizon);