// Learn more about F# at http://fsharp.org
namespace FSharp.FinancialContracts

open System.Threading
open Environment
open Contract
open System

module program = 
    let f = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));

    [<EntryPoint>]
    let main argv =
        let boolObservables = [(BoolVal("true"), true);
                               (BoolVal("50/50"), f());
                               (BoolVal("false"), false)]
        let numObservables = [(NumVal("GBP/DKK"), getExchangeRate(GBP, DKK));
                              (NumVal("USD/DKK"), getExchangeRate(USD, DKK));
                              (NumVal("100"), 100.0)]

        // Add function to add to environments???
        let mutable env1: Environment.Environment = 0, (boolObservables |> Map.ofList), (numObservables |> Map.ofList)

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
        let c2 = Or(BoolVal("50/50"),Delay(10,One(DKK)), One(EUR))

        let c3 = If(BoolVal("50/50"), One(DKK), Zero)

        let addObservables c env = 
            let f = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
            match env with
            | (t,boolEnv,numEnv) ->
                let (bools, nums) = getObservables c
                let newBoolEnv = List.fold (fun acc obs -> (addBoolObs (obs, f()) acc)) boolEnv bools
                let newNumEnv = List.fold (fun acc obs -> 
                                            match obs with
                                            | NumVal(s) ->
                                                (addNumObs (obs, float(s)) acc)
                                            | _ -> failwith "Not yet implemented"
                                          ) numEnv nums
                (t, newBoolEnv, newNumEnv)
        
        for i = 1 to 10 do
            env1 <- increaseTime env1
            printfn "%A" i
            printfn "%A" (evalC (addObservables c3 env1) c3)
            printfn "%A" (evalC env1 c2)
            Thread.Sleep(1000)

        0 // return an integer exit code
