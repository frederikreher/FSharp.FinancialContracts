namespace FSharp.FinancialContracts.Testing

open System
open FSharp.FinancialContracts.Environment
open FSharp.FinancialContracts.Contract
open FSharp.FinancialContracts.Time
open FSharp.FinancialContracts.Observables


module Generators =

    /// <summary> A function which takes time as parameter and returns and observable value. </summary>
    type ValueGenerator = System.Random -> Time -> ObservableValue

    /// <summary> Holds keys for observables which maps to corresponding valuegenerators </summary>
    type ValueGenerators = Map<string,ValueGenerator>

    /// <summary> Represents a function that takes a contract and generates a environment. </summary>
    type EnvironmentGenerator = Contract -> Environment

    /// <summary> Wrapper type for holding predefined bool generators </summary>
    [<Sealed>] 
    type BoolGenerators =

        /// <summary>Accesses a boolean generator</summary>
        /// <returns>A generator for generating random boolean values</returns>
        static member RandomBool        : ValueGenerator

        /// <summary>Accesses a value generator with chance of true or false shifted</summary>
        /// <param name="float">Parameter deciding how to shift chance. Value should between <c>0.0</c> and <c>1.0/c>, the higher value the higher chance of <c>true</c></param>
        /// <returns>A valuegenerator producing <c>true</c> or <c>false</c> at the specified chance</returns>
        static member RandomBoolShifted : (float -> ValueGenerator)

        /// <summary>Creates a value generator which returns true at a point in time</summary>
        /// <param name="Time">Parameter deciding which day the generator should return <c>true</c></param>
        /// <returns>A valuegenerator producing <c>true</c> at the specified time, and otherwise <c>false</c></returns>
        static member BoolTrueAtTime   : (Time  -> ValueGenerator)

        /// <summary>Creates a value generator which returns false at a point in time</summary>
        /// <param name="Time">Parameter deciding which day the generator should return <c>false</c></param>
        /// <returns>A valuegenerator producing <c>true</c> at the specified time, and otherwise <c>true</c></returns>
        static member BoolFalseAtTime   : (Time  -> ValueGenerator)

        /// <summary>Accesses the default value generator for boolean values. Wrapper for <c>RandomBool</c></summary>
        /// <returns>A generator for generating random boolean values</returns>
        static member Default  : ValueGenerator
    
    /// <summary> Wrapper type for holding predefined numeric generators </summary>
    [<Sealed>]
    type NumericGenerators =

       /// <summary>Accesses a random numeric generator</summary>
       /// <returns>A generator for generating random numeric values between 0.0 and 1.0</returns>
       static member RandomNumber        : ValueGenerator

       /// <summary>Creates a random numeric value generator in range</summary>
       /// <param name="min">Deciding the min of the range</param>
       /// <param name="max">Deciding the max of the range</param>
       /// <returns>A generator for generating random numeric values between min and max</returns>
       static member RandomNumberInRange : (float -> float -> ValueGenerator)

       /// <summary>Accesses the default value generator for numeric values. Wrapper for <c>RandomNumberWithinRange 1.0 10.0</c></summary>
       /// <returns>A generator for generating random numeric values between 1.0 and 10.0</returns>
       static member Default             : ValueGenerator

    /// <summary> Wrapper type for holding predefined environment generators </summary>
    [<Sealed>] 
    type EnvironmentGenerators =
        /// <summary>Accesses a environment generator</summary>
        /// <returns>A generator for generating environments with default ValueGenerators</returns>
        static member WithDefaultGenerators : EnvironmentGenerator

        /// <summary>Creates environment generator for generating environment with custom valuegenerators.</summary>
        /// <param name="genMap"> Map containing the generators for the observables. Type is ValueGenerators.</param>
        /// <returns>A generator for generating environments with custom valuegenerators. Fallback to default generators.</returns>
        static member WithCustomGenerators  : (ValueGenerators -> EnvironmentGenerator)

        /// <summary>Accesses the default valuegenerator</summary>
        /// <returns>A generator for generating environments with default valueGenerators</returns>
        static member Default               : EnvironmentGenerator

