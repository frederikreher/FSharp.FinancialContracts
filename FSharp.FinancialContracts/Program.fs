// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Environment
open Contract
open Contracts
open System

module program = 

    [<EntryPoint>]
    let main argv =
        let boolObservables = [("50/50",true)]

        let numObservables = [("one", 1.3);
                              ("two", 10.0);
                              ("100", 100.0)]

        // Add function to add to environments???
        let mutable env1: Environment.Environment = 0, [|(boolObservables |> Map.ofList)|], [|(numObservables |> Map.ofList)|]
        
        let c1 = zcb 5 (Const 50.0) (Currency DKK)
        let c2 = american (Bool true) 5 (One(Currency DKK))
        let c3 = european (GreaterThan(Const 5.0, Const 2.0)) 5 (Scale((Const 100.0), One(Currency GBP)))
        let c4 = asian (BoolVal("test")) (NumVal("x")) 5 3 (One(Currency DKK))

        // If vs. Or
        let c7 = Scale(Add(Const 1.0,Mult(NumVal("one"),NumVal("two"))),One(Currency GBP))
        let c8 = If(BoolVal("50/50"), 5, One(Currency DKK), Zero)
        let c9 = If(Not(Bool false), 1, One(Currency DKK), One(Commodity(Gold, KG)))

        let generateObservables t hori c : Environment.Environment = 
            let rndBool = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
            let rndNum = (fun () -> float((new Random()).NextDouble())) 

            let (bools, nums) = getObservables c
            let array = (Array.create (hori) Map.empty)

            let newBoolEnv = List.fold (fun acc index -> 
                                         let boolEnv = List.fold (fun acc1 obs -> (addBoolObs (obs, rndBool()) acc1)) Map.empty bools
                                         Array.set acc index boolEnv
                                         acc
                                       ) array [0..(hori - 1)]
            let newNumEnv = List.fold (fun acc index -> 
                                         let numEnv = List.fold (fun acc1 obs -> (addNumObs (obs, rndNum()) acc1)) Map.empty nums
                                         Array.set acc index numEnv
                                         acc
                                       ) (Array.create (hori) Map.empty) [0..(hori - 1)]
            (t, newBoolEnv, newNumEnv)
            
        printfn "%A" (getHorizon c4)
        for i = 0 to (getHorizon c4) do
            let env = (generateObservables i (i + (getHorizon c4)) c4)
            printfn "%A" env
            printfn "%A" (evalC env c4)
            //Thread.Sleep(1000)

        0 // return an integer exit code
