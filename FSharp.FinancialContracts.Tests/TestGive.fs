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
type TestGive () =

    [<TestMethod>]
    member this.``Test that 'Give' contract is the opposite of receiveing a contract`` () = 
        let c1 = One DKK
        let contract = c1 &-& (Give c1)
        
        let transactionProperty = forAllTimes isZero 
        let amountProperty = countOf allTransactions (=) 2
        
        PropertyCheck.Check contract (transactionProperty &|& amountProperty)

    [<TestMethod>]
    member this.``Test that 'Give' can compare the sum of two contracts`` () = 
        let c1 = Scale(Const 100.0, One DKK)
        let c2 = And(Scale(Const 25.0, One DKK), Delay(TimeObs.Const 10, Scale(Const 75.0, One DKK)))
        let contract = c1 &-& (Give c2)
        
        let transactionProperty = accumulatedSumOf allTransactions (=) 0.0 
        let amountProperty = accumulatedCountOf allTransactions (=) 3
        
        PropertyCheck.Check contract (transactionProperty &|& amountProperty)





        

