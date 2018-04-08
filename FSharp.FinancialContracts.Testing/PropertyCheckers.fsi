﻿namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Testing.Property

module PropertyCheckers =

            
    [<Sealed>]
    type PropertyCheck = 
       static member Check       : (Contract  -> Property -> unit)
       static member CheckWithConfig : (Configuration -> Contract -> Property -> unit)