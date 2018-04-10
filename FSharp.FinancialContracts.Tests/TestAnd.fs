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
type TestAnd () =

    [<TestMethod>]
    member this.``Test that 'And' evaluates both contracts`` () = 
        let contract = And(One DKK,One CNY)
        
        let targetMap = Map.empty<int, Transaction list>
                            .Add(0, [Transaction(1.0, DKK); Transaction(1.0, CNY)])
        
        let transactionProperty = (hasTransactions targetMap)
        let amountProperty = countOf allTransactions (=) 2

        PropertyCheck.Check contract (transactionProperty &|& amountProperty)
        
    [<TestMethod>]
    member this.``Test that 'And' evaluates nested contracts`` () =        
        let contract = And(And(One EUR,One USD),And(One CNY,One GBP))
        
        let targetMap = Map.empty<int, Transaction list>
                            .Add(0, [Transaction(1.0, EUR); Transaction(1.0, USD); 
                                     Transaction(1.0, CNY); Transaction(1.0, GBP)])
        
        let transactionProperty = (hasTransactions targetMap)
        let amountProperty = countOf allTransactions (=) 4

        PropertyCheck.Check contract (transactionProperty &|& amountProperty)