namespace FSharp.FinancialContracts
open FSharp.FinancialContracts.Time

module Observables =            
    
    // Observables returning a boolean.
    type BoolObs =
        | BoolVal     of string
        | Bool        of bool
        | And         of BoolObs   * BoolObs
        | Or          of BoolObs   * BoolObs
        | GreaterThan of NumberObs * NumberObs
        | LessThan    of NumberObs * NumberObs
        | Equal       of NumberObs * NumberObs
        | Not         of BoolObs
    // Observables representing Time
    and TimeObs =
        | Const       of Time
        | Add         of TimeObs * TimeObs
        | Mult        of TimeObs * TimeObs
        | If          of BoolObs * TimeObs * TimeObs
    // Observables returning a float.
    and NumberObs =
        | NumVal      of string
        | Const       of float
        | Add         of NumberObs * NumberObs
        | Sub         of NumberObs * NumberObs
        | Mult        of NumberObs * NumberObs
        | If          of BoolObs   * NumberObs * NumberObs
        | Average     of NumberObs * Time
    
        
    // Observable values pointed to by BoolVal and NumVal.
    type ObservableValue = 
        | BoolValue   of bool
        | NumberValue of float

    // Identifies the observables, that the provided boolean observable depends on.
    let rec boolObs obs boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | BoolVal(_) -> if not (List.contains obs boolAcc) then (obs::boolAcc,numAcc) 
                        else (boolAcc, numAcc)
        | Bool(_)    -> (boolAcc, numAcc)
        | Not(bool)  -> boolObs bool boolAcc numAcc
        | And(bool1, bool2) | Or(bool1, bool2) -> 
                        let (boolAcc1, numAcc1) = boolObs bool1 boolAcc numAcc
                        boolObs bool2 boolAcc1 numAcc1
        | GreaterThan(num1, num2) | LessThan(num1, num2) | Equal(num1, num2) -> 
                        let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
                        numberObs num2 boolAcc1 numAcc1
    // Identifies the observables, that the provided numerical observable depends on.
    and numberObs obs boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | NumVal(_)            -> if not (List.contains obs numAcc) then (boolAcc,obs::numAcc)
                                  else (boolAcc, numAcc)
        | Const(_)             -> (boolAcc, numAcc)
        | Average(num, _)      -> numberObs num boolAcc numAcc
        | If(bool, num1, num2) -> let (boolAcc1, numAcc1) = boolObs bool boolAcc numAcc
                                  let (boolAcc2, numAcc2) = numberObs num1 boolAcc1 numAcc1
                                  numberObs num2 boolAcc2 numAcc2
        | Add(num1, num2) | Sub(num1, num2) | Mult(num1, num2) -> 
                                  let (boolAcc1, numAcc1) = numberObs num1 boolAcc numAcc
                                  numberObs num2 boolAcc1 numAcc1
    // Identifies the observables, that the provided time observable depends on.
    and timeObs obs boolAcc numAcc : (BoolObs list * NumberObs list) =
        match obs with
        | TimeObs.Const _     -> (boolAcc,numAcc)
        | TimeObs.If(b,t1,t2) -> let (boolAcc1, numAcc1) = boolObs b boolAcc numAcc
                                 let (boolAcc2, numAcc2) = timeObs t1 boolAcc1 numAcc1
                                 timeObs t2 boolAcc2 numAcc2
        | TimeObs.Add(t1,t2)  -> let (boolAcc1, numAcc1) = timeObs t1 boolAcc numAcc
                                 timeObs t2 boolAcc1 numAcc1
        | TimeObs.Mult(t1,t2) -> let (boolAcc1, numAcc1) = timeObs t1 boolAcc numAcc
                                 timeObs t2 boolAcc1 numAcc1
