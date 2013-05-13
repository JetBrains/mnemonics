module Types

type Expression =
  | Text of string // plain text, output as-is
  | FixedType // primary expression type, substituted
  | Variable of string * string // variable, result of LT function eval
  | Constant of string * string // constant, has a default value
  | Scope of Expression list // curly-brace-delimited scope
  | DefaultValue // default value, taken from type enumeration

let space = Text " "
let endConstant = Constant ("END", "")
let semiColon = Text ";"
let className = Constant ("CLASSNAME", "MyClass")
let interfaceName = Constant ("INTERFACENAME", "MyInterface")
let traitName = Constant("TRAITNAME", "MyTrait")
let propName = Constant("propname", "MyProperty")