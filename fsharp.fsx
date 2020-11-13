open System

// Quick F# summary:
// declare value:
let x = 0
let s = "Hello"
// declare function:
let square x = x * x

// pass result to next function:
let r =
    [ 1;2;3]
    |> List.map square // apply function to each list element
    |> List.filter (fun x -> x % 2 = 0) // keep element that pass predicate
// FSharp define scope by alignment
// an indentation starts a new scope:
let v =
    let inc x = // this is defined in v scope
        x + 1   // this is defined in inc scope
    inc 4       // this is defined in v scope

// Discriminated union
type Shape =
    | Rectangle of float * float // this case contains an int and a string
    | Circle of float                // a case without inner values
    | Point

// build:
let rect = Rectangle(16., 9.)
let circ = Circle 3.
let point = Point

// pattern matching:
let area shape =
    match shape with
    | Rectangle(width, height) -> width * height
    | Circle radius -> Math.PI * radius * radius
    | Point -> 0.

// Record
// declare
type Person =
    { Name: string
      Age: int }

// build
let john = { Name = "John"; Age = 42}

let next = { john with Age = 43 }

