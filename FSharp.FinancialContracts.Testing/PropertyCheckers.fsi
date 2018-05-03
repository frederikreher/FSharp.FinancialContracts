namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Testing.Generators
open FSharp.FinancialContracts.Testing.PropertyCheckerInternal
open FSharp.FinancialContracts.Testing.Property

module PropertyCheckers =
    
    /// <summary> A type containing an implemenation of the <c>PropertyCheckerInternal </c>. Both runs checksuites and both raises exceptions if unsuccessfull. </summary>
    [<Sealed>]
    type PropertyCheck = 
        /// <summary>Checks the given contract with the property with the default generators and the default configuration </summary>
        static member Check           : (Contract  -> Property -> unit)
        /// <summary>Checks the given contract with the property with custom check suite configuration </summary>
        static member CheckWithConfig : (Configuration -> Contract -> Property -> unit)
        /// <summary>Checks the given contract with the property with custom check suite configuration and returns the test data from the internal function</summary>
        static member CheckAndReturnData           : (Contract -> Property -> TestData)
        /// <summary>Checks the given contract with the property with custom check suite configuration and returns the test data from the internal function</summary>
        static member CheckAndReturnDataWithConfig : (Configuration -> Contract -> Property -> TestData)     