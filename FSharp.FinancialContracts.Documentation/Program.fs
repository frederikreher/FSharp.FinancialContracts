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
        let dkkEUR = LessThan(NumVal "DKK/EUR", Const 7.0)
        let buy100EUR = Scale(Const 100.0, One EUR)
        let buy500DKK = Scale(Const 500.0, One DKK)
        let c0 = And(If(BoolVal "x", TimeObs.Const 10, 
                                Give(ScaleNow(Mult(NumVal "x", Const 100.0), One DKK)), 
                                Give(Scale(Sub(NumVal "y", NumVal "x"), One EUR))),
                             One CNY)
        let c1 = Delay(TimeObs.Const 100, c0)
        let c2 = If(GreaterThan(NumVal "DKK/EUR", Const 7.5), TimeObs.Const 60, buy500DKK, c1)
        let c3 = If(dkkEUR, TimeObs.Const 30, buy100EUR, c2)
        let contract = And(And(c3, 
                                 And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR))),
                               Delay(TimeObs.Const 365, One DKK))
                               
        let contract1 = Delay(TimeObs.Const 10000, contract)
        
        let contract2 = Delay(TimeObs.Const 100, )
        let contract3 = One DKK
        let contract4 = Delay(TimeObs.Const 10,One DKK)
        let contract5 = Scale(NumVal "x",One DKK)
        
        let fastEvaluation = ("fastEvaluation", fun env c -> evaluateContract env c |> ignore)
        let simpleEvaluation = ("simpleEvaluation", fun env c -> ContractEvaluation.evaluateContract env c |> ignore)
        
        let contracts = [(1000,contract1);
                         (1000,contract2);
                         (1000000,contract3);
                         (1000000,contract4);
                         (1000000,contract5)]
                
        PerformanceChecker.checkPerformance contracts fastEvaluation simpleEvaluation
        
        0 // return an integer exit code
