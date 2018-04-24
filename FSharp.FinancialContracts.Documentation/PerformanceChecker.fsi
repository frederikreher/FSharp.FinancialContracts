namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Testing.Property

module PerformanceChecker =
    val checkPerformance : seq<Contract> -> ('a * (Environment -> Contract -> unit)) -> ('b * (Environment -> Contract -> unit)) -> unit