open System
open System.IO
open System.Collections
open System.Collections.Generic
open System.Text.RegularExpressions

////////////////////////////////////////////////////////////////////////////////////////////////////

type HashSet<'a> with
    
    member x.Union(set : seq<'a>) = set |> Seq.fold (fun acc t -> x.Add(t) || acc) false

////////////////////////////////////////////////////////////////////////////////////////////////////

[<AbstractClass>]
type Character(name : String) =
    
    let name = if name = "\\" then "\\ " else name

    member x.Name = name

    static member Create(name : String) =
        if name.Length > 0 && Char.IsUpper(name.[0]) then new Variable(name) :> Character
                                                     else new Terminal(name) :> Character
    
    override x.Equals(y : obj) = 
        match y with
        | :? Character as y -> y.Name = name
        | _                 -> false

    override x.ToString() = name

    override x.GetHashCode() = name.GetHashCode()

////////////////////////////////////////////////////////////////////////////////////////////////////

and Terminal(name : String) =
    inherit Character(name)

    static member EndOfString = new Terminal(" $ ")

////////////////////////////////////////////////////////////////////////////////////////////////////

and Variable(name : String) = 
    inherit Character(name)

////////////////////////////////////////////////////////////////////////////////////////////////////

and SystemCharacter private(name : String) =
    inherit Character(name)

    static member OK   = new SystemCharacter("OK")   :> Character
    static member FAIL = new SystemCharacter("FAIL") :> Character
    
////////////////////////////////////////////////////////////////////////////////////////////////////

type Product(head : Variable, tail : Character list) =
    member x.Head = head
    member x.Tail = tail 
    
    member x.Variables = tail
                      |> List.filter (fun c -> c :? Variable)
                      |> List.map (fun c -> c :?> Variable)
                      |> fun l -> new HashSet<Variable>(head :: l)
                      |> List.ofSeq

    member x.Terminals = tail
                      |> List.filter (fun c -> c :? Terminal)
                      |> List.map (fun c -> c :?> Terminal)
                      |> fun l -> new HashSet<Terminal>(l)
                      |> List.ofSeq

    override x.Equals y = 
        match y with
        | :? Product as y -> y.Head = head 
                             && x.Tail.Length = y.Tail.Length
                             && List.fold2 (fun acc c1 c2 -> acc && c1 = c2) true x.Tail y.Tail
        | _               -> false

///////////////////////////////////////////////////////////////////////////////////////////////////

type Rule = Null 
          | Error 
          | P of Product 
          with

    member x.IsErr = match x with
                     | Error -> true
                     | _     -> false

    member x.IsEmpty = match x with
                       | Null -> true
                       |_     -> false

    member x.Value = match x with
                     | P p -> p

////////////////////////////////////////////////////////////////////////////////////////////////////

type Table(variables : Variable list, terminals : Terminal list, axiom : Variable) =
    
    let indexInVariable = Dictionary<Variable, int>()
    let indexInTerminal = Dictionary<Terminal, int>()
    let table = Array2D.create variables.Length (terminals.Length + 1) Null

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    do variables                         |> List.iteri (fun i v -> indexInVariable.Add(v, i))
       Terminal.EndOfString :: terminals |> List.iteri (fun i t -> indexInTerminal.Add(t, i))

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Item with get(v, t)   = table.[indexInVariable.[v], indexInTerminal.[t]]
                   and set(v, t) a = match table.[indexInVariable.[v], indexInTerminal.[t]] with
                                     | Null           -> table.[indexInVariable.[v], indexInTerminal.[t]] <- P a
                                     | P p when p = a -> ()
                                     | _              -> table.[indexInVariable.[v], indexInTerminal.[t]] <- Error

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.IsError =
        let mutable isError = false 
        for i = 0 to Array2D.length1 table - 1 do
            for j = 0 to Array2D.length2 table - 1 do
                isError <- isError || table.[i,j].IsErr

        isError

    ////////////////////////////////////////////////////////////////////////////////////////////////////
        
    member x.ParseString(str : Terminal list) = 
        if x.IsError then [SystemCharacter.FAIL]
        else
            let stack = new Stack<Character>()
            stack.Push(Terminal.EndOfString)
            stack.Push(axiom)

            let rec nextStep str acc = 
                try match str, stack.Pop() with
                    | hd::[], c when hd = Terminal.EndOfString && hd :> Character = c -> SystemCharacter.OK :: acc
                    | hd::tl, c when (c :? Terminal) && hd :> Character = c           -> nextStep tl (c :: acc)
                    | hd::tl, c when (c :? Terminal)                                  -> SystemCharacter.FAIL :: acc
                    | hd::tl, v when x.[v :?> Variable, hd].IsEmpty                   -> SystemCharacter.FAIL :: acc
                    | hd::tl, v                                                       -> let t = x.[v :?> Variable, hd].Value
                                                                                         List.iter stack.Push (List.rev t.Tail)
                                                                                         nextStep str (v :: acc)
                    | _                                                               -> SystemCharacter.FAIL :: acc
                with _ -> SystemCharacter.FAIL :: acc

            List.rev <| nextStep (str @ [Terminal.EndOfString]) [] 

////////////////////////////////////////////////////////////////////////////////////////////////////

type Grammar(axiom : Variable, products : Product list) =
    
    member x.Axiom    = axiom
    member x.Products = products

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Variables =
        products
        |> List.map (fun p -> p.Variables)
        |> List.fold (fun (acc : HashSet<Variable>) set -> acc.UnionWith(set); acc) (new HashSet<Variable>([axiom]))
        |> List.ofSeq
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Terminals =
        products
        |> List.map (fun p -> p.Terminals)
        |> List.fold (fun (acc : HashSet<Terminal>) set -> acc.UnionWith(set); acc) (new HashSet<Terminal>())
        |> List.ofSeq
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Firsts =
        let firsts = new Dictionary<Variable, bool ref * HashSet<Terminal>>()
        x.Variables |> List.iter (fun v -> firsts.Add(v, (ref false, new HashSet<Terminal>())))

        let rec updateFirsts() =
            let updateFirst (p : Product) =
                let first           = firsts.[p.Head]
                let haveEps, first' = first
   
                let rec localUF (tail : Character list) = 
                    match tail with
                    | t::ts when (t :? Terminal) -> first'.Add(t :?> Terminal)
                    | t::ts                      -> let mustAdd = firsts.[t :?> Variable]
                                                    first'.Union(snd mustAdd)
                                                    || if !(fst mustAdd) then localUF ts else false
                    | []                         -> let wasChange = not !haveEps
                                                    haveEps := true
                                                    wasChange
                localUF p.Tail

            let wasChange = products |> List.fold (fun acc p -> updateFirst p || acc) false
            if wasChange then updateFirsts()

        updateFirsts()
        firsts

    ////////////////////////////////////////////////////////////////////////////////////////////////////      

    member x.First(str : Character list) =
        let set = new HashSet<Terminal>()
        match str with
        | c::cs when (c :? Terminal) -> ignore <| set.Add(c :?> Terminal)
                                        false, set
        | c::cs                      -> let eps, mustAdd = x.Firsts.[c :?> Variable] 
                                        set.UnionWith(mustAdd)
                                        if !eps then let eps, mustAdd = x.First cs
                                                     set.UnionWith(mustAdd)
                                                     eps, set
                                                else false, set
        | _                         -> true, set

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Follows =
        let follows = new Dictionary<Variable, HashSet<Terminal>>()
        x.Variables |> List.iter (fun v -> follows.Add(v, new HashSet<Terminal>()))
        ignore <| follows.[axiom].Add(Terminal.EndOfString)

        let rec UpdateFollows() =
            let UpdateOnProduct (p : Product) =
                let rec localUOP (tail : Character list) wasChange =
                    match tail with
                    | t::ts when (t :? Terminal) -> localUOP ts wasChange
                    | t::ts                      -> let eps, mustAdd = x.First(ts)
                                                    let follow = follows.[t :?> Variable]
                                                    let wasChange = follow.Union(mustAdd) || wasChange
                                                    if eps then
                                                        let wasChange = follow.Union(follows.[p.Head]) || wasChange
                                                        localUOP ts wasChange
                                                    else
                                                        localUOP ts wasChange
                    | []                         -> wasChange
                localUOP p.Tail false

            let wasChange = products |> List.fold (fun acc p -> UpdateOnProduct p || acc) false
            if wasChange then UpdateFollows()

        UpdateFollows()
        follows

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    member x.Table =
        let table = new Table(x.Variables, x.Terminals, axiom)

        products |> List.iter (fun p -> let eps, first = x.First(p.Tail)
                                        first |> Seq.iter (fun t -> table.[p.Head, t] <- p)
                                        if eps then x.Follows.[p.Head] |> Seq.iter (fun t -> table.[p.Head, t] <- p)
                              )

        table

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    static member ParseInFile(nameFile : String) =
        let reader = new StreamReader(nameFile)

        let rec read() = if reader.EndOfStream then [] else reader.ReadLine().Trim([|' '; '\t'|]) :: read()

        let split str = Regex.Split(str, "[\s\t]+")

        let createProd (terms : String list) =
            match terms with
            | t1 :: t2 :: ts when Char.IsUpper(t1.[0]) && t2 = "->" ->  
                let head = new Variable(t1) 
                let tail = List.map Character.Create ts
                Some <| new Product(head, tail)
            | _ -> None

        let createGramm (prods : Product option list) =
            if List.forall Option.isSome prods then 
                let prods = List.map Option.get prods  
                let axiom = if prods.IsEmpty then new Variable("S")
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

////////////////////////////////////////////////////////////////////////////////////////////////////


let parseString(nameFile : String) =
     let reader = new StreamReader(nameFile)
     reader.ReadToEnd().Split([|'\n'; '\r'; '\t'; ' '|])
     |> String.concat ""
     |> fun s -> s.ToCharArray()
     |> Array.map (fun c -> new Terminal(string c))
     |> List.ofArray
     
    

[<EntryPoint>]
let main argv =
    if argv.Length = 2 then
        try let grammarOpt = Grammar.ParseInFile(argv.[0])
            if grammarOpt.IsSome then
                let grammar = grammarOpt.Value
                try let str = parseString argv.[1]
                    
                    str
                    |> grammar.Table.ParseString
                    |> List.map string
                    |> String.concat " "
                    |> printf "%s"

                with :? FileNotFoundException -> printfn "File with string not found"
            else printf "Incorrect grammar"
        with :? FileNotFoundException -> printfn "File with grammar not found"
    else printfn "Incorrect arguments"

    0


