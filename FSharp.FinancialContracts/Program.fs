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
        let c2 = Scale(Add(Const 1.0,Mult(NumVal("one"),NumVal("two"))),One(GBP))

        let c3 = If(BoolVal("50/50"), One(DKK), Zero)

        let generateObservables t c = 
            let rndBool = (fun () -> (if (new Random()).Next(0,2) = 0 then true else false));
            let rndNum = (fun () -> float((new Random()).NextDouble())) 

            let (bools, nums) = getObservables c
            let newBoolEnv = List.fold (fun acc obs -> (addBoolObs (obs, rndBool()) acc)) Map.empty bools
            let newNumEnv = List.fold (fun acc obs -> 
                                       match obs with
                                       | NumVal(s) ->
                                           (addNumObs (obs, rndNum()) acc)
                                       | _ -> failwith "Not yet implemented"
                                     ) Map.empty nums
            (t, newBoolEnv, newNumEnv)

        
        for i = 1 to 10 do
            printfn "%A" i
            printfn "%A" (evalC (generateObservables i c2) c2)
            //printfn "%A" (evalC env1 c2)
            Thread.Sleep(1000)

        0 // return an integer exit code
