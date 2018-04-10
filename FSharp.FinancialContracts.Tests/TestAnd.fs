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
    member this.``Test that scale evaluates both contracts`` () =        
        let contract = And(One DKK,One CNY)
        
        let targetTransaction = Transaction(1.0, DKK)
        let targetTransaction2 = Transaction(1.0, CNY)              
        
        let property = (hasTransaction targetTransaction) &|& (hasTransaction targetTransaction2)
                
        PropertyCheck.Check contract property
        
    [<TestMethod>]
    member this.``Test that scale evaluates both nested contracts`` () =        
        let contract = And(And(One EUR,One USD),And(One CNY,One GBP))
        
        let targetTransaction = Transaction(1.0, EUR)
        let targetTransaction2 = Transaction(1.0, USD) 
        let targetTransaction3 = Transaction(1.0, CNY)
        let targetTransaction4 = Transaction(1.0, GBP)              
        
        let property = (hasTransaction targetTransaction) &|& (hasTransaction targetTransaction2)
                       &|& (hasTransaction targetTransaction3) &|& (hasTransaction targetTransaction4)
                
        PropertyCheck.Check contract property