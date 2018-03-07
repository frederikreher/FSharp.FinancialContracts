// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Environment
open Contract
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

        // Zero-coupon discount bond
        let zcb time amount cur = 
            let obs = NumVal(string(amount))
            Delay(time, Scale(obs, One(cur)))
        
        let c1 = (zcb 5 50.0 DKK)
        
        // European option
        //let eur = failwith "Not yet implemented"

        // American option
        //let amer = failwith "Not yet implemented"

        // If vs. Or
        let c2 = Scale(Add(Const 1.0,Mult(NumVal("one"),NumVal("two"))),One(GBP))

        let c3 = If(BoolVal("50/50"), One(DKK), Zero)

        let generateObservables t hori c : Environment.Environment = 
            let rndBool = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
            let rndNum = (fun () -> float((new Random()).NextDouble())) 

            let (bools, nums) = getObservables c
            let boolEnvAtT = List.fold (fun acc obs -> (addBoolObs (obs, rndBool()) acc)) Map.empty bools
            let numEnvAtT = List.fold (fun acc obs -> 
                                       match obs with
                                       | NumVal(s) ->
                                           (addNumObs (obs, rndNum()) acc)
                                       | _ -> failwith "Not yet implemented"
                                     ) Map.empty nums

            let newBoolEnv =
                let arr = Array.create (hori+1) Map.empty
                Array.set arr hori boolEnvAtT
                arr
            let newNumEnv =
                let arr = Array.create (hori+1) Map.empty
                Array.set arr hori numEnvAtT
                arr
            (t, newBoolEnv, newNumEnv)

        
        for i = 1 to (getHorizon c1) do
            printfn "%A" i
            let env = (generateObservables i (i + (getHorizon c1)) c1)
            printfn "%A" env
            printfn "%A" (evalC  env c1)
            //printfn "%A" (evalC env1 c2)
            Thread.Sleep(1000)

        0 // return an integer exit code
