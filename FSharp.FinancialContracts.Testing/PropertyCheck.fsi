namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module PropertyCheck =

    //Type definitions
    type Property  = (Environment -> TransactionResults -> bool)
    type BinOp<'a> = ('a -> 'a -> bool)
    type Filter    = (Transaction -> bool)
    
    //Default filters:
    val allTransactions     : Filter
    val transactionsOfAsset : Currency -> Filter
    
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
        
    //Advanced properties
    val sumOf   : Filter -> BinOp<float> -> float -> Property
    val countOf : Filter -> BinOp<int> -> int -> Property
    val satisfyBoolObs : BoolObs -> Property
    val satisfyNumObs  : NumberObs -> BinOp<float> -> float -> Property
    
    //Advanced combinators
    val atTime         : Time -> Property -> Property
    val forAllTimes    : Property -> Property
    val forSomeTime    : Property -> Property
    val isZero         : Property
    
    //And, Or, Implies, Not, IsZero, AtTime, ForAllTimes, ForSomeTime, (Satisfy BoolObs)