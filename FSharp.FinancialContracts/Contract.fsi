namespace FSharp.FinancialContracts

open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables

module Contract =
    
    /// <summary> Currency used in an Transaction. </summary>
    type Currency = 
        | USD | JPY | BGN | CZK | DKK | GBP | HUF | PLN | RON | SEK 
        | CHF | ISK | NOK | HRK | RUB | TRY | AUD | BRL | CAD | CNY 
        | HKD | IDR | ILS | INR | KRW | MXN | MYR | NZD | PHP | SGD
        | THB | ZAR | EUR

    /// <summary> 
    /// Transaction represents the amount of a currency the holding party receives or gives.
    /// </summary>
    type Transaction = Transaction of float * Currency
    
    ///  <summary>
    /// The evaluation of a contract results in TransactionResults representing the current time 
    /// and array of current and future transactions. 
    ///  </summary>
    type TransactionResults = Time * Transaction list []
    
    /// <summary> Gets the transactions in TransactionResults.</summary>
    /// <param name="trs"> The TransactionResults element to retrieve the transactions from. </param>
    /// <returns> The transactions in the TransactionResults element. </returns>
    val getTransactions: trs:TransactionResults -> Transaction list []
        
    /// <summary> Increases the current time of an TransactionResults element. </summary>
    /// <param name="tsr"> The TransactionResults element to increase the time of. </param>
    /// <param name="t"> The amount of time the current time should be increased by. </param>
    /// <returns> An TransactionResults element with the current time increased. </returns>
    val increaseTime: tsr:TransactionResults -> t:Time -> TransactionResults

    /// <summary> Defines the contract combinators used to construct a contract. </summary>
    type Contract = 
        | Zero
        | One      of Currency
        | Delay    of Time      * Contract
        | Scale    of NumberObs * Contract
        | ScaleNow of NumberObs * Contract
        | And      of Contract  * Contract
        | If       of BoolObs   * Time     * Contract * Contract
        | Give     of Contract
    
    /// <summary> Creates a new contract equal to the conjuction of the two provided contracts. </summary>
    /// <param name="c1"> The first contract. </param>
    /// <param name="c2"> The second contract. </param>
    /// <returns> A contract equal to And(c1,c2). </returns>
    val (&-&): c1:Contract -> c2:Contract -> Contract
    
    /// <summary> Finds the horizon of a contract. </summary>
    /// <param name="contract"> The contract to find a horizon for. </param>
    /// <returns> 
    /// A Time object representing the point in time needed to ensure that all elements 
    /// of a contract can be evaluated.
    /// </returns>
    val getHorizon: contract:Contract -> Time

    /// <summary> Identifies all the observables in a contract. </summary>
    /// <param name="contract"> The contract to isolate observables for. </param>
    /// <returns> 
    /// Returns a tuple of BoolObs list and NumberObs list, 
    /// containing the observables needed to evaluate all elements of a contract. 
    /// </returns>
    val getObservables: contract:Contract -> BoolObs list * NumberObs list

    /// <summary> Evaluates a contract. </summary>
    /// <param name="env"> The Environment to evaluate the contract in. </param>
    /// <param name="contract"> The contract to evaluate. </param>
    /// <returns> 
    /// Returns an array of lists of Transactions, with each list representing the transaction 
    /// of a specific day.
    /// </returns>
    val evaluateContract : env:Environment -> contract:Contract -> TransactionResults