module Java

open Types

let javaPrimitiveTypes =
  [
    ("c", "char")
    ("f", "float")
    ("by", "byte")
    ("d", "double")
    ("i", "int")
    ("s", "String")
    ("l", "long")
    ("t", "java.lang.Date")
  ]

let javaGenericTypes =
  [
    ("l.", "java.util.ArrayList", 1)
    ("h.", "java.util.HasSet", 1)
    ("di.", "java.util.HashMap", 2)
    ("~", "java.lang.Iterable", 1) // <-- somewhat unnecessary, unlike in .NET
  ]

let javaStructureTemplates =
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
  ]