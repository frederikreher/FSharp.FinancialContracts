namespace FSharp.FinancialContracts

open Environment

module Contract =
    
    /// <summary> Currency used in contracts. </summary>
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    /// <summary> Evaluation of a contract result in a Transaction. </summary>
    type Transaction = Transaction of float * Currency

    /// <summary> Defines how a contract can be constructed. </summary>
    type Contract = 
        | Zero
        | One of Currency
        | Delay of Time * Contract
        | Scale of NumberObs * Contract
        | And of Contract * Contract
        | If of BoolObs * Time * Contract * Contract
        | Give of Contract 

    /// <summary> Provides the current exchange rate between two currencies. </summary>
    /// <param name="cur1"> The currency to exchange from. </param>
    /// <param name="cur2"> The currency to exchange to. </param>
    /// <returns> A float value representing the exchange rate between the two currencies. </returns>
    val getExchangeRate : Currency * Currency -> float

    /// <summary> Finds the horizon of a contract. </summary>
    /// <param name="c"> The contract to find a horizon for. </param>
    /// <returns> 
    /// A Time object representing the point in time needed to ensure that all elements 
    /// of a contract can be evaluated.
    /// </returns>
    val getHorizon : Contract -> Time

    /// <summary> Identifies all the  observables in a contract. </summary>
    /// <param name="c"> The contract to isolate observables for. </param>
    /// <returns> 
    /// Returns a tuple of BoolObs list and NumberObs list, 
    /// containing the observables needed to evaluate all elements of a contract. 
    /// </returns>
    val getObservables : Contract -> BoolObs list * NumberObs list

    /// <summary> Evaluates a contract. </summary>
    /// <param name="env"> The Environment to evaluate the contract in. </param>
    /// <param name="contract"> The contract to evaluate. </param>
    /// <returns> Returns a list of Transactions. </returns>
    val evalC : Environment -> Contract -> Transaction list