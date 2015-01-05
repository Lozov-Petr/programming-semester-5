
module mProgram =
    
    open System
    open System.IO
    open System.Text.RegularExpressions
    open System.Collections.Generic

    ///////////////////////////////////////////////////////////////////////////////////////////
    // GRAMMAR ////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////

    type private Charaster(name : String) =

        member x.Value = name 
        
        member x.IsVariable = Char.IsUpper(name.[0])

        member x.IsTerminal = not x.IsVariable

        override x.Equals y =
            match y with
            | :? Charaster as y -> x.Value = y.Value
            | _                 -> false

        override x.GetHashCode() = name.GetHashCode()
    
        override x.ToString() = name

    ///////////////////////////////////////////////////////////////////////////////////////////
    
    type private Product(head : Charaster, tail : Charaster[]) =

        member x.Head = head
        member x.Tail = tail

        override x.Equals y =
            match y with
            | :? Product as y -> x.Head = y.Head && x.Tail = y.Tail
            | _               -> false

        override x.GetHashCode() = x.Head.GetHashCode() + x.Tail.GetHashCode()

        override x.ToString() = tail
                             |> Array.map string
                             |> String.concat " "
                             |> sprintf "%A -> %s" head

    ///////////////////////////////////////////////////////////////////////////////////////////

    type private Grammar private(axiom : Charaster, prods : Product[]) =

        member x.Axiom    = axiom
        member x.Products = prods
        
        member x.Variables =
            let set = new HashSet<Charaster>()
            axiom |> set.Add |> ignore
            
            prods |> Array.iter (fun p -> p.Head |> set.Add |> ignore
                                          p.Tail |> Array.iter (fun c -> if c.IsVariable then c |> set.Add |> ignore))
            Seq.toList set

        member x.Charasters =
            let set = new HashSet<Charaster>()
            
            prods |> Array.iter (fun p -> p.Head |> set.Add |> ignore
                                          p.Tail |> Array.iter (set.Add >> ignore))
            Seq.toList set


        static member ParseInFile(nameFile : String) =
            let reader = new StreamReader(nameFile)

            let rec read() = if reader.EndOfStream then [] else reader.ReadLine().Trim([|' '; '\t'|]) :: read()

            let split str = Regex.Split(str, "[\s\t]+")

            let createProd (terms : String list) =
                match terms with
                | t1 :: t2 :: ts when Char.IsUpper(t1.[0]) && t2 = "->" -> 
                    Some <| new Product(new Charaster(t1), Array.map (fun s -> new Charaster(s)) <| List.toArray ts)
                | _ -> None

            let createGramm (prods : Product option list) =
                if List.forall Option.isSome prods then 
                    let prods = List.map Option.get prods  
                    let axiom = if prods.IsEmpty then new Charaster("S")
                                                    else prods.Head.Head
                    Some <| new Grammar(axiom, List.toArray prods) 
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
    // Non-Deterministic Automaton ////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////

    [<AbstractClass>]
    type private NonDeterministicState(Сhildren) as x =       
        
        let children = new Dictionary<Charaster option ref, NonDeterministicState list>()
        
        do List.iter x.AddChild Сhildren
        
        member x.Children = children

        member x.AddChild(charaster : Charaster option, child : NonDeterministicState) =
            let childListRef = ref []
            let char = ref charaster
            let result = children.TryGetValue(char, childListRef)
            if result then children.[char] <- child :: !childListRef
                      else children.[char] <- [child]

    ///////////////////////////////////////////////////////////////////////////////////////////

    and private Base(var : Charaster) =
        inherit NonDeterministicState([])

        member x.Variable = var

        override x.ToString() = var.Value

        override x.Equals y = match y with
                              | :? Base as b -> var = b.Variable
                              | _            -> false

        override x.GetHashCode() = var.Value.GetHashCode()

    ///////////////////////////////////////////////////////////////////////////////////////////

    and private Situation(prod : Product, bull : int, children) =
        inherit NonDeterministicState(children)

        let mutable number = -1

        member x.Product        = prod
        member x.BulletPosition = bull
        
        member x.Number with get() = number
                        and set(n) = number <- n

        override x.ToString() =
            let tail = x.Product.Tail
            let left  = tail.[0..bull-1]           |> Array.map string |> String.Concat
            let right = tail.[bull..tail.Length-1] |> Array.map string |> String.Concat

            sprintf "%A -> %s&bull;%s" prod.Head left right

        override x.Equals y = match y with
                              | :? Situation as s -> number = s.Number
                              | _            -> false

        override x.GetHashCode() = number
       
    ///////////////////////////////////////////////////////////////////////////////////////////

    type private NonDeterministicAutomaton(grammar : Grammar) =

        let basesDict = new Dictionary<string, Base>(grammar.Variables.Length) 
        let bases = grammar.Variables |> List.map (fun v -> new Base(v))

        do bases |> List.iter (fun v -> basesDict.Add(v.Variable.Value, v))
        
        let prodToSituations (prod : Product) =
            let rec prodToSits bullPos (acc : Situation list) =
                if bullPos >= 0 then let currChar = prod.Tail.[bullPos]
                                     let mayBeChild1 = if currChar.IsVariable then [None, basesDict.[currChar.Value] :> NonDeterministicState] else [] 
                                     let children = (Some currChar, acc.Head :> NonDeterministicState) :: mayBeChild1
                                     prodToSits (bullPos - 1) <| new Situation(prod, bullPos, children) :: acc
                                else acc
            prodToSits (prod.Tail.Length - 1) [new Situation(prod, prod .Tail.Length, [])]

        
        let situationsArrays = grammar.Products |> Array.map prodToSituations

        do situationsArrays |> Array.iter (fun a -> let child = a.Head in basesDict.[child.Product.Head.Value].AddChild(None, child :> NonDeterministicState))

        let situations = List.concat situationsArrays
        
        do situations |> List.iteri (fun i s -> s.Number <- i) 

        member x.Bases = bases
        member x.Situations = situations
        member x.Grammar = grammar
        
    ///////////////////////////////////////////////////////////////////////////////////////////
    // Deterministic Automaton ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////

    type private DeterministicState(sits : Situation list) =
        
        let mutable children = []
        let mutable number = -1

        member x.Situations = sits |> List.sortBy (fun s -> s.Number)

        member x.Number with get() = number
                        and set(n) = number <- n

        member x.Children = children

        member x.AddChildren(child : Charaster * DeterministicState) =
            children <- child :: children

        override x.Equals y =
            let xs = x.Situations
            let ys = (y :?> DeterministicState).Situations
            
            xs.Length = ys.Length && List.forall2 (=) xs ys

        override x.GetHashCode() = number

    ///////////////////////////////////////////////////////////////////////////////////////////

    type private DeterministicAutimaton(nda : NonDeterministicAutomaton) =

        
        let GetChildrenWithChar(charaster : Charaster option, state : NonDeterministicState) =
            let childrenListRef = ref []
            let result = state.Children.TryGetValue(ref charaster, childrenListRef)
            if result then !childrenListRef else []

        let GetChildren(charaster : Charaster option, states : NonDeterministicState list) =
            states |> List.map (fun s -> GetChildrenWithChar(charaster, s)) |> List.concat
        
        let EpsClosing(states : NonDeterministicState list) =        
            let all = new HashSet<Situation>()

            states
            |> List.filter (fun s -> s :? Situation)
            |> List.map (fun s -> s :?> Situation)
            |> List.iter (ignore << all.Add)

            let rec epsClosing (curStates : NonDeterministicState list) =
                let newCurStates = GetChildren(None, curStates)
                let isNotLastStep = newCurStates
                                 |> List.fold (fun acc (s : NonDeterministicState) -> if s :? Situation then all.Add(s :?> Situation) || acc else true) false
                if isNotLastStep then epsClosing newCurStates

            epsClosing states

            List.ofSeq all

        let states =

            let chars = nda.Grammar.Charasters
            let first = new DeterministicState(EpsClosing [nda.Bases.Head :> NonDeterministicState])

            let rec calculateStates (next : DeterministicState list) (acc : DeterministicState list) =
                match next with
                | n::ns -> let sits = n.Situations |> List.map (fun s -> s :> NonDeterministicState)
                           let nextStates = chars
                                         |> List.map    (fun c -> c, GetChildren(Some c, sits))
                                         |> List.filter (not << List.isEmpty << snd)
                                         |> List.map    (fun (c, l) -> c, EpsClosing l)
                                         |> List.map    (fun (c, l) -> c, new DeterministicState(l))
                                         |> List.map    (fun (c, s) -> c, s, List.tryFind ((=) s) <| next @ acc)
                            
                           nextStates |> List.iter (fun (c,a,b) -> n.AddChildren(c, if b.IsSome then b.Value else a))
                           
                           let nextStates = nextStates
                                         |> List.filter (fun (_,_,b) -> b.IsNone)
                                         |> List.map    (fun (_,a,_) -> a)

                           calculateStates (nextStates @ ns) (n :: acc)

                | _     -> List.rev acc

            let states = calculateStates [first] []

            states |> List.iteri (fun i s -> s.Number <- i + 1)

            states
 
        member x.States = states

        member x.ToDot() =
            let nodes = states
                     |> List.map (fun s -> sprintf "  node_%d[label=\"%s\"];\r\n" s.Number <| String.Join("\\n", s.Situations))
                     |> String.Concat

            let item3 (_,_,a) = a
            
            let edges = states
                     |> List.map (fun s -> s.Children |> List.map (fun (c, child) -> s.Number, c, child.Number) |> List.sortBy item3)
                     |> List.concat
                     |> List.map (fun (a,b,c) -> sprintf "  node_%d -> node_%d [label=\"%s\"];\r\n" a c b.Value)
                     |> String.Concat

            sprintf "digraph G {\r\n rankdir=LR;\r\n\r\n%s\r\n%s}" nodes edges

    let GrammarToDot(pathToGrammar : String) =
        let grammar = try Grammar.ParseInFile(pathToGrammar) with _ -> None
        if grammar.IsNone then "FAIL"
        else
            let grammar = grammar.Value
            let nda = new NonDeterministicAutomaton(grammar)
            let da = new DeterministicAutimaton(nda)
            da.ToDot() 

///////////////////////////////////////////////////////////////////////////////////////////

module mMain =

    open mProgram
    open System.Collections.Generic

    [<EntryPoint>]
    let main args = 

        printf "%s" <| if args.Length <> 1 then "FAIL"
                       else GrammarToDot(args.[0])

        0

