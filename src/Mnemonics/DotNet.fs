module DotNet

open Types

let dotNetSimpleTypes =
  [
    ("b", "bool")
    ("c", "char")
    ("f", "float")
    ("by", "byte")
    ("d", "double")
    ("i", "int")
    ("m", "decimal")
    ("s", "string")
    ("l", "long")
    ("u", "uint")
    ("g", "System.Guid")
    ("t", "System.DateTime")
  ]

let dotNetGenericTypes =
  [
    ("l.", "System.Collections.Generic.List", 1)
    ("h.",  "System.Collections.Generic.HashSet", 1)
    ("di.", "System.Collections.Generic.Dictionary", 2)
    ("~",  "System.Collections.Generic.IEnumerable", 1)
  ]

let cSharpStructureTemplates =
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
      "i",
      [
        Text "public interface "
        Constant ("INTERFACENAME", "MyInterface")
        Scope [ endConstant ]
      ]
    )
    (
      "s",
      [
        Text "public struct "
        Constant ("STRUCTNAME", "MyStruct")
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

let cSharpMemberTemplates =
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
      "vr",
      [
        Text "A readonly field of type "
        FixedType
      ],
      [
        Text "private "
        Constant ("type", "type")
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
      ]
    )
    (
      "o",
      [
        Text "A readonly field of type "
        FixedType
        Text " initialized to the default value."
      ],
      [
        Text "private readonly "
        FixedType
        Text " "
        Constant ("fieldname", "fieldname")
        Text " = "
      ]
    )
    (
      "t",
      [
        Text "A test method."
      ],
      [
        Text "[Test] public void "
        Constant ("methodname", "MyMethod")
        Text "()"
        Scope [
          endConstant
        ]
      ]
    )
    (
      "m",
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        Text "public void "
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
        Text "public s void "
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
        Text "An automatic property of type "
        FixedType
      ],
      [
        Text "public "
        FixedType
        space
        Constant("propname", "MyProperty")
        Text "{ get; set; }"
        endConstant
      ]
    )
    (
      "pr",
      [
        Text "An automatic property of type "
        FixedType
        Text " with a private setter"
      ],
      [
        Text "public "
        FixedType
        space
        Constant("propname", "MyProperty")
        Text "{ get; private set; }"
        endConstant
      ]
    )
    (
      "pg",
      [
        Text "An automatic property of type "
        FixedType
        Text " with an empty getter and no setter"
      ],
      [
        Text "public "
        FixedType
        Text " "
        Constant("propname", "MyProperty")
        Scope [
          Text "get "
          Scope [ endConstant ]
        ]
      ]
    )
    (
      "d",
      [
        Text "A dependency property of type "
        FixedType
        Text "."
      ],
      [
        Text "public "
        FixedType
        Text " "
        Constant("propname", "MyProperty")
        Scope [
          Text "get "
          Scope [
            Text "return ("
            FixedType
            Text ")GetValue("
            Constant("propname", "MyProperty")
            Text "Property);"
          ]
          Text "set "
          Scope [
            Text "SetValue("
            Constant("propname", "MyProperty")
            Text "Property, value);"
          ]
        ]
        Text "public static readonly System.Windows.DependencyProperty "
        Constant("propname", "MyProperty")
        Text "Property ="
      ]
    )
  ]