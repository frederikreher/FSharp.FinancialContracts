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
type TestScaleAndScaleNow () =

    [<TestMethod>]
    member this.``Test that scale evaulates observable when delayed`` () =
        let tc = 10
        let factor = 20.0
        
        let contract = Scale(NumVal("x"),Delay(tc, One DKK))
        
        let targetTransaction = Transaction(factor, DKK)       
        
        let property = atTime tc (hasTransaction targetTransaction)
        
        let numGenMap = Map.empty
                                            .Add(NumVal "x", fun t -> if t = tc then factor else NumericGenerators.RandomNumber t)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty }
        
        PropertyCheck.CheckWithConfig config contract property
    
    [<TestMethod>]
    member this.``Test that ScaleNow evaulates from current time`` () =
        let tc = 10
        let factor = 20.0
        
        let contract = ScaleNow(NumVal("x"),Delay(tc, One DKK))
        
        let targetTransaction = Transaction(factor, DKK)       
        
        let property = atTime tc (hasTransaction targetTransaction)
        
        let numGenMap = Map.empty
                                            .Add(NumVal "x", fun t -> if t = 0 then factor else NumericGenerators.RandomNumber t)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty }
        
        PropertyCheck.CheckWithConfig config contract property
        
    [<TestMethod>]
    member this.``Test that nested delay produces same result as unnested delay for Scale`` () =
        let tc = 10
        let factor = 20.0
        
        let c1 = Scale(NumVal("x"),Delay(tc, One DKK))
        let c2 = Delay(tc,Scale(NumVal("x"),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property
    
    [<TestMethod>]
    member this.``Test that nested delay doesn't produces same result as unnested delay for ScaleNow`` () =
        let tc = 10
        
        let c1 = ScaleNow(NumVal("x"),Delay(tc, One DKK))
        let c2 = Delay(tc,ScaleNow(NumVal("x"),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes (!!isZero ||| hasNoTransactions ) 
        
        PropertyCheck.Check contract property
        
    
    [<TestMethod>]
    member this.``Test that nested scale produces same result as multiplied obs`` () =
        let tc = 10
        
        let c1 = Scale(NumVal("y"),Scale(NumVal("x"),Delay(tc, One DKK)))
        let c2 = Delay(tc,Scale(Mult(NumVal("x"),NumVal("y")),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property