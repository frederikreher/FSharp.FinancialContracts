namespace FSharp.FinancialContracts

open Environment

module Contract =
    
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR
    type Transaction

    type Contract = 
        | Zero
        | One of Currency
        | Delay of Time * Contract
        | Scale of NumberObs * Contract
        | And of Contract * Contract
        | Or of BoolObs * Contract * Contract
        | If of BoolObs * Time * Contract * Contract
        | Give of Contract 

    val getExchangeRate : Currency * Currency -> float
    val getHorizon : Contract -> Time
    val getObservables : Contract -> BoolObs list * NumberObs list
    val evalC : Environment -> Contract -> Transaction list