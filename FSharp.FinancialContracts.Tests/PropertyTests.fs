namespace FSharp.FinancialContracts.Tests

open System
open System.IO
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Properties
open FSharp.FinancialContracts.Testing.TestRunners
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.PropertyCheckers
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing
open System.Text

[<TestClass>]
type PropertyTests () =

    [<TestMethod>]
    member this.``Test That Give And Take Results In Zero Sum`` () =
        //Arrange
        let contract = One DKK
        let neutralContract = And(contract,One GBP)
        let env = EnvironmentGenerators.WithDefaultGenerators neutralContract
        
        //Act   
        let tsr = evalC env neutralContract
        printfn "result is %A" tsr
        
        
        
        //Assert
       // Assert.IsTrue(isZero env tsr)
        PropertyCheck.Check neutralContract isZero

    [<TestMethod>]
    member this.``Test That Give And Take Results In Zero Sum For All Times`` () =
            //Arrange
            let observable = NumVal("x")
            let simpleContract = Scale(observable,One DKK)
            let neutralContract = Delay(1,And(simpleContract,Give simpleContract))
            let contract = Delay(1,neutralContract) &-& Delay(2,neutralContract) &-& Delay(5,neutralContract)
            
            let env = EnvironmentGenerators.WithDefaultGenerators contract
            
            //Act
            let tsr = evalC env contract
            printfn "result is %A" tsr
            
            //Assert
            Assert.IsTrue(forAllTimes isZero env tsr)

    [<TestMethod>]
    member this.``Test 100000 Times That Give And Take Results In Zero Sum For All Times`` () =
            //Arrange
            let observable = NumVal("x")
            let simpleContract = Scale(observable,One DKK)
            let neutralContract = Delay(1,And(simpleContract,Give simpleContract))
            let contract = Delay(1,neutralContract) &-& Delay(2,neutralContract) &-& Delay(5,neutralContract) &-& One DKK
            
            let testResult = TestRunners.Runner.RunNTimes 100000 contract (forAllTimes isZero)
            
            match testResult with
            | (b, env, tsr) ->
                //printfn "%A" env
                //printfn "%A" tsr
                Assert.IsTrue(b)
            
