namespace FSharp.FinancialContracts

open Environment

module Contract =
    
    /// <summary> Currency used in an asset. </summary>
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    /// <summary> Evaluation of a contract result in a Transaction. </summary>
    type Transaction = Transaction of float * Currency
    
    //Final result of the evaluation of a contract. 
    type TransactionResults = Time * Transaction list []
    
    val getTransactions : TransactionResults -> Transaction list []
        
    //TODO implement guard on increasing above length of array
    val increaseTime : TransactionResults -> Time -> TransactionResults

    /// <summary> Defines how a contract can be constructed. </summary>
    type Contract = 
        | Zero
        | One of Currency
        | Delay of Time * Contract
        | Scale of NumberObs * Contract
        | ScaleNow of NumberObs * Contract
        | And of Contract * Contract
        | If of BoolObs * Time * Contract * Contract
        | Give of Contract
    
    /// <summary> Creates a new contract equal to the conjuction of the two contracts.</summary>
    /// <param name="c1"> The first contract</param>
    /// <param name="c2"> The second contract</param>
    /// <returns> 
    /// A contract equal to And(c1,c2) 
    /// </returns>
    val (&-&): c1:Contract -> c2:Contract -> Contract
    
    /// <summary> Finds the horizon of a contract. </summary>
    /// <param name="c"> The contract to find a horizon for. </param>
    /// <returns> 
    /// A Time object representing the point in time needed to ensure that all elements 
    /// of a contract can be evaluated.
    /// </returns>
    val getHorizon: contract:Contract -> Time

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
    /// <returns> Returns an array of list of Transactions. </returns>
    val evaluateContract : Environment -> Contract -> TransactionResults