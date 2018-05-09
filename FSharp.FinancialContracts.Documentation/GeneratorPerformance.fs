// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System

module GeneratorPerformance = 
    let Run () =
//           let customGenerators = Map.empty
//                                           .Add( "x", BoolGenerators.BoolTrueAtTime 50)
//                                           .Add( "y", fun _ t -> if t = 20 then NumberValue 20.0 else NumberValue 10.0)
//                                           .Add( "z", fun _ _ -> NumberValue 20.0)
//                                                
            let rec sum (numbobss:NumberObs list) = 
                match numbobss with
                   | [] -> Const 0.0
                   | obs::rest -> Add(obs,sum rest)
                   
            
            let rec conjunction (boolobss:BoolObs list) = 
                match boolobss with
                   | [] -> Bool true
                   | obs::rest -> BoolObs.And(obs,conjunction rest)
            
            let boolVals2 = [BoolVal "b1";BoolVal "b2"]
            let boolVals4 = [BoolVal "b1";BoolVal "b2";BoolVal "b3";BoolVal "b4"]
            let boolVals8 = [BoolVal "b1";BoolVal "b2";BoolVal "b3";BoolVal "b4";BoolVal "b5";BoolVal "b6";BoolVal "b7";BoolVal "b8"]
            
            let numVals2 = [NumVal "n1";NumVal "n2"]
            let numVals4 = [NumVal "n1";NumVal "n2";NumVal "n3";NumVal "n4"]
            let numVals8 = [NumVal "n1";NumVal "n2";NumVal "n3";NumVal "n4";NumVal "n5";NumVal "n6";NumVal "n7";NumVal "n8"]
            
           
            let generatorContract2Bool    = If(conjunction boolVals2, TimeObs.Const 365, One DKK, One EUR)
            let generatorContract2Num     = If(Bool true, TimeObs.Const 365, Scale(sum numVals2, One DKK), One DKK)
            let generatorContract2BoolNum = If(BoolVal("x"), TimeObs.Const 365, Scale(NumVal "y", One DKK), One EUR)
            
            let generatorContract4Bool    = If(conjunction boolVals4,TimeObs.Const 365, One DKK, One EUR)
            let generatorContract4Num     = If(Bool true, TimeObs.Const 365, Scale(sum numVals4, One DKK), One DKK)
            let generatorContract4BoolNum = If(conjunction boolVals2,TimeObs.Const 365, Scale(sum numVals2, One DKK), One EUR )
            
            let generatorContract8Bool    = If(conjunction boolVals8,TimeObs.Const 365, One DKK, One EUR)           
            let generatorContract8Num     = If(Bool true, TimeObs.Const 365, Scale(sum numVals8, One DKK), One DKK)
            let generatorContract8BoolNum = If(conjunction boolVals4,TimeObs.Const 365, Scale(sum numVals4, One DKK), One EUR )
            
            
            let envGenerators = [
                 (10000,generatorContract2Bool,EnvironmentGenerators.Default);
                 (10000,generatorContract2Num,EnvironmentGenerators.Default);
                 (10000,generatorContract2BoolNum,EnvironmentGenerators.Default);
                 (10000,generatorContract4Bool,EnvironmentGenerators.Default);
                 (10000,generatorContract4Num,EnvironmentGenerators.Default);
                 (10000,generatorContract4BoolNum,EnvironmentGenerators.Default);
                 (10000,generatorContract8Bool,EnvironmentGenerators.Default);
                 (10000,generatorContract8Num,EnvironmentGenerators.Default); 
                 (10000,generatorContract8BoolNum,EnvironmentGenerators.Default)]
           
            PerformanceChecker.checkGeneratorPerformance envGenerators