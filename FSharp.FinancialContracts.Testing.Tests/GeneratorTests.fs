namespace FSharp.FinancialContracts.Tests

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contracts
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Testing.Generators
open Microsoft.VisualStudio.TestTools.UnitTesting
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing
open FSharp.FinancialContracts.Testing.PropertyCheckers
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal

[<TestClass>]
type GeneratorTests () =
    
    let notExistingNumber numVal env =
        try 
            evalNumberObs numVal env |> ignore
            false
        with
            _ -> true
                        
    let notExistingBool boolVal env =
        try 
            evalBoolObs boolVal env |> ignore
            false
        with
            _ -> true
    

    [<TestMethod>]
    member this.``Test Random Number Within Range 100 times`` () =
        let scale1 = Scale(NumVal("x"),One(CNY))
        let scale2 = Scale(NumVal("y"),One(DKK))
        let c = Delay(200,And(scale1,scale2))

        let numGenMap = Map.empty
                            .Add(NumVal "x", NumericGenerators.RandomNumberWithinRange 20.0 40.0)
                            .Add(NumVal "y", NumericGenerators.RandomNumberWithinRange 40.0 60.0)

        let envGen = EnvironmentGenerators.WithCustomGenerators numGenMap Map.empty
        let xProperty = (satisfyNumObs (NumVal "x") (>=) 20.0) &|& (satisfyNumObs (NumVal "x") (<=) 40.0)
        let yProperty = (satisfyNumObs (NumVal "y") (>=) 40.0) &|& (satisfyNumObs (NumVal "y") (<=) 60.0)
        let p = forAllTimes xProperty &|& yProperty
        
        let config = {
                   Configuration.Default with 
                        EnvironmentGenerator = envGen
                        MaxFail = 3
                        FailSilently = false
                }
        
        PropertyCheck.CheckWithConfig config c p  
    
    [<TestMethod>]
        member this.``Test That All Observables Have Values Generated`` () =            
            let add = Add(NumVal("x"),NumVal("y"))
            let subScale = Scale(NumVal("t"),One GBP)
            let c = If(BoolVal("b"),10,Scale(Mult(add,NumVal("z")),One DKK),subScale)
            
            let boolValueIsGenerated = fun env _ ->
                evalBoolObs (BoolVal("b")) env  |> ignore
                true
            
            let numValueIsGenerated = fun env _ ->
                evalNumberObs (NumVal("x")) env |> ignore
                evalNumberObs (NumVal("y")) env |> ignore
                evalNumberObs (NumVal("z")) env |> ignore
                evalNumberObs (NumVal("t")) env |> ignore
                true
                    
            let valuesAreNotGenerated = fun env _ ->
                notExistingNumber (NumVal("b")) env &&
                notExistingBool   (BoolVal("x")) env &&
                notExistingBool   (BoolVal("y")) env &&
                notExistingBool   (BoolVal("z")) env &&
                notExistingBool   (BoolVal("t")) env
                            
            let p = forAllTimes(numValueIsGenerated &|& boolValueIsGenerated &|& valuesAreNotGenerated)
                            
            PropertyCheck.Check c p 