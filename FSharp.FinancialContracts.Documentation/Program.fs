// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Documentation
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
                        else And(contract, Delay(TimeObs.Const i, rep contract (i+1)))
                        
                    if n = 0 then Zero
                    else rep contract 1   
           
                              
        let contract1 = One DKK
        let contract2 = Delay(TimeObs.Const 1,One DKK)
        let contract3 = Delay(TimeObs.Const 5,One DKK)
        let contract4 = Delay(TimeObs.Const 10,One DKK)
        let contract5 = Delay(TimeObs.Const 100,One DKK)
        let contract6 = Delay(TimeObs.Const 1000,One DKK)
        let contract7 = Scale(NumVal "x",One DKK)
        let contract8 = Scale(Const 500.0,One DKK)
        let contract9 = repeat (Scale(NumVal "x",One DKK)) 365
        let contract10 = repeat Zero 365
        
        
        let fastEvaluation = ("fastEvaluation", fun env c -> evaluateContract env c |> ignore)
        let simpleEvaluation = ("simpleEvaluation", fun env c -> ContractEvaluation.evaluateContract env c |> ignore)
        
        let contracts = [(10000,contract1);
                         (10000,contract2);
                         (10000,contract3);
                         (10000,contract4);
                         (1000,contract5);
                         (1000,contract6);
                         (10000,contract7);
                         (10000,contract8);
                         (100,contract9);
                         (100,contract10);
                        ]
                         
            
        PerformanceChecker.checkPerformance contracts fastEvaluation simpleEvaluation
        
        0 // return an integer exit code
