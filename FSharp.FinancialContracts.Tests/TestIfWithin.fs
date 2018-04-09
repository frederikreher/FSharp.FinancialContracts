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
type SimpleContractTests () =

    [<TestMethod>]
    member this.``Test If boolean is never true c1 is never evaluated and c2 is always evaluated`` () =
        let contract = If(BoolVal("x"),5,One (DKK),One (CNY))
        let targetTransaction1 = Transaction(1.0, DKK)       
        let targetTransaction2 = Transaction(1.0, CNY)       
        
        let property = (forAllTimes (!!(hasTransaction targetTransaction1))) &|& forSomeTime (hasTransaction targetTransaction2)
        let boolGenMap = Map.empty
                                            .Add(BoolVal "x", fun _ -> false)
        let config = {Configuration.Default with EnvironmentGenerator = EnvironmentGenerators.WithCustomGenerators Map.empty boolGenMap}
        
        PropertyCheck.CheckWithConfig config contract property