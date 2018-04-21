namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Observables
open FSharp.FinancialContracts.Time

module Property =

    //Type definitions
    /// <summary> Property function type, used to define properties. </summary>
    type Property  = Environment -> TransactionResults -> bool
    /// <summary> A type representing a simple function used to determine if a transaction shold be included or excluded.</summary>
    type Filter    = (Transaction -> bool)
    /// <summary> A type representing a function that compares two values of 'a and returns a boolean. </summary>
    type BinOp<'a> = ('a -> 'a -> bool)
    
    
    //Default filters
    /// <summary> A filter that includes all transactions. </summary>
    /// <returns> A filter that always returns true. </returns>
    val allTransactions       : Filter
    /// <summary> A filter that includes all transactions of the given currency. </summary>
    /// <param name="currency"> The currency that the transactions must be equal to. </param>
    /// <returns> A filter that always returns true if the currency of the transaction match. </returns>
    val transactionsOfCurrency: currency:Currency -> Filter
    
    //Simple Properties
    /// <summary> This property is satisfied if the all the sums of the transactions of each currency are equal to 0.0. The sums are calculated that the current time. </summary>
    val isZero            : Property
    /// <summary> This property is satisfied if the all the count of transations are equal to 0. The count is calculated at the current time. </summary>
    val hasNoTransactions : Property            
    
    //Advanced properties
    /// <summary> A property comparing the sum of the transactions at current time fulfilling the filter to the constant </summary>
    /// <param name="filter"> The filter that the transactions must pass. </param>
    /// <param name="compare"> Binary operator comparing the sumOf the transactions to f. </param>
    /// <param name="f"> The value that the sum must be compared with. </param>
    /// <returns> A property which are fulfilled if the sum successfully compares with f</returns>
    val sumOf           : filter:Filter  -> compare:BinOp<float> -> f:float -> Property
    /// <summary> A property comparing the count of the transactions at current time fulfilling the filter to the constant </summary>
    /// <param name="filter"> The filter that the transactions must pass. </param>
    /// <param name="compare"> Binary operator comparing the count of the transactions to n. </param>
    /// <param name="n"> The value that the count must be compared with. </param>
    /// <returns> A property which are fulfilled if the count successfully compares with n. </returns>
    val countOf         : filter:Filter  -> compare:BinOp<int>   -> n:int   -> Property
    /// <summary> A property comparing the sum of the transactions of all times fulfilling the filter to the constant </summary>
    /// <param name="filter"> The filter that the transactions must pass. </param>
    /// <param name="compare"> Binary operator comparing the sumOf the transactions to f </param>
    /// <param name="f"> The value that the sum must be compared with</param>
    /// <returns> A property which are fulfilled if the sum successfully compares with f</returns>
    val sumOfAllTimes   : filter:Filter  -> compare:BinOp<float> -> f:float -> Property
    /// <summary> A property comparing the count of the transactions of all times fulfilling the filter to the constant </summary>
    /// <param name="filter"> The filter that the transactions must pass. </param>
    /// <param name="compare"> Binary operator comparing the count of the transactions to n </param>
    /// <param name="n"> The value that the count must be compared with</param>
    /// <returns> A property which are fulfilled if the count successfully compares with n</returns>
    val countOfAllTimes : filter:Filter  -> compare:BinOp<int>   -> n:int   -> Property
    /// <summary> A property that checks if the current time contains all the transactions in transactions </summary>
    /// <param name="transactions"> A list of transactions</param>
    /// <returns> A property that checks if the current time contains all the transactions in transactions </returns>   
    val hasTransactions : transactions: Transaction list -> Property
    
    
    //Properties For observables
    /// <summary> A property that checks if the boolean observable evaulates to true at the current time </summary>
    /// <param name="observable"> The observable that should be evaluated </param>
    /// <returns> A property that returns true if the observable is true at current time </returns>   
    val satisfyBoolObs  : observable: BoolObs  -> Property
    /// <summary> A property that compares the value of the numeric observable to the constant by the comparator. </summary>
    /// <param name="compare"> Binary operator comparing the value of the observable to the constant </param>
    /// <param name="f"> The value that the value of the observable must be compared with </param>
    /// <returns> A property that returns true if the observable value successfully compares to f</returns>
    val satisfyNumObs   : observable: NumberObs -> compare: BinOp<float> -> f: float -> Property
       
    //Simple property combinators 
    /// <summary>A property that negates the property given</summary>
    /// <param name="p">The property to be negated</param>
    /// <returns> A property that returns true if p is false</returns>
    val notProperty     : p: Property -> Property
    /// <summary>A property that combines two properties into their conjuction</summary>
    /// <param name="p1">The first property</param>
    /// <param name="p2">The second property</param>
    /// <returns> A property that returns true if both p1 and p2 are true</returns>
    val andProperty     : p1: Property -> p2: Property -> Property
    /// <summary>A property that combines two properties into their disjunction</summary>
    /// <param name="p1">The first property</param>
    /// <param name="p2">The second property</param>
    /// <returns> A property that returns true if one of p1 and p2 are true</returns>
    val orProperty      : p1: Property -> p2: Property -> Property
    /// <summary>A property that combines two properties into their implication</summary>
    /// <param name="p1">The property that first part of the implication consists of </param>
    /// <param name="p2">The second property</param>
    /// <returns> A property that returns true if p1 is false and if p1 and p2 is true</returns>
    val impliesProperty : p1: Property -> p2: Property -> Property
    //Syntactic sugar for simple property combinators
    /// <summary>Syntactic sugar for <c>notProperty</c></summary>
    val (!!)  : p: Property -> Property
    /// <summary>Syntactic sugar for <c>andProperty</c></summary>
    val (&|&) : p1: Property -> p2: Property -> Property
    /// <summary>Syntactic sugar for <c>orProperty</c></summary>
    val (|||) : p1: Property -> p2: Property -> Property
    /// <summary>Syntactic sugar for <c>impliesProperty</c></summary>
    val (=|>) : p1: Property -> p2: Property -> Property
    
    //Advanced combinators
    /// <summary>A property combinator that checks a property at a given time</summary>
    /// <param name="t">The time to increase the evaluation time with</param>
    /// <param name="p">The property to check</param>
    /// <returns>A property that are checked at a later time equal to t</returns>
    val atTime            : t: Time  -> p: Property -> Property
    /// <summary>A property combinator that checks a property is true for all times</summary>
    /// <param name="p">The property to check</param>
    /// <returns>A property that are true if p are true for all times</returns>
    val forAllTimes       : p: Property -> Property
    /// <summary>A property combinator that checks a property is true for some time</summary>
    /// <param name="p">The property to check</param>
    /// <returns>A property that are true if p are true for at least one time</returns>
    val forSomeTime       : p: Property -> Property
    /// <summary>A property combinator that checks a property is true for exactly one time</summary>
    /// <param name="p">The property to check</param>
    /// <returns>A property that are true if p are true for exactly one time</returns>
    val forOneTime        : p: Property -> Property
    
    
    

    
    
