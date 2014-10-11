module mGrammar =

    open System
    open System.IO
    open System.Text.RegularExpressions


    ///////////////////////////////////////////////////////////////////////////////////////////

    type private Charaster(name : String) =
    
        static member NewVar(index : int) = new Charaster(sprintf "V%d" index)
    
        member x.Value = name

        member x.IsVariable = Char.IsUpper(name.[0])

        member x.IsTerminal = not x.IsVariable

        member x.CompateTo (y : Charaster) = 
            if   name > y.Value then 1
            elif name < y.Value then -1
            else 0

        interface IComparable with
            member x.CompareTo y = x.CompateTo (y :?> Charaster)

        override x.Equals y =
            match y with
            | :? Charaster as y -> x.Value = y.Value
            | _                 -> false

        override x.GetHashCode() = name.GetHashCode()
    
        override x.ToString() = name

    ///////////////////////////////////////////////////////////////////////////////////////////

    type private Product(head : Charaster, tail : Charaster list) =

        member x.Head = head
        member x.Tail = tail

        member x.IsEpsilon = tail.IsEmpty

        member x.IsChain = tail.Length = 1 && tail.Head.IsVariable

        member x.IsBadChain = x.IsChain && head = tail.Head

        interface IComparable with
            member x.CompareTo y =
                let rec compareLists (tail1 : Charaster list) (tail2 : Charaster list) =
                    match tail1, tail2 with
                    | []     , []      -> 0
                    | _      , []      -> 1
                    | []     , _       -> -1
                    | t1::ts1, t2::ts2 -> let compareT = t1.CompateTo(t2)
                                          if compareT <> 0 then compareT
                                          else compareLists ts1 ts2
                
                let y = y :?> Product
                compareLists (x.Head::x.Tail) (y.Head::y.Tail)

        override x.Equals y =
            match y with
            | :? Product as y -> x.Head = y.Head && x.Tail = y.Tail
            | _               -> false

        override x.ToString() = tail
                             |> List.map string
                             |> String.concat " "
                             |> sprintf "%A -> %s" head

        override x.GetHashCode() = (x :> obj).GetHashCode()

    ///////////////////////////////////////////////////////////////////////////////////////////

    type Grammar private(axiom : Charaster, prods : Product list) =

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let contains list elem = List.exists ((=) elem) list

        ///////////////////////////////////////////////////////////////////////////////////////////
    
        static let add list elem = if contains list elem then list else elem::list

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let addMaxIndexOfVariableV(grammar : Grammar) = 
            grammar.Products
            |> List.map (fun (p : Product) -> p.Head :: p.Tail)
            |> List.concat
            |> List.filter (fun (c : Charaster) -> Regex.IsMatch(c.Value, "\AV[1-9]+\z"))
            |> List.map (fun (c : Charaster) -> c.Value.Substring 1)
            |> List.map int
            |> List.fold max -1
            |> fun n -> (grammar, n) 

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let deleteEpsilonProducts (grammar : Grammar, maxIndexV : int) = 
        
            ///////////////////////////////////////////////////////////////////////////////////////////

            let getEpsilonVariables() =
                let rec getEpsVars acc =
                    let newAcc = grammar.Products
                                |> List.filter (fun p -> List.forall (fun c -> List.exists ((=)c) acc) p.Tail)
                                |> List.map (fun p -> p.Head)
                                |> List.fold add acc
                    if newAcc.Length <> acc.Length then getEpsVars newAcc else acc
                getEpsVars []

            ///////////////////////////////////////////////////////////////////////////////////////////

            let createNewTails oldV newV tail =
                let rec splitTail left right =
                    match right with
                    | r::rs when r <> oldV -> splitTail (r::left) rs
                    | _                    -> List.rev left, right

                let rec createTails tails acc =
                    match tails with
                    | []::ts -> createTails ts acc
                    | t ::ts -> let left, right = splitTail [] t
                                if not right.IsEmpty then
                                    let t1 = left @ right.Tail
                                    let t2 = left @ (newV::right.Tail)
                                    createTails (t1::t2::ts) acc
                                else createTails ts (t::acc)
                    | _      -> acc

                createTails [tail] []

            ///////////////////////////////////////////////////////////////////////////////////////////

            let deleteEpsilonVariableInProduct oldV newV (p : Product) =
                let newHead = if p.Head = oldV then newV else p.Head
                let newTails = createNewTails oldV newV p.Tail
                List.map (fun t -> new Product(newHead, t)) newTails

            ///////////////////////////////////////////////////////////////////////////////////////////

            let deleteEpsilonProduct (prods, maxIndexV) oldV =
            
                let newMaxIndexV = maxIndexV + 1
                let newV = Charaster.NewVar(newMaxIndexV)
            
                let newProds = prods
                            |> List.map (deleteEpsilonVariableInProduct oldV newV)
                            |> List.concat
            
                let newProds = if oldV = grammar.Axiom then
                                    let p = new Product(oldV, [newV])
                                    p::newProds
                                else newProds
            
                newProds, newMaxIndexV

            ///////////////////////////////////////////////////////////////////////////////////////////
    
            let epsVars = getEpsilonVariables()
        
            let newProds, maxIndexV = List.fold deleteEpsilonProduct (grammar.Products, maxIndexV) epsVars
            let newProds = if contains epsVars grammar.Axiom then new Product(grammar.Axiom, []) :: newProds else newProds
            let newGrammar = new Grammar(grammar.Axiom, newProds)
        
            newGrammar, maxIndexV
        
        ///////////////////////////////////////////////////////////////////////////////////////////
    
        static let deleteChainProducts(grammar : Grammar, maxIndexV : int) =

            ///////////////////////////////////////////////////////////////////////////////////////////
        
            let deleteChainProd (prod : Product) (prods : Product list) =

                let rec createNewTails (paths : Charaster list list) (prods : Product list) acc =
                    match paths with
                    | (curr::prev)::ps -> 
                        let tails1, tailsN = prods
                                            |> List.filter (fun p -> p.Head = curr)
                                            |> List.map (fun p -> p.Tail)
                                            |> List.partition (fun t -> t.Length = 1 && t.Head.IsVariable)
                    
                        let newPaths = tails1
                                    |> List.map List.head
                                    |> List.filter (fun c -> not <| contains (curr::prev) c)
                                    |> List.map (fun c -> c::curr::prev)
                                    |> fun newPs -> newPs @ ps

                        let newAcc = List.fold add acc tailsN

                        createNewTails newPaths prods newAcc

                    | _ -> acc

                let newProds = if prod.IsBadChain then [] 
                                else createNewTails [[prod.Tail.Head; prod.Head]] prods []
            
                newProds
                |> List.map (fun t -> new Product(prod.Head, t))
                |> fun ps -> ps @ List.filter ((<>)prod) prods

            ///////////////////////////////////////////////////////////////////////////////////////////

            let rec deleteChainProds prods =
            
                let chainProdOpt = List.tryFind (fun (p : Product) -> p.IsChain) prods
            
                match chainProdOpt with
                | Some chainProd -> let newProds = deleteChainProd chainProd prods
                                    deleteChainProds newProds
                | _              -> prods

            let newProds = deleteChainProds grammar.Products
            let newGrammar = new Grammar(grammar.Axiom, newProds)
        
            newGrammar, maxIndexV

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let sortAndDeleteDuplicates(grammar : Grammar, maxIndexV : int) =
        
            let rec DeleteDuplicates acc prods =
                match prods with
                | p1::p2::ps -> if p1 = p2 then DeleteDuplicates acc (p2::ps)
                                else DeleteDuplicates (p1::acc) (p2::ps)
                | [p]        -> DeleteDuplicates (p::acc) []
                | _          -> List.rev acc
        
            let newProds = grammar.Products
                        |> List.sort
                        |> DeleteDuplicates [] 

            let newGrammar = new Grammar(grammar.Axiom, newProds)

            newGrammar, maxIndexV

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let deleteNonproductiveProducts(grammar : Grammar, maxIndexV : int) =
        
            ///////////////////////////////////////////////////////////////////////////////////////////

            let rec getProductiveVariables acc =
                let newAcc = grammar.Products
                            |> List.filter (fun p -> List.forall (fun (c : Charaster) -> c.IsTerminal || contains acc c) p.Tail)
                            |> List.map (fun p -> p.Head)
                            |> List.fold add acc

                if newAcc.Length <> acc.Length then getProductiveVariables newAcc else acc
        
            ///////////////////////////////////////////////////////////////////////////////////////////

            let productiveVars = getProductiveVariables []

            let newProds = grammar.Products
                        |> List.filter (fun p -> contains productiveVars p.Head)
                        |> List.filter (fun p -> List.forall (fun (c : Charaster) -> c.IsTerminal || contains productiveVars c) p.Tail)
            let newGrammar = new Grammar(grammar.Axiom, newProds)

            newGrammar, maxIndexV
          
        ///////////////////////////////////////////////////////////////////////////////////////////

        static let deleteUnattainableProducts(grammar : Grammar, maxIndexV : int) =
        
            ///////////////////////////////////////////////////////////////////////////////////////////

            let rec getAttainableVariables curr acc =
                match curr with
                | c::cs -> grammar.Products
                        |> List.filter (fun p -> p.Head = c)
                        |> List.map (fun p -> p.Tail)
                        |> List.concat
                        |> List.filter (fun c -> c.IsVariable)
                        |> List.filter (fun c -> not <| contains acc c)
                        |> fun a -> getAttainableVariables (a@cs) (c::acc)
                | _     -> acc

        
            ///////////////////////////////////////////////////////////////////////////////////////////

            let attainableVars = getAttainableVariables [grammar.Axiom] []

            let newProds = grammar.Products
                        |> List.filter (fun p -> contains attainableVars p.Head)
                        |> List.filter (fun p -> List.forall (fun (c : Charaster) -> c.IsTerminal || contains attainableVars c) p.Tail)
            let newGrammar = new Grammar(grammar.Axiom, newProds)

            newGrammar, maxIndexV          
          
        ///////////////////////////////////////////////////////////////////////////////////////////

        static let removeTerminalsAndLongTails(grammar : Grammar, maxIndexV : int) =
        
            let rec splitTail left (right : Charaster list) =
                match right with
                | r::rs when r.IsVariable -> splitTail (r::left) rs
                | _                       -> List.rev left, right
        
            let rec removeTermssAndLongTails (prods : Product list) acc maxIndexV =
                match prods with
                | p::ps when p.Tail.Length <= 1 -> removeTermssAndLongTails ps (p::acc) maxIndexV
                | p::ps -> let left, right = splitTail [] p.Tail
                           if right.IsEmpty && p.Tail.Length = 2 then removeTermssAndLongTails ps (p::acc) maxIndexV
                           else 
                                let newMaxIndexV = maxIndexV + 1
                                let newVar = Charaster.NewVar(newMaxIndexV)
                                let p1, p2 = 
                                    if not right.IsEmpty then      
                                        new Product(p.Head, left @ (newVar::right.Tail)),
                                        new Product(newVar, [right.Head])
                                    else 
                                        new Product(newVar, p.Tail.Tail),
                                        new Product(p.Head, [p.Tail.Head; newVar])

                                removeTermssAndLongTails (p1::ps) (p2::acc) newMaxIndexV
                | _     -> acc, maxIndexV

            let newProds, maxIndexV = removeTermssAndLongTails grammar.Products [] maxIndexV
            let newGrammar = new Grammar(grammar.Axiom, newProds)

            newGrammar, maxIndexV

        ///////////////////////////////////////////////////////////////////////////////////////////

        static let moveAxiomUp (grammar : Grammar) =
            let rec moveAxUp left (right : Product list) =
                match right with
                | r::rs when r.Head = grammar.Axiom -> r::(List.rev left @ rs)
                | r::rs                             -> moveAxUp (r::left) rs
                | _ -> []

            if grammar.Products.IsEmpty then grammar
            else let newProds = moveAxUp [] grammar.Products
                 let newGrammar = new Grammar(grammar.Axiom, newProds)
                 newGrammar

        ///////////////////////////////////////////////////////////////////////////////////////////

        member private x.Axiom = axiom

        ///////////////////////////////////////////////////////////////////////////////////////////

        member private x.Products = prods

        ///////////////////////////////////////////////////////////////////////////////////////////

        static member ParseInFile(nameFile : String) =
            let reader = new StreamReader(nameFile)

            let rec read() = if reader.EndOfStream then [] else reader.ReadLine().Trim([|' '; '\t'|]) :: read()

            let split str = Regex.Split(str, "[\s\t]+")

            let createProd (terms : String list) =
                match terms with
                | t1 :: t2 :: ts when Char.IsUpper(t1.[0]) && t2 = "->" -> 
                    Some <| new Product(new Charaster(t1), List.map (fun s -> new Charaster(s)) ts)
                | _ -> None

            let createGramm (prods : Product option list) =
                if List.forall Option.isSome prods then 
                    let prods = List.map Option.get prods  
                    let axiom = if prods.IsEmpty then new Charaster("S")
                                                    else prods.Head.Head
                    Some <| new Grammar(axiom, prods) 
                else None 

            let grammar = read()
                        |> List.filter (fun s -> s.Length <> 0)
                        |> List.map split
                        |> List.map List.ofArray
                        |> List.map createProd
                        |> createGramm

            reader.Close()
            grammar

        ///////////////////////////////////////////////////////////////////////////////////////////

        member grammar.Normalize() =
            grammar
            |> addMaxIndexOfVariableV
            |> deleteEpsilonProducts
            |> deleteChainProducts
            |> sortAndDeleteDuplicates
            |> deleteNonproductiveProducts
            |> deleteUnattainableProducts
            |> removeTerminalsAndLongTails
            |> sortAndDeleteDuplicates
            |> fst
            |> moveAxiomUp


        ///////////////////////////////////////////////////////////////////////////////////////////

        override x.ToString() = if prods.IsEmpty then "FAIL"
                                else prods
                                    |> List.map string
                                    |> String.concat "\r\n"




///////////////////////////////////////////////////////////////////////////////////////////

module mMain =

    open mGrammar

    open System.IO

    ///////////////////////////////////////////////////////////////////////////////////////////

    [<EntryPoint>]
    let main argv = 
        if argv.Length <> 1 then printfn "Incorrect arguments"
        else try let gramOpt = Grammar.ParseInFile argv.[0] 
                 let newGrammar = gramOpt.Value.Normalize()
                 printfn "%A" newGrammar
             with
             | :? FileNotFoundException -> printfn "File not found"
        0

    ///////////////////////////////////////////////////////////////////////////////////////////
