module Types

type Expression =
  | Text of string
  | FixedType
  | Variable of string * string
  | Constant of string * string
  | Scope of Expression list

let space = Text " "
let endConstant = Constant ("END", "")
let semiColon = Text ";"