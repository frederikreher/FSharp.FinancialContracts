namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.PropertyCheckers
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal

[<TestClass>]
type TestIfWithin () =

    [<TestMethod>]
    member this.``Test If boolean is never true c1 is never evaluated and c2 is always evaluated`` () =
        let tc = 5
        let contract = If(BoolVal("x"),tc,One (DKK),One (CNY))
        let targetTransaction1 = [Transaction(1.0, DKK)]
        let targetTransaction2 = [Transaction(1.0, CNY)]
        
        let property = (forAllTimes (!!(hasTransactions targetTransaction1))) &|& forSomeTime (hasTransactions targetTransaction2)
        let boolGenMap = Map.empty
                            .Add(BoolVal "x", fun _ -> false)
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators Map.empty boolGenMap}
        
        PropertyCheck.CheckWithConfig config contract property
    
    [<TestMethod>]
    member this.``Test If boolean is true at time 5 c1 is evaluated on that time and not at other times`` () =
        let t = 5
        let contract = If(BoolVal("x"),t,One (DKK),One (CNY))
        
        let property = atTime t (hasTransactions [Transaction(1.0, DKK)]) &|& !!(atTime (t-1) (hasTransactions [Transaction(1.0, DKK)]))
        let boolGenMap = Map.empty
                                                    .Add(BoolVal "x", BoolGenerators.BoolTrueAtDate 5)
                                                    
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators Map.empty boolGenMap}  
        PropertyCheck.CheckWithConfig config contract property                                                  
        
    [<TestMethod>]
    member this.``Test If boolean is never true c2 is evaluated at end t and not at other times`` () =
        let t = 10
        let contract = If(BoolVal("x"),t,One (DKK),One (CNY))
                
        let property = atTime t (hasTransactions [Transaction(1.0, CNY)]) &|& !!(atTime (t-1) (hasTransactions [Transaction(1.0, CNY)]))
        let boolGenMap = Map.empty
                                                    .Add(BoolVal "x", fun _ -> false)
                                                    
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators Map.empty boolGenMap}  
        PropertyCheck.CheckWithConfig config contract property   
        
    [<TestMethod>]
    member this.``Test If boolean is true then c1 is evaluated`` () =
        let t = 5
        let c1 = One (DKK)
        let contract = If(BoolVal("x"),t,c1,One (CNY))
        let targetTransaction1 = [Transaction(1.0, DKK)]
        
        let property = (forSomeTime (satisfyBoolObs (BoolVal("x"))) =|> forOneTime (hasTransactions targetTransaction1))
        PropertyCheck.Check contract property 