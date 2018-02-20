namespace FSharp.FinancialContracts

open System

module Contract =

    // Currency used in contracts
    type Currency = GBP | USD | DKK | None

    // Time difference
    type Days = Double

    // Defines how a contract can be constructed
    type Contract = 
      | Trade of Double * Currency          // Standard contract with an infinite horizon.
      | After of DateTime * Contract        // Standard contract with an infinite horizon from the provided date.
      | Until of DateTime * Contract        // Standard ccontract with finite horizon.
      | Zero of Double * Currency           // Contract that has no rights or obligations and has an infinite horizon.
      | One of Currency                     // Contract paying one unit of the provided currency. Has an infinite horizon.
      | Give of Contract                    // Contract giving away the provided contract. 
                                            // E.g. X acquiring c from Y, implies that Y 'give c'.
      | And of Contract * Contract          // Immediately acquire both contracts.
      | Or of Contract * Contract           // Immediately acquire either of the contracts.
      | Truncate of DateTime * Contract     // Contract that expires at the provided time, 
                                            // but have the horizion of the provided contract.
      | Then of Contract * Contract         // Acquire the first of the provided contracts unless it has expired
                                            // in which case the second contract is acquired.
      | Scale of Double * Contract          // Acquire the provided contract, but all rights and obligations 
                                            // is scaled by the provided value.
      | Get of Contract                     // Acquire the provided contract on its expiration date.
      | Anytime of Contract                 // Acquire the provided contract at anytime between the acquisition 
                                            // date of this contract and the expiration date of the provided contract.
      
    let date s = DateTime.Parse s
    let diff (d1:DateTime) (d2:DateTime) : Days = (d1.Subtract d2).TotalDays
    let add (d1:DateTime) (days:Days) = d1.AddDays days

    // Functions for creating contracts
    let trade amount what = Trade (amount, what)
    let after dt contract = After (dt, contract)
    let until dt contract = Until (dt, contract)

    let zero = Zero(0.0, None)
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
            | Zero(a, n) -> yield (a, n)
            | One(currency) -> yield (1.0, currency)
            | Give(c) -> 
                match eval c day with
                | [(a, n)] -> yield (-a, n)
                | _ -> failwith "Could not evaluate the 'Give' contract"
            | And(c1, c2) -> 
                yield! eval c1 day
                yield! eval c2 day
            | Or(c1, c2) ->
                let rand = new Random()
                if (rand.Next(0,2) = 1) then 
                    yield! eval c1 day
                else
                    yield! eval c2 day
            | Truncate(d1, c1) when day < d1 -> yield! eval c1 day
            | Then(c1, c2) -> 
                match eval c1 day with
                | [(a, n)] -> yield (a, n)
                | [] -> yield! eval c2 day
                | _ -> failwith "Could not evaluate the 'Then' contract"
            | Scale(d, c1) ->
                match eval c1 day with
                | [(a, n)] -> yield (d * a, n)
                | _ -> failwith "Could not evaluate the 'Scale' contract"
            | Get(c) ->
                match c with
                | Until(dt, c2) when dt = day -> yield! eval c2 day
                | _ -> failwith "Could not evaluate the 'Get' contract. 'Get' has to contain a 'Until' contract"
            | Anytime(c) ->
                match c with
                | Until(dt, c2) -> yield! eval c day
                | _ -> failwith "Could not evaluate the 'Anytime' contract. 'Anytime' has to contain a 'Until' contract"
            | _ -> () ]