namespace FSharp.FinancialContracts.Documentation

open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment

module ContractEvaluation =

    /// <summary> Evaluates a contract in a semantically simple way </summary>
    /// <param name="environment"> The Environment to evaluate the contract in. </param>
    /// <param name="contract"> The contract to evaluate. </param>
    /// <returns> Returns an list of lists of Transactions. </returns>
    val evaluateContract: environment:Environment -> contract:Contract -> Transaction list list