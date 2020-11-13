// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

// Simple light switch using Functional Event Sourcing
// Jeremie Chassaing / thinkbeforecoding

// This is a basic exercise in 9 Steps.
// The code should compile at each step, and progressively run further.
// All the code is in this single file, and steps are in the top down order.
// Steps 1 to 7 are about domain and tests
// Steps 8 and 9 are about infrastructure code to run domain code

// You can run a single line or a selection of lines in F# interactive
// with Alt+Enter to test your code

// To run the program open a terminal and run
// dotnet run -- <args>
// It will build and run this program passing the args
// you can also build it :
// dotnet build -c release
// the output executable should be ./bin/release/netcoreapp3.1/light
// Usage: light (on|off|state) <file>
// on: switch light on
// off: switch light off
// state: display light state
// <file> the file containing the light events
// ex: ./light on light1

// You can learn the basics of F# syntax in fsharp.fsx

open System

// These types represent command and events from ou Event Modeling
type Command =
    | SwitchOn
    | SwitchOff

type Event =
    | SwitchedOn
    | SwitchedOff
    | Broke

// this is a simple type to represent a light state
type LightState =
    | On
    | Off

// this type represent the current state
type State = InitialState

// this is the value of the initial state
let initialState = InitialState

//-----------------------
// Domain Implementation
//-----------------------

// Step 1: 
// Implement this function with the simplest
// implementation that compile, but that take no decision
let decide (cmd: Command) (state: State) : Event list = 
    [] // this is a working implementation that does nothing

// Step 2:
// Implement this function with the simplest
// implementation that compile, but that don't evolve anything
let evolve (state: State) (event: Event) : State =
    state // we just return input state
          // so state always remains the same

//---------------------
// Tests on the Domain
//---------------------

// Step 3:
// This operator is used in tests bellow
// it takes a list of past events on the left,
// and a command to exectute on the right.
// It returns the Events resulting from the command
// Write its implementation using decide evolve and
// List.fold agg seed
// where agg is the aggregation function and seed the initial value

let (=>) (events: Event list) (cmd: Command) : Event list =
    failwith "Not implemented"

// this operator is an equality assertion 
let (==) actual expected =
    if actual = expected then
        printf "✅"
    else
        printfn "❌ %A <> %A" actual expected

// Step 4:
// Modify the decide function to make this test pass.
// Tests come from specifications, you should not change them.
let ``Switching On should switch on`` =
    []
    => SwitchOn
    == [ SwitchedOn ]

// Step 5:
// Can you make both tests pass without touching the evolve function ?
let ``Switching On twice should be idempotent`` =
    [ SwitchedOn ]
    => SwitchOn
    == [ ]

// Step 6:
// Make the next 3 test and the next pass
let ``Switching Off the first time should do nothing`` =
    [ ]
    => SwitchOff
    == [ ]

let ``Switching Off when should switch off `` =
    [ SwitchedOn ]
    => SwitchOff
    == [ SwitchedOff ]

let ``Switching On after switched Off should switch on `` =
    [ SwitchedOn
      SwitchedOff ]
    => SwitchOn
    == [ SwitchedOn ]

// Step 7:
// Now, when switching light on a 3rd time, it should break
// and do nothing afterward.
// make the remaining 3 tests pass.
// Notice how changing state should not affect existing tests.
let ``Switching On the 3rd time should break`` =
    [ SwitchedOn; SwitchedOff
      SwitchedOn; SwitchedOff ]
    => SwitchOn
    == [ Broke ]

let ``Switching On after broke should do nothing`` =
    [ Broke ]
    => SwitchOn
    == [ ]

let ``Switching Off after broke should do nothing`` =
    [ SwitchedOn; Broke ]
    => SwitchOff
    == [ ]


//--------------------
// Infrastructure
//--------------------

// these are basic serialization/deserialization functions
// for events. You'll probably use Json/Xml/Protobuf serialization
// for production code.
// It returns a list for evolution purpose...
// in the future, a single event in code can 
// return several serialized events or none
let serialize event =
    match event with
    | SwitchedOn -> ["SwitchedOn"]
    | SwitchedOff -> ["SwitchedOff"]
    | Broke -> ["Broke"]

// Same thing here, it returns a list for evolution purpose:
// when unable to deserialize, it returns an empty list
// a serialized single event can deserialize as multiple events.
let deserialize input =
    match input with
    | "SwitchedOn" -> [SwitchedOn]
    | "SwitchedOff" -> [SwitchedOff]
    | "Broke" -> [Broke]
    | _ -> []


// this function loads events from storage
// your storage will probably be an EventStore or a Database
// It reads Events for a given stream
let loadEvents filename =
    if IO.File.Exists filename then
        IO.File.ReadAllLines filename
        |> Seq.toList
        |> List.collect deserialize // concats all list returned by deserialize
    else
        []

// this function appends event to a given stream
let appendEvents filename events =
    let lines =
        events
        |> List.collect serialize // concats all list returned by serialize
    IO.File.AppendAllLines(filename, lines)


// Step 8:
// write the command handler.
// it should loads events from file,
// determine new events due to the command,
// and append them to the end of the file
let execute cmd file =
    let events = loadEvents file
    let newEvents = []
    appendEvents file newEvents
    printfn "%A" newEvents

// Step 9:
// This is a query. Display the current state 
// from stored events 
let queryState file =
    let events = loadEvents file

    printfn "%A" events

// this is the entry point for command line parsing
[<EntryPoint>]
let main argv =
    match argv with
    | [| "on" ; file |] -> execute SwitchOn file
    | [| "off"; file |] -> execute SwitchOff file
    | [| "state"; file |] -> queryState file
    | _ -> failwith "Usage: light (on|off|state) <file>"

    0 // return an integer exit code