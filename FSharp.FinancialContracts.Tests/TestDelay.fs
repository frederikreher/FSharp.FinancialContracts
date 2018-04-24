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
    member this.``Test that simple contract is delayed by 10`` () =
        let tc = 10
        
        let contract = Delay(TimeObs.Const tc,One DKK)
        
        let targetList = [Transaction(1.0, DKK)]
        
        let transactionProperty = atTime tc (hasTransactions targetList)
        let amountProperty = accumulatedCountOf allTransactions (=) 1
        
        PropertyCheck.Check contract (transactionProperty &|& amountProperty)
        
    [<TestMethod>]
    member this.``Test that nested delays are equal to one larger`` () =
        let tc = 10
        
        let c1 = Delay(TimeObs.Const (tc*2),One DKK)
        let c2 = Delay(TimeObs.Const tc,Delay(TimeObs.Const tc,One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property