namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency = GBP | USD | DKK | None
    type Contract
    type Transaction
    type Time = int
    // implement arithmetic expression, get
    type Observable =
        | Get of Currency * Currency
    type Environment =
        | CurrentTime of DateTime
        | ExchangeRate of Observable

    val evalC : Environment -> Contract -> Transaction list
    val evalObs : Environment -> Observable -> float

    // Primitives for defining contracts - See Composing contracts
    val zero : Contract
    val one : Currency -> Contract
    val delay : Time -> Contract -> Contract
    val scale : Observable -> Contract -> Contract
    val _and_ : Contract -> Contract -> Contract
    val _or_ : Contract -> Contract -> Contract
    val _if_ : Observable -> Time -> Contract -> Contract -> Contract
    val give : Contract -> Contract

