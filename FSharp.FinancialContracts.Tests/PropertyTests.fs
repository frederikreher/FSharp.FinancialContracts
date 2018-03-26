namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Properties
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract

[<TestClass>]
type PropertyTests () =

    [<TestMethod>]
    member this.TestThatGiveAndTakeResultsInZeroSum () =
        //Arrange
        let contract = One DKK
        let neutralContract = And(contract,Give contract)
        let env = EnvironmentGenerators.WithDefaultGenerators neutralContract
        
        //Act
        let tsr = evalC env neutralContract
        printfn "result is %A" tsr
        
        //Assert
        Assert.IsTrue(isZero env tsr)

    [<TestMethod>]
    member this.TestThatGiveAndTakeResultsInZeroSumForAllTimes () =
            //Arrange
            let observable = NumVal("x")
            let simpleContract = Scale(observable,One DKK)
            let neutralContract = Delay(1,And(simpleContract,Give simpleContract))
            let contract = Delay(1,neutralContract) &-& Delay(2,neutralContract) &-& Delay(5,neutralContract) &-& One GBP
            
            let env = EnvironmentGenerators.WithDefaultGenerators contract
            
            //Act
            let tsr = evalC env contract
            printfn "result is %A" tsr
            
            //Assert
            Assert.IsTrue(forAllTimes isZero env tsr)
