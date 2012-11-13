module VB

open Types

let vbTypes =
  [
    ("b", "Boolean")
    ("c", "Char")
    ("f", "Single")
    ("by", "Byte")
    ("d", "Double")
    ("i", "Integer")
    ("m", "Decimal")
    ("s", "String")
    ("l", "Long")
    ("u", "UInteger")
    ("g", "System.Guid")
    ("t", "System.DateTime")
  ]

// note: vb structures are '
let vbStructureTemplates =
  [
    (
      "c",
      [
        Text "Public Class "
        Constant ("CLASSNAME", "MyClass")
      ]
    )
    (
      "a",
      [
        Text "Public MustInherit Class "
        Constant ("CLASSNAME", "MyClass")
      ]
    )
    (
      "C",
      [
        Text "Public Module "
        Constant ("MODULENAME", "MyModule")
      ]
    )
  ]