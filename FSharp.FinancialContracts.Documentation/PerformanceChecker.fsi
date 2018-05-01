namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Testing.Property

module PerformanceChecker =
    val checkGeneratorPerformance : seq<int*Contract*(Contract->'b)> -> unit
    val checkPropertyPerformance  : seq<int*Contract*EnvironmentGenerator*Property> -> unit
    val checkPerformance : seq<int*Contract> -> ('a * (Environment -> Contract -> 'c)) -> ('b * (Environment -> Contract -> 'd)) -> unit