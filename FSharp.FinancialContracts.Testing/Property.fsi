namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

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
    /// <summary> A filter that includes all transactions of the given currency. </summary>
    /// <param name="filter"> The currency that the transactions must be equal to. </param>
    /// <param name="compare"> The currency that the transactions must be equal to. </param>
    /// <param name="f"> The currency that the transactions must be equal to. </param>
    /// <returns> A filter that always returns true if the currency of the transaction match. </returns>
    val sumOf           : filter:Filter  -> compare:BinOp<float> -> f:float -> Property
    val countOf         : filter:Filter  -> compare:BinOp<int>   -> n:int   -> Property
    val satisfyBoolObs  : BoolObs  -> Property
    val satisfyNumObs   : NumberObs -> BinOp<float> -> float -> Property
    val hasTransactions : Transaction list -> Property
    
    //Simple property combinators 
    val notProperty     : Property -> Property
    val andProperty     : Property -> Property -> Property
    val orProperty      : Property -> Property -> Property
    val impliesProperty : Property -> Property -> Property
    
    //Syntactic sugar for better readability
    val (!!)  : Property -> Property
    val (&|&) : Property -> Property -> Property
    val (|||) : Property -> Property -> Property
    val (=|>) : Property -> Property -> Property
    
    //Advanced combinators
    val atTime            : Time     -> Property -> Property
    val forAllTimes       : Property -> Property
    val forSomeTime       : Property -> Property
    val forOneTime        : Property -> Property
    
    
    

    
    
