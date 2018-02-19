namespace FSharp.FinancialContracts

open System

module Contract =

    // Currency used in contracts
    type Currency = GBP | USD | DKK

    // Time difference
    type Days = Double

    // Defines how a contract can be constructed
    type Contract = 
      | Trade of int * Currency
      | After of DateTime * Contract
      | Until of DateTime * Contract
      //| Zero of Contract
      | One of Currency
      | Give of Contract
      | And of Contract * Contract
      | Or of Contract * Contract
      | Truncate of DateTime * Contract
      | Then of Contract * Contract
      | Scale of Double * Contract
      | Get of Contract
      | Anytime of Contract
      
    let date s = DateTime.Parse s
    let diff (d1:DateTime) (d2:DateTime) : Days = (d1.Subtract d2).TotalDays
    let add (d1:DateTime) (days:Days) = d1.AddDays days

    // Functions for creating contracts
    let trade (amount, what) = Trade (amount, what)
    let after dt contract = After (dt, contract)
    let until dt contract = Until (dt, contract)

    //let zero
    let one currency = One(currency)
    let give c = Give(c)
    let _and_ c1 c2 = And(c1, c2)
    let _or_ c1 c2 = Or(c1, c2)
    let truncate d1 c1 = Truncate(d1, c1)
    let _then_ c1 c2 = Then(c1, c2)
    let scale d c1 = Scale(d, c1)
    let get c = Get(c)
    let anytime c = Anytime(c)

    // Evaluate contract on a specific day
    let rec eval contract (day:DateTime) = 
      [ match contract with
        | Trade(a, n) -> yield (a, n) 
        | After(dt, c) when day > dt -> yield! eval c day
        | Until(dt, c) when day < dt -> yield! eval c day
        | One(currency) -> yield (1, currency)
        | Give(c) -> 
            match eval c day with
            | [(a, n)] -> yield (-a, n)
            | _ -> failwith "Could not evaluate the 'Give' contract"
        | And(c1, c2) -> 
            yield! eval c1 day
            yield! eval c2 day
        | _ -> () ]