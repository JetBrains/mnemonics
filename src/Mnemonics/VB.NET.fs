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

let vbStructureTemplates =
  [
    (
      "c",
      [
        Text "Public Class "
        Constant ("CLASSNAME", "MyClass")
        endConstant
        Text "End Class"
      ]
    )
    (
      "a",
      [
        Text "Public MustInherit Class "
        Constant ("CLASSNAME", "MyClass")
        endConstant
        Text "End Class"
      ]
    )
    (
      "C",
      [
        Text "Public Module "
        Constant ("MODULENAME", "MyModule")
        endConstant
        Text "End Class"
      ]
    )
  ]