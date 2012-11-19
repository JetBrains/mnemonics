module Kotlin

(* Kotlin comes with a few ready-made templates but we stick to predefined notations here *)

open Types

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
        interfaceName
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

