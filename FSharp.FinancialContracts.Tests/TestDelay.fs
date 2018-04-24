namespace FSharp.FinancialContracts.Tests

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
type TestDelay () =

    [<TestMethod>]
    member this.``Test that Delay by constant result in correct time`` () =
        let tc = 10
        
        let contract = Delay(TimeObs.Const (tc+2),One DKK)
        
        let targetList = [Transaction(1.0, DKK)]
        
        let transactionProperty = atTime tc (hasTransactions targetList)
        let amountProperty = accumulatedCountOf allTransactions (=) 1
        
        PropertyCheck.Check contract (atTime 2 transactionProperty &|& amountProperty)
    
    [<TestMethod>]
    member this.``Test that Delay by complex Timeobs result in correct time`` () =
        //70+(7*("b"?2:3))
        let timeObs = TimeObs.Add((TimeObs.Const 70),
                                    TimeObs.Mult(TimeObs.Const 7,
                                    TimeObs.If(BoolVal "b",TimeObs.Const 2,TimeObs.Const 3)))
        
        
        let contract = Delay(timeObs,One DKK)
                
        let property = (satisfyBoolObs (BoolVal "b") &|& (atTime 84 (hasTransactions [Transaction(1.0, DKK)]))) 
                       ||| (atTime 91 (hasTransactions [Transaction(1.0, DKK)]))
        
        PropertyCheck.Check contract property
    
    [<TestMethod>]
    member this.``Test that nested delays are equal to one larger`` () =
        let tc = 10
        
        let c1 = Delay(TimeObs.Const (tc*2),One DKK)
        let c2 = Delay(TimeObs.Const tc,Delay(TimeObs.Const tc,One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property