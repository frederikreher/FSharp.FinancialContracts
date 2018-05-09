// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Documentation
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.Property
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts
open System

module EvaluationPerfomance = 
    let repeat contract n =  
            let rec rep c i =
                if i = n then c
                else And(contract, Delay(TimeObs.Const 1, rep contract (i+1)))
                
            if n = 0 then Zero
            else rep contract 1

    let delay n = Delay(TimeObs.Const n,One DKK)
    let repeatScale n = repeat (Scale(NumVal ("x"),One DKK)) n
    
    
     
    let andNest n     =     
        let rec nest i = 
                    if i = n then One DKK
                    else And(nest (i+1),nest (i+1))
                  
        nest 0          
        
    let scaleNestIf n = 
        let rec nest i = 
            if i = n then One DKK
            else if i%2=0 then If(BoolVal "b1",TimeObs.Const 10, (nest (i+1)), (nest (i+1)))
            else If(BoolVal "b2",TimeObs.Const 10, (nest (i+1)), (nest (i+1)))
          
        Scale(NumVal "x",nest 0)
        
    let repeatComplex contract n =  
                let rec rep c i =
                    if i = n then c
                    else Scale(Const 100.0, And(contract, If(BoolVal "x", TimeObs.Const i, rep contract (i+1), rep contract (i+1))))
                    
                if n = 0 then Zero
                else rep contract 1

    let Run () =                       
        printfn "evaluation was called"
        let fastEvaluation   = ("fastEvaluation", FSharp.FinancialContracts.Contract.evaluateContract)
        let simpleEvaluation = ("simpleEvaluation", ContractEvaluation.simpleEvaluateContract)
        
        let createTests envcount con () =
            [(envcount,con 0);
             ( envcount,con 1);
             ( envcount,con 5);
             ( envcount,con 10);
//             ( envcount,con 100);
//             ( envcount,con 1000);
//             ( envcount,con 10000);
//             ( envcount,con 20000);
            ]
        
        let delayTests = createTests 1000 delay
        
        let scaleTests = createTests 1000 repeatScale
        
        let scaleNestIfTests = createTests 100 scaleNestIf
        let andNestTests = createTests 1000 andNest
        
//        let contract = scaleNestIf 10
//        let env = EnvironmentGenerators.WithDefaultGenerators contract
//        printfn "envornment is %A" env    
//        
//        let evaluation env c = 
//            let (t,arra) = (FSharp.FinancialContracts.Contract.evaluateContract env c)
//            List.ofArray arra
//            
//        let res1 = evaluation env contract
//        let mutable res2 = ContractEvaluation.evaluateContract env contract
//        
//        printfn "%A and %A" res1 res2
//        
//        let diff = res1.Length - res2.Length
//        if diff > 0 then
//            for i in [1..diff] do
//                res2 <- (res2@[[]])
//        
//        
//        printfn "First result is: %A and second is %A" res1 res2
//        
//        if res1 = res2 then printfn "they were equal" 
//        else printfn "something is very wrong"
        
        
        PerformanceChecker.checkPerformance 1000 (scaleNestIfTests()) simpleEvaluation fastEvaluation