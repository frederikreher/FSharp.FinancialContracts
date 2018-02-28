namespace FSharp.FinancialContracts

open System

module Contract =
    
    type Currency = GBP | USD | DKK | None
    type Transaction
    type Time = int
    type Observable = 
        | Number of string
        | Bool of string
    
    type ObservableValue = 
        | NumberValue of float
        | BoolValue of bool

    type Environment = Time * Map<Observable,ObservableValue>

    type Contract = 
        | Zero of float * Currency                     
        | One of Currency                               
        | Delay of Time * Contract
        | Scale of Observable * Contract                    
                                                        
        | And of Contract * Contract                    
        | Or of Contract * Contract                     
        | If of Observable * Time * Contract * Contract
        | Give of Contract 
    
    val evalC : Environment -> Contract -> Transaction list
    val increaseTime : Environment -> Environment
    val getTime : Environment -> Time
    val getExchangeRate : Currency * Currency -> float