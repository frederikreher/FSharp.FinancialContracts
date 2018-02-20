namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency
    type Days = Double
    type Contract
    type Transaction

    val eval: Contract -> DateTime -> IObservable<Transaction>

    val date : String -> DateTime
    val diff : DateTime -> DateTime -> Days
    val add : DateTime -> Days -> DateTime

    val trade : Double -> Currency -> Contract
    val after : IObservable<Boolean> -> Contract -> Contract
    val until : IObservable<Boolean> -> Contract -> Contract

    // Primitives for defining contracts - See Composing contracts
    val zero : Contract
    val one : Currency -> Contract
    val give : Contract -> Contract
    val _and_ : Contract -> Contract -> Contract
    val _or_ : Contract -> Contract -> Contract
    val truncate : IObservable<Boolean> -> Contract -> Contract
    val _then_ : Contract -> Contract -> Contract
    val scale : Double -> Contract -> Contract
    val get : Contract -> Contract
    val anytime : Contract -> Contract  

