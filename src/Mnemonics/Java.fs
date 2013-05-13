module Java

open Types

let entity10 = "&#10;"
let ideaLineBreak = System.Web.HttpUtility.HtmlDecode entity10

let javaPrimitiveTypes =
  [
    ("c", "char", "''")
    ("f", "float", "0.0f")
    ("b", "boolean", "false")
    ("by", "byte", "0")
    ("d", "double", "0.0")
    ("i", "int", "0")
    ("s", "String", "\"\"")
    ("l", "long", "0")
    ("t", "java.util.Date", "new java.util.Date()")
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
    (
      "C",
      [
        Text "public static class "
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
        Text "public interface "
        interfaceName
        Scope [ endConstant ]
      ]
    )
    (
      "e",
      [
        Text "public enum "
        Constant ("ENUMNAME", "MyEnum")
        Scope [ endConstant ]
      ]
    )
  ]

let javaMemberTemplates =
  [
    (
      "v",
      [
        Text "A field of type "
        FixedType
      ],
      [
        Text "private "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )
    (
      "V",
      [
        Text "A static field of type "
        FixedType
      ],
      [
        Text "private static "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        semiColon
      ]
    )
    (
      "n",
      [
        Text "A field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "private "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        Text " = "
        DefaultValue
        semiColon
      ]
    )
    (
      "m",
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        Text "public"
        space
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
    (
      "M",
      [
        Text "A static method that returns a(n) "
        FixedType
      ],
      [
        Text "public static "
        FixedType
        space
        Constant ("methodname", "MyMethod")
        Text "()"
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
        Text " with generated getter/setter methods."
      ],
      [
        Text "private "
        FixedType
        space
        propName
        semiColon

        Text "public "
        FixedType
        space
        Text "get"
        propName
        Text "()"
        Scope [
          Text "return "
          propName
          semiColon
        ]

        Text "public void set"
        propName
        Text "("
        FixedType
        space
        propName
        Text ")"
        Scope [
          Text "this."
          propName
          Text " = "
          propName
          semiColon
        ]
      ]
    )
  ]