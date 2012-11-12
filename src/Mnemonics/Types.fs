module Types

type Expression =
  | Text of string
  | FixedType
  | Variable of string * string
  | Constant of string * string
  | Scope of Expression list

let public endConstant = Constant ("END", "")

let semiColon = Text ";"