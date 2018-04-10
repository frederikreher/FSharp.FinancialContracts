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
    member this.``Test horizon of Complex Contract`` () = 
        let Zcb = zcb 0 (Const 1.0) DKK
        let American c = american (BoolVal "x") 30 (Scale(NumVal "y", c))
        let Asian = asian (BoolVal "x") (NumVal "y") 120 30
        let European = european (LessThan(NumVal "a", NumVal "b")) 180

        let cc = Zcb |> American |> Asian |> European

        let horizon = getHorizon cc
        
        Assert.AreEqual(331, horizon);

    [<TestMethod>]
    member this.``Test sum of transactions in Complex Contract`` () = 
        let Zcb = zcb 0 (Const 1.0) DKK
        let American = Delay(1, american (BoolVal "x") 3 (Scale(NumVal "y", Zcb)))
        let Asian = asian (BoolVal "x") (NumVal "y") 12 3 American
        let European = european (LessThan(NumVal "a", NumVal "b")) 18 Asian

        let cc = European

        let numGenMap = Map.empty
                            .Add(NumVal "y", fun t -> if t = 33 then 50.0 else 20.0)
                            .Add(NumVal "a", fun _ -> 5.0)
                            .Add(NumVal "b", fun _ -> 10.0)
        let boolGenMap = Map.empty
                            .Add(BoolVal "x", fun t -> if (t = 30 || t = 33) then true else false)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators numGenMap boolGenMap }

        let sumProperty = sumOf allTransactions (=) 1000.0

        PropertyCheck.CheckWithConfig config cc sumProperty

    [<TestMethod>]
    member this.``Test number of transactions in Complex Contract`` () = 
        let ZcbDKK = zcb 0 (Const 1.0) DKK
        let ZcbEUR = zcb 0 (Const 7.5) EUR
        let am1 = american (BoolVal "x") 5 (Scale(NumVal "y", ZcbDKK))
        let am2 = american (BoolVal "x") 3 (Scale(NumVal "z", ZcbEUR))
        let American = Delay(1, And(am1, am2))
        let Asian = asian (BoolVal "x") (NumVal "y") 12 3 American
        let European = european (LessThan(NumVal "a", NumVal "b")) 18 Asian

        let cc = European

        let numGenMap = Map.empty
                            .Add(NumVal "y", fun t -> if t = 33 then 50.0 else 20.0)
                            .Add(NumVal "a", fun _ -> 5.0)
                            .Add(NumVal "b", fun _ -> 10.0)
        let boolGenMap = Map.empty
                            .Add(BoolVal "x", fun t -> if (t = 30 || t = 33) then true else false)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators numGenMap boolGenMap }

        let amountProperty = countOf allTransactions (=) 2

        PropertyCheck.CheckWithConfig config cc amountProperty