namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR
    type Transaction
    type Time = int

    type BoolObs =
        | BoolVal of string
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs

    type Environment = Time * Map<BoolObs, bool> * Map<NumberObs, float>

    val increaseTime : Environment -> Environment
    val getTime : Environment -> Time

    type Contract = 
        | Zero of float * Currency                     
        | One of Currency                               
        | Delay of Time * Contract
        | Scale of NumberObs * Contract                    
                                                        
        | And of Contract * Contract                    
        | Or of Contract * Contract                     
        | If of BoolObs * Time * Contract * Contract
        | Give of Contract 

    val getExchangeRate : Currency * Currency -> float    
    val evalC : Environment -> Contract -> Transaction list