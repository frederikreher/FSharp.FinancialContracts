﻿namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module Properties =
    val sumIs : float -> Asset -> Transaction list -> bool
    val countIs : int -> Transaction list -> bool
