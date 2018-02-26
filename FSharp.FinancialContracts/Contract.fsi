namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency = GBP | USD | DKK | None
    type Contract
    type Transaction
    type Time = float
    // implement arithmetic expression, get
    type Observable =
        | Get of Currency
        | Bool of Boolean
    //type Environment

    val evalC : Map<String,float> -> Contract -> Transaction list
    val evalObs : Map<String,float> -> Observable -> float

    // Primitives for defining contracts - See Composing contracts
    val zero : Contract
    val one : Currency -> Contract
    val delay : Time -> Contract -> Contract
    val scale : Observable -> Contract -> Contract
    val _and_ : Contract -> Contract -> Contract
    val _or_ : Contract -> Contract -> Contract
    val _if_ : Observable -> Time -> Contract -> Contract -> Contract
    val give : Contract -> Contract

