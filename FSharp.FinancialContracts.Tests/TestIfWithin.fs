namespace FSharp.FinancialContracts.Tests

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing
open FSharp.FinancialContracts.Testing.PropertyCheckers
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal

[<TestClass>]
type TestIfWithin () =

    [<TestMethod>]
    member this.``Test If boolean is never true c1 is never evaluated and c2 is always evaluated`` () =
        let tc = 5
        let contract = If(BoolVal("x"),TimeObs.Const tc,One (DKK),One (CNY))
        let targetTransaction1 = [Transaction(1.0, DKK)]
        let targetTransaction2 = [Transaction(1.0, CNY)]
        
        let property = (forAllTimes (!!(hasTransactions targetTransaction1))) &|& forSomeTime (hasTransactions targetTransaction2)
        let genMap = Map.empty
                            .Add("x", fun _ -> BoolValue(false))
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap}
        
        PropertyCheck.CheckWithConfig config contract property
    
    [<TestMethod>]
    member this.``Test If boolean is true at time 5 c1 is evaluated on that time and not at other times`` () =
        let t = 5
        let contract = If(BoolVal("x"),TimeObs.Const t,One (DKK),One (CNY))
        
        let property = atTime t (hasTransactions [Transaction(1.0, DKK)]) &|& !!(atTime (t-1) (hasTransactions [Transaction(1.0, DKK)]))
        let genMap = Map.empty
                                                    .Add("x", BoolGenerators.BoolTrueAtTime 5)
                                                    
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap}  
        PropertyCheck.CheckWithConfig config contract property                                                  
        
    [<TestMethod>]
    member this.``Test If boolean is never true c2 is evaluated at end t and not at other times`` () =
        let t = 10
        let contract = If(BoolVal("x"),TimeObs.Const t,One (DKK),One (CNY))
                
        let property = atTime t (hasTransactions [Transaction(1.0, CNY)]) &|& !!(atTime (t-1) (hasTransactions [Transaction(1.0, CNY)]))
        let genMap = Map.empty.Add("x", fun _ -> BoolValue(false))
                                                    
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap}  
        PropertyCheck.CheckWithConfig config contract property   
        
    [<TestMethod>]
    member this.``Test If boolean is true then c1 is evaluated`` () =
        let t = 5
        let c1 = One (DKK)
        let contract = If(BoolVal("x"),TimeObs.Const t,c1,One (CNY))
        let targetTransaction1 = [Transaction(1.0, DKK)]
        
        let property = (forSomeTime (satisfyBoolObs (BoolVal("x"))) =|> forOneTime (hasTransactions targetTransaction1))
        PropertyCheck.Check contract property 
        
    [<TestMethod>]
    member this.``Test If TimeObs is true then within is correct `` () =
       let timeObs = TimeObs.If(BoolVal ("b"), TimeObs.Const 20, TimeObs.Const 10)
       let contract = If(BoolVal("x"),timeObs,One (DKK),Zero)
       
       let hasTransactions = (hasTransactions [Transaction(1.0, DKK)])
       let property = ((satisfyBoolObs (BoolVal "b")) &|& atTime 20 hasTransactions &|& atTime 19 !!hasTransactions)
                      ||| (!!(satisfyBoolObs (BoolVal "b") &|& forAllTimes hasNoTransactions))
                    
       let genMap = Map.empty.Add("x", BoolGenerators.BoolTrueAtTime 20)
                                                   
       let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators genMap}  
       PropertyCheck.CheckWithConfig config contract property   