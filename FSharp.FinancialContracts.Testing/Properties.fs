namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Testing.Property

module Properties =
   let sumIs target cur (ts: Transaction list) = target = (ts |> List.sumBy (fun (Transaction(v,c)) -> if c = cur then v else 0.0))
   //let sumIs target cur (ts: Transaction list) = true
   let countIs target (ts: Transaction list) = true

   let p = TransactionProperty(sumIs 40.0 DKK)
   let p2 = TransactionProperty(sumIs 20.0 EUR)

   let parent = p |&&| p2

   let s = "hej"