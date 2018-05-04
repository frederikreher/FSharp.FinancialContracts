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
    let Run =
           let customGenerators = Map.empty
                                           .Add( "x", BoolGenerators.BoolTrueAtTime 50)
                                           .Add( "y", fun _ t -> if t = 20 then NumberValue 20.0 else NumberValue 10.0)
                                           .Add( "z", fun _ _ -> NumberValue 20.0)
                                                  
                   
                   
           let generatorContract1 = If(BoolVal("x"),TimeObs.Const 365, One DKK, One EUR)
           let generatorContract2 = If(BoolVal("x"),TimeObs.Const 365, Scale(NumVal("y"),One DKK), Scale(NumVal("z"),One DKK))
           
           
           let envGenerators = [(10000,generatorContract1,EnvironmentGenerators.Default);
                                (10000,generatorContract1,EnvironmentGenerators.WithCustomGenerators customGenerators);
                                (10000,generatorContract2,EnvironmentGenerators.Default)
                                (10000,generatorContract2,EnvironmentGenerators.WithCustomGenerators customGenerators)
                               ]
           
           PerformanceChecker.checkGeneratorPerformance envGenerators