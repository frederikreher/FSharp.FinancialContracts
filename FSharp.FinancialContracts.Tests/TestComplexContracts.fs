﻿namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Observables
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
    member this.``Test horizon`` () = 
        let Zcb = zcb (TimeObs.Const 0) (Const 1.0) DKK
        let American c = american (BoolVal "x") (TimeObs.Const 30) (Scale(NumVal "y", c))
        let Asian = asian (BoolVal "x") (NumVal "y") (TimeObs.Const 120) 30
        let European = european (LessThan(NumVal "a", NumVal "b")) (TimeObs.Const 180)

        let cc = Zcb |> American |> Asian |> European

        let horizon = getHorizon cc
        
        Assert.AreEqual(331, horizon);

    [<TestMethod>]
    member this.``Test sum of transactions`` () = 
        let Zcb = zcb (TimeObs.Const 0) (Const 1.0) DKK
        let American = Delay(TimeObs.Const 1, american (BoolVal "x") (TimeObs.Const 3) (Scale(NumVal "y", Zcb)))
        let Asian = asian (BoolVal "x") (NumVal "y") (TimeObs.Const 12) 3 American
        let European = european (LessThan(NumVal "a", NumVal "b")) (TimeObs.Const 18) Asian

        let cc = European

        let genMap = Map.empty
                            .Add("y", fun _ t -> NumberValue(if t = 33 then 50.0 else 20.0))
                            .Add("a", fun _ _ -> NumberValue(5.0))
                            .Add("b", fun _ _ -> NumberValue(10.0))
                            .Add("x", fun _ t -> BoolValue(if (t = 30 || t = 33) then true else false))
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap }

        let sumProperty = accumulatedSumOf allTransactions (=) 1000.0

        PropertyCheck.CheckWithConfig config cc sumProperty

    [<TestMethod>]
    member this.``Test number of transactions`` () = 
        let ZcbDKK = zcb (TimeObs.Const 0) (Const 1.0) DKK
        let ZcbEUR = zcb (TimeObs.Const 0) (Const 7.5) EUR
        let am1 = american (BoolVal "x") (TimeObs.Const 5) (Scale(NumVal "y", ZcbDKK))
        let am2 = american (BoolVal "x") (TimeObs.Const 3) (Scale(NumVal "z", ZcbEUR))
        let American = Delay(TimeObs.Const 1, And(am1, am2))
        let Asian = asian (BoolVal "x") (NumVal "y") (TimeObs.Const 12) 3 American
        let European = european (LessThan(NumVal "a", NumVal "b")) (TimeObs.Const 18) Asian

        let cc = European

        let genMap = Map.empty
                                    .Add("y", fun _ t -> NumberValue(if t = 33 then 50.0 else 20.0))
                                    .Add("a", fun _ _ -> NumberValue(5.0))
                                    .Add("b", fun _ _ -> NumberValue(10.0))
                                    .Add("x", fun _ t -> BoolValue(if (t = 30 || t = 33) then true else false))
        
        let boolObsX = Or((Equal(NumVal("t"),Const(30.0))),(Equal(NumVal("t"),Const(33.0))))
        
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap }

        let amountProperty = accumulatedCountOf allTransactions (=) 2

        PropertyCheck.CheckWithConfig config cc amountProperty

    [<TestMethod>]
    member this.``Test number of transactions with random generators`` () = 
        let dkkEUR = LessThan(NumVal "DKK/EUR", Const 7.0)
        let buy100EUR = Scale(Const 100.0, One EUR)
        let buy500DKK = Scale(Const 500.0, One DKK)
        let c0 = And(If(BoolVal "x", TimeObs.Const 10, 
                        Give(ScaleNow(Mult(NumVal "x", Const 100.0), One DKK)), 
                        Give(Scale(Sub(NumVal "y", NumVal "x"), One EUR))),
                     One CNY)
        let c1 = Delay(TimeObs.Const 100, c0)
        let c2 = If(GreaterThan(NumVal "DKK/EUR", Const 7.5), (TimeObs.Const 60), buy500DKK, c1)
        let c3 = If(dkkEUR, TimeObs.Const 30, buy100EUR, c2)
        let contract = And(c3,
                           And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR)))

        let property = accumulatedCountOf allTransactions (=) 3 ||| countOf allTransactions (=) 4

        PropertyCheck.Check contract property
    
    [<TestMethod>]
    member this.``Test number of transactions with implies`` () = 
        let dkkEUR = LessThan(NumVal "DKK/EUR", Const 7.0)
        let buy100EUR = Scale(Const 100.0, One EUR)
        let buy500DKK = Scale(Const 500.0, One DKK)
        let c0 = And(If(BoolVal "x", TimeObs.Const 10, 
                        Give(ScaleNow(Mult(NumVal "x", Const 100.0), One DKK)), 
                        Give(Scale(Sub(NumVal "y", NumVal "x"), One EUR))),
                     One CNY)
        let c1 = Delay(TimeObs.Const 100, c0)
        let c2 = If(GreaterThan(NumVal "DKK/EUR", Const 7.5), (TimeObs.Const 60), buy500DKK, c1)
        let c3 = If(dkkEUR, TimeObs.Const 30, buy100EUR, c2)
        let contract = And(c3, 
                           And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR)))

        let property = 
            atTime 0 (hasTransactions [Transaction(1000.0, DKK); Transaction(1000.0, EUR)]) &|&
            (forSomeTime (satisfyBoolObs dkkEUR)) =|> 
                (forOneTime (satisfyBoolObs dkkEUR &|& 
                             hasTransactions [Transaction(100.0, EUR)]))

        PropertyCheck.Check contract property