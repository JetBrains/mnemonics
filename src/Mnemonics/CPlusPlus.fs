module CPlusPlus

open Types

let cppTypes =
  [
    ("b", "bool", "false")
    ("c", "char", "0")
    ("f", "float", "0.0f")
    ("d", "double", "0.0")
    ("i", "int", "0")
    ("s", "std::string", "\"\"")
    ("l", "long", "0")
    ("u", "unsigned int", "0")
  ];

let cppStructureTemplates = 
  [
    (
      "c",
      [
        Text "class "
        Constant ("CLASSNAME", "MyClass")
        Scope [ endConstant ]
        semiColon
      ]
    )
    (
      "s",
      [
        Text "struct "
        Constant ("STRUCTNAME", "MyStruct")
        Scope [ endConstant ]
        semiColon
      ]
    )
    (
      "e",
      [
        Text "enum class "
        Constant ("ENUMNAME", "MyEnum")
        Scope [ endConstant ]
        semiColon
      ]
    )
  ]

let cppMemberTemplates =
  [
    (
      "m",
      [
        Text "A method that returns a(n) "
        FixedType
      ],
      [
        FixedType
        Text " "
        Constant("methodname", "MyMethod")
        Text "()"
        space
        Scope [
          endConstant
        ]
      ]
    )
    (
      "v",
      [
        Text "A field of type "
        FixedType
      ],
      [
        FixedType
        Text " "
        Constant("fieldname", "fieldname")
        semiColon;
      ]
    )
    (
      "p",
      [
        Text "A property of type "
        FixedType
      ],
      [
        FixedType
        Text " _"
        Constant("fieldname", "fieldname")
        semiColon
        lineBreak
        
        Text "__declspec(property(get=get"
        Constant("fieldname", "fieldname")
        Text ", put=put"
        Constant("fieldname", "fieldname")
        Text ")) "
        FixedType
        Text " "
        Constant("fieldname", "fieldname")
        semiColon
        lineBreak
        
        Text "void put"
        Constant("fieldname", "fieldname")
        Text "("
        FixedType
        Text " value) "
        Scope [
          Text "_"
          Constant("fieldname", "fieldname")
          Text " = value;"
        ]
        lineBreak
        
        FixedType
        Text " get"
        Constant("fieldname", "fieldname")
        Text "() "
        Scope [
          Text "return _"
          Constant("fieldname", "fieldname")
          semiColon
        ]
      ]
    )
  ]