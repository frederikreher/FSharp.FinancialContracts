namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property

module program =
    open Generators
    open Generators
    open Generators
    open FSharp.FinancialContracts
     
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
        let contract1 = And(And(c3, 
                             And(ScaleNow(Const 1000.0, One DKK), ScaleNow(Const 1000.0, One EUR))),
                           Delay(TimeObs.Const 365, One DKK))
                           
        let contract = Delay(TimeObs.Const 365, contract1)

        let ifEven : ValueGenerator = fun _ t -> BoolValue(t%2=0)
        let currentTime : ValueGenerator = fun _ t -> NumberValue(float t)
    
        let c1 = If(BoolVal "b", TimeObs.Const 5, One DKK, Zero)
        let c2 = If(BoolVal "b", TimeObs.Const 5, Zero, One DKK)
        
        let property = forAllTimes 
                            !!(satisfyBoolObs (BoolVal "b")) =|> hasNoTransactions
    
        
        let run = 100
        
        let fastEvaluation = ("fastEvaluation", fun env c -> evaluateContract env c |> ignore)
        //let simpleEvaluation = ("simpleEvaluation", fun env c -> ContractEvaluation.evaluateContract env c |> ignore)
        
        //PerformanceChecker.checkPerformance [contract] fastEvaluation simpleEvaluation
        
        let sumOfDKKAre20  : Property = 
            sumOf (transactionsOfCurrency DKK) (=) 20.0
        let countOfAllAre1 : Property = countOf allTransactions (=) 1
        
        let bIsTrue = satisfyBoolObs (BoolVal "b")
        let sumAndCount = (sumOfDKKAre20 &|& countOfAllAre1)
        let bImpliesSumAndCount = bIsTrue =|> sumAndCount
        printfn "lol"
        let bImpliesSumAndCountOnce = 
            forSomeTime bIsTrue =|> 
                forOneTime (bIsTrue &|& sumAndCount)
                    
        let contract = If(BoolVal "b", TimeObs.Const 5, Scale(Const 20.0, One DKK), Give(And(One GBP,One USD)))
        let environment = EnvironmentGenerators.WithDefaultGenerators contract
        let transactions = evaluateContract environment contract
        let propertyIsFulfilled = bImpliesSumAndCountOnce environment transactions
        
        printfn "%A result was with env %A and transactions %A" propertyIsFulfilled environment transactions
                        
        let lowExchangeRate = LessThan(NumVal "USD/CNY", Const 0.15)
        let tttt = If(lowExchangeRate, TimeObs.Const 30, Scale(Const 100.0, One CNY), Zero)

        Delay(TimeObs.If(lowExchangeRate, TimeObs.Const 10, TimeObs.Const 20), One USD)

        0 // return an integer exit code
