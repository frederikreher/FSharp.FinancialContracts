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
type TestScaleAndScaleNow () =

    [<TestMethod>]
    member this.``Test that scale evaulates observable when delayed`` () =
        let tc = 10
        let factor = 20.0
        
        let contract = Scale(NumVal("x"),Delay(TimeObs.Const tc, One DKK))
        
        let targetList = [Transaction(factor, DKK)]
        
        let transactionProperty = atTime tc (hasTransactions targetList)
        let amountProperty = accumulatedCountOf allTransactions (=) 1
        
        let genMap = Map.empty
                            .Add( "x", fun r t -> if t = tc then NumberValue factor else NumericGenerators.RandomNumber r t)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap }

        PropertyCheck.CheckWithConfig config contract (transactionProperty &|& amountProperty)

    [<TestMethod>]
    member this.``Test that ScaleNow evaulates from current time`` () =
        let tc = 10
        let factor = 20.0
        
        let contract = ScaleNow(NumVal("x"),Delay(TimeObs.Const tc, One DKK))
        
        let targetList = [Transaction(factor, DKK)]
        
        let transactionProperty = atTime tc (hasTransactions targetList)
        let amountProperty = accumulatedCountOf allTransactions (=) 1
        
        let genMap = Map.empty
                        .Add("x", fun r t -> if t = 0 then NumberValue factor else NumericGenerators.RandomNumber r t)
        
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap }
        
        PropertyCheck.CheckWithConfig config contract (transactionProperty &|& amountProperty)
        
    [<TestMethod>]
    member this.``Test that nested delay produces same result as unnested delay for Scale`` () =
        let tc = 10
        let factor = 20.0
        
        let c1 = Scale(NumVal("x"),Delay(TimeObs.Const tc, One DKK))
        let c2 = Delay(TimeObs.Const tc,Scale(NumVal("x"),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property
    
    [<TestMethod>]
    member this.``Test that nested delay doesn't produces same result as unnested delay for ScaleNow`` () =
        let tc = 10
        
        let c1 = ScaleNow(NumVal("x"),Delay(TimeObs.Const tc, One DKK))
        let c2 = Delay(TimeObs.Const tc,ScaleNow(NumVal("x"),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes (!!isZero ||| hasNoTransactions ) 
        
        PropertyCheck.Check contract property
        
    
    [<TestMethod>]
    member this.``Test that nested scale produces same result as multiplied obs`` () =
        let tc = 10
        
        let c1 = Scale(NumVal("y"),Scale(NumVal("x"),Delay(TimeObs.Const tc, One DKK)))
        let c2 = Delay(TimeObs.Const tc,Scale(Mult(NumVal("x"),NumVal("y")),One DKK))
        let contract = c1 &-& (Give c2)    
        
        let property = forAllTimes isZero
        
        PropertyCheck.Check contract property

    [<TestMethod>]
    member this.``Test that average works as intended`` () =
        let c = Delay(TimeObs.Const 10, Scale(Average(NumVal "x", 5), One EUR))

        let gen = Map.empty.Add("x", fun _ t -> NumberValue (float t))
        let conf = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators gen}

        let property = atTime 10 (hasTransactions [Transaction(7.5, EUR)])


        PropertyCheck.CheckWithConfig conf c property

