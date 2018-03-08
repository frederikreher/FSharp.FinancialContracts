namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property

module Properties =
   
   //Compares the balance of the specfied currency to the target balance
   let sumIs target cur (ts: Transaction list) : bool = (ts |> List.sumBy (fun (Transaction(v,c)) -> if c = cur then v else 0.0)) = target

   //Count the number of transactions
   let countIs target (ts: Transaction list) = (ts |> List.length) = target