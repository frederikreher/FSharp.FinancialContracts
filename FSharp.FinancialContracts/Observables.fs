namespace FSharp.FinancialContracts
open FSharp.FinancialContracts.Time

module Observables =            
    
    // Observable of type boolean.
    type BoolObs =
        | BoolVal of string
        | Bool of bool
        | And of BoolObs * BoolObs
        | Or of BoolObs * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan of NumberObs * NumberObs
        | Not of BoolObs
    // Observable of type float.
    and NumberObs =
        | NumVal of string
        | Const of float
        | Add of NumberObs * NumberObs
        | Sub of NumberObs * NumberObs
        | Mult of NumberObs * NumberObs
        | If of BoolObs * NumberObs * NumberObs
        | Average of NumberObs * Time
     

    // Identifies the observables, that the provided boolean observable depends on.
    let rec boolObs (obs:BoolObs) boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | BoolVal(_) -> 
            if not (List.contains obs boolAcc) then
                (obs::boolAcc,numAcc)
            else
                (boolAcc, numAcc)
        | Bool(_) -> (boolAcc, numAcc)
        | And(bool1, bool2) -> 
            let (boolAcc1, numAcc1) = boolObs bool1 boolAcc numAcc
            boolObs bool2 boolAcc1 numAcc1
        | Or(bool1, bool2) -> 
            let (boolAcc1, numAcc1) = boolObs bool1 boolAcc numAcc
            boolObs bool2 boolAcc1 numAcc1
        | GreaterThan(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | LessThan(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Not(bool) -> boolObs bool boolAcc numAcc
    // Identifies the observables, that the provided numerical observable depends on.
    and numberObs (obs:NumberObs) boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | NumVal(_) -> 
            if not (List.contains obs numAcc) then
                (boolAcc,obs::numAcc)
            else
                (boolAcc, numAcc)
        | Const(_) -> (boolAcc, numAcc)
        | Add(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Sub(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | Mult(num1, num2) -> 
            let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
            numberObs num2 boolAcc1 numAcc1
        | If(bool, num1, num2) -> 
            let (boolAcc1, numAcc1) = boolObs bool boolAcc numAcc
            let (boolAcc2, numAcc2) = numberObs num1 boolAcc1 numAcc1
            numberObs num2 boolAcc2 numAcc2
        | Average(num, _) -> numberObs num boolAcc numAcc
