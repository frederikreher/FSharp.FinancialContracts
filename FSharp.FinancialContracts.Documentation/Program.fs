// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Documentation
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System

module program = 

    [<EntryPoint>]
    let main argv =
        let repeat contract n =  
                    let rec rep c i =
                        if i = n then c
                        else And(contract, Delay(TimeObs.Const 1, rep contract (i+1)))
                        
                    if n = 0 then Zero
                    else rep contract 1  
        
        let repeatComplex contract n =  
                    let rec rep c i =
                        if i = n then c
                        else Scale(Const 100.0, And(contract, If(BoolVal "x", TimeObs.Const i, rep contract (i+1), rep contract (i+1))))
                        
                    if n = 0 then Zero
                    else rep contract 1
                              
//        let contract1 = One DKK
//        let contract2 = Delay(TimeObs.Const 1,One DKK)
//        let contract3 = Delay(TimeObs.Const 5,One DKK)
//        let contract4 = Delay(TimeObs.Const 10,One DKK)
//        let contract5 = Delay(TimeObs.Const 100,One DKK)
//        let contract6 = Delay(TimeObs.Const 1000,One DKK)
//        let contract7 = Scale(NumVal "x",One DKK)
//        let contract8 = Scale(Const 500.0,One DKK)
//        let contract9 = repeat (Scale(NumVal "x",One DKK)) 365
//        let contract10 = repeat Zero 365
//        
//        
//        let fastEvaluation = ("fastEvaluation", fun env c -> evaluateContract env c)
//        let simpleEvaluation = ("simpleEvaluation", fun env c -> ContractEvaluation.evaluateContract env c)
//        
//        let contracts = [(10000,contract1);
//                         (10000,contract2);
//                         (10000,contract3);
//                         (10000,contract4);
//                         (1000,contract5);
//                         (1000,contract6);
//                         (10000,contract7);
//                         (10000,contract8);
//                         (100,contract9);
//                         (100,contract10);
//                        ]
                         
        //PerformanceChecker.checkPerformance contracts fastEvaluation simpleEvaluation
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
        
        //PerformanceChecker.checkGeneratorPerformance envGenerators
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
        
        let properties = [
                            (1000,contract1,EnvironmentGenerators.Default,property1)
                            (1000,contract2,EnvironmentGenerators.Default,property2)
                            (1000,contract3,EnvironmentGenerators.Default,property3)
                         ] 
         
        //PerformanceChecker.checkPropertyPerformance properties
        
        PerformanceChecker.checkParallelPerformance contract3 property3
        
        0 // return an integer exit code
