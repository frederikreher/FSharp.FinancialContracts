// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Documentation.EvaluationPerfomance
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System

module PropertyCheckerPerformance = 
    let Run () =
        let contract1 = american (BoolVal "b") (TimeObs.Const 365) (One DKK)
        let property1 = 
            forSomeTime (satisfyBoolObs (BoolVal "b" )) =|> 
                forOneTime 
                    ((satisfyBoolObs (BoolVal "b") &|& hasTransactions[Transaction(1.0,DKK)]))
        
        let contract2 = repeat (One DKK) 365
        let property2 = forAllTimes (hasTransactions[Transaction(1.0,DKK)])
        
        let contract3 = And(contract2,If(BoolVal "x", TimeObs.Const 364, Scale(NumVal "y", One DKK), Zero))
        let property3 = property2 &|& 
                        (forSomeTime (satisfyBoolObs (BoolVal "x")) =|> 
                          forOneTime 
                              ((satisfyBoolObs (BoolVal "x")) &|& (countOf allTransactions (=) 2)))
                              
        let contract4 = repeat (Scale(NumVal "x", One DKK)) 1000
        let property4 = forAllTimes !!hasNoTransactions
        
        let properties = [
                            (1000,contract1,EnvironmentGenerators.Default,property1)
                            (1000,contract2,EnvironmentGenerators.Default,property2)
                            (1000,contract3,EnvironmentGenerators.Default,property3)
                         ] 
         
        //PerformanceChecker.checkPropertyPerformance properties
        PerformanceChecker.checkParallelPerformance contract4 property4