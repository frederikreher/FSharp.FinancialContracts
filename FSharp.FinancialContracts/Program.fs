// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Contracts

module program = 

    [<EntryPoint>]
    let main argv = 
        let c = Give (One EUR) &-& One USD
        let delayed = Delay(TimeObs.Const 5, c)
        let delayed2 = Delay(TimeObs.Const 2, Delay(TimeObs.Const 3, Give (One EUR) &-& One USD))
        let option = american (BoolVal "isExercised") (TimeObs.Const 5) c
        printfn "%A" option
        0 // return an integer exit code
