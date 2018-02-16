namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency
    type Contract

    val eval: Contract -> (int * Currency) list

    val trade : int -> Currency -> Contract
    val after : DateTime -> Contract -> Contract
    val until : DateTime -> Contract -> Contract
    val ($) : Contract -> Contract -> Contract
