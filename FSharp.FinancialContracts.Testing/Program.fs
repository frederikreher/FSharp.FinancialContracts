namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property

module program =
    open FSharp.FinancialContracts.Documentation
     
    [<EntryPoint>]
    let main argv =
        let dkkEUR = LessThan(NumVal "DKK/EUR", Const 7.0)
        let buy100EUR = Scale(Const 100.0, One EUR)
        let buy500DKK = Scale(Const 500.0, One DKK)
        let c0 = And(If(BoolVal "x", 10, 
                        Give(ScaleNow(Mult(NumVal "x", Const 100.0), One DKK)), 
                        Give(Scale(Sub(NumVal "y", NumVal "x"), One EUR))),
                     One CNY)
        let c1 = Delay(100, c0)
        let c2 = If(GreaterThan(NumVal "DKK/EUR", Const 7.5), 60, buy500DKK, c1)
        let c3 = If(dkkEUR, 30, buy100EUR, c2)
        let contract1 = And(And(c3, 
                             And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR))),
                           Delay(365, One DKK))
                           
        let contract = Delay(365, contract1)

        let ifEven : ValueGenerator = fun t -> BoolValue(t%2=0)
        let currentTime : ValueGenerator = fun t -> NumberValue(float t)
    
        let c1 = If(BoolVal "b", 5, One DKK, Zero)
        let c2 = If(BoolVal "b", 5, Zero, One DKK)
        
        let property = forAllTimes 
                            !!(satisfyBoolObs (BoolVal "b")) =|> hasNoTransactions
    
        let run = 100
        
        let fastEvaluation = ("fastEvaluation", fun env c -> evaluateContract env c |> ignore)
        let simpleEvaluation = ("simpleEvaluation", fun env c -> ContractEvaluation.evaluateContract env c |> ignore)
        
        PerformanceChecker.checkPerformance [contract] fastEvaluation simpleEvaluation
        
        0 // return an integer exit code
