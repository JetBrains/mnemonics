module Python
open Types

(* Python doesn't benefit so much from mnemonics because it is already very concise *)

let pythonStructureTemplates =
  [
    (
      "c",
      [
        Text "class "
        Constant ("CLASSNAME", "MyClass")
        Text ":"
      ]
    )
  ]