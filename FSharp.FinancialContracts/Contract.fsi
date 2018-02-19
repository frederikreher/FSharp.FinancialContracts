namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency
    type Days
    type Contract

    val eval: Contract -> (int * Currency) list

    val date : String -> DateTime
    val diff : DateTime -> DateTime -> Days
    val add: DateTime -> Days -> DateTime

    val trade : int -> Currency -> Contract
    val after : DateTime -> Contract -> Contract
    val until : DateTime -> Contract -> Contract

    // Primitives for defining contracts - See Composing contracts
    val zero : Contract
    val one : Currency -> Contract
    val give : Contract -> Contract
    val _and_ : Contract -> Contract -> Contract
    val _or_ : Contract -> Contract -> Contract
    val truncate : DateTime -> Contract -> Contract
    val _then_ : Contract -> Contract -> Contract
    val scale : Double -> Contract -> Contract
    val get : Contract -> Contract
    val anytime : Contract -> Contract  

