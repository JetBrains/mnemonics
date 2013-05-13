module Kotlin

(* Kotlin comes with a few ready-made templates but we stick to predefined notations here *)

open Types
open Java

let kotlinPrimitiveTypes =
  [
    ("c", "Character", "''")
    ("f", "Float", "0.0f")
    ("b", "Boolean", "false")
    ("by", "Byte", "0")
    ("d", "Double", "0.0")
    ("i", "Int", "0")
    ("s", "String", "\"\"")
    ("l", "Long", "0")
    ("t", "java.util.Date", "new java.util.Date()")
  ]

(* note: no static classes, interfaces are traits *)
let kotlinStructureTemplates =
  [
    (
      "c",
      [
        Text "public class "
        Constant ("CLASSNAME", "MyClass")
        Scope [
          endConstant
        ]
      ]
    )
    (
      "a",
      [
        Text "public abstract class "
        className
        Scope [ endConstant ]
      ]
    )
    (
      "i",
      [
        Text "public trait "
        traitName
        Scope [ endConstant ]
      ]
    )
    (
      "e",
      [
        Text "public enum class "
        Constant ("ENUMNAME", "MyEnum")
        Scope [ endConstant ]
      ]
    )
  ]

let kotlinMemberTemplates =
  [
    (* there are no fields *)
    (
      "m", (* same as 'fun' but no params *)
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        Text "public fun "
        Constant ("methodname", "MyMethod")
        Text "() : "
        FixedType
        space
        Scope [
          endConstant
        ]
      ]
    )
    (
      "p",
      [
        Text "A property of type "
        FixedType
      ],
      [
        Text "public var "
        propName
        Text ": "
        FixedType
        Text " = "
        DefaultValue
      ]
    )
    (
      "pr",
      [
        Text "A property of type "
        FixedType
        Text " with a private setter"
      ],
      [
        Text "public var "
        propName
        Text ": "
        FixedType
        Text " = "
        DefaultValue
        Text ideaLineBreak
        Text "private set"
      ]
    )
    (
      "pg",
      [
        Text "A property of type "
        FixedType
        Text " with an empty getter and no setter"
      ],
      [
        Text "public var "
        propName
        Text ": "
        FixedType
        Text " = "
        DefaultValue
        Text ideaLineBreak
        Text "get () "
        Scope [ endConstant ]
      ]
    )
  ]