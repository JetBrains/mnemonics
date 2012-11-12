open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Xml.Serialization
open Types
open DotNet

type StringBuilder with
  member x.AppendString (s:string) = ignore <| x.Append s
  member x.AppendStrings (ss:string list) =
    for s in ss do ignore <| x.Append s


/// Renders an XML template for C#, VB.NET and F#
let renderReSharper() =
  let te = new TemplatesExport(family = "Live Templates")
  let templates = new List<TemplatesExportTemplate>()

  let printExpressions expressions (vars:List<TemplatesExportTemplateVariable>) =
    let rec impl exps (builder:StringBuilder) =
      match exps with
      | Text(txt) :: t ->
        builder.AppendString txt
        impl t builder

      | Variable(name, value) :: t ->
        let v = new TemplatesExportTemplateVariable()
        v.name <- name
        v.initialRange <- 0
        v.expression <- value
        vars.Add(v)
        builder.AppendStrings ["$"; name; "$"]
      
      | Constant(name,text) :: t ->
        let v = new TemplatesExportTemplateVariable()
        v.name <- name
        v.initialRange <- 0
        v.expression <- "constant(\"" + text + "\")"
        vars.Add(v)
        builder.AppendStrings ["$"; name; "$"]
      
      | Scope(content) :: t ->
        builder.AppendString "{"
        impl content builder
        builder.AppendString "}"
        impl t builder

      | FixedType :: t ->
        builder.AppendString "$typename$" // replaced later
        impl t builder
      
      | [] -> ()
    let sb = new StringBuilder()
    impl expressions sb
    sb.ToString();

  // first, process structures
  for (s,exprs) in cSharpStructureTemplates do
  begin
    let t = new TemplatesExportTemplate(shortcut=s)
    let vars = new List<TemplatesExportTemplateVariable>();
    t.description <- String.Empty
    t.reformat <- "True"
    t.uid <- Guid.NewGuid().ToString().ToLower()
    t.text <- printExpressions exprs vars

    t.Context <- new TemplatesExportTemplateContext()
    t.Context.CSharpContext <- new TemplatesExportTemplateContextCSharpContext
      (
        context = "TypeMember, TypeAndNamespace",
        minimumLanguageVersion = 2.0M
      )
    t.Variables <- vars.ToArray()
    templates.Add t
  end

  // now process members
  let fixTypeName (s:string) =
    s.Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
   
  for (s,doc,exprs) in cSharpMemberTemplates do
    for (tk,tv) in List.concat [dotNetPrimitiveTypeShorthands; dotNetReferenceTypeShorthands] do
      let t = new TemplatesExportTemplate(shortcut=s+tk)
      let vars = new List<TemplatesExportTemplateVariable>()
      t.description <- printExpressions doc vars
      t.reformat <- "True"
      t.shortenQualifiedReferences <- "True"

      t.text <- (printExpressions exprs vars)
                .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
                
      t.uid <- Guid.NewGuid().ToString().ToLower()

      t.Context <- new TemplatesExportTemplateContext()
      t.Context.CSharpContext  <- new TemplatesExportTemplateContextCSharpContext
        (
          context = "TypeMember",
          minimumLanguageVersion = 2.0M
        )
      t.Variables <- vars.ToArray()
      templates.Add t
    done
  done
  
  te.Template <- templates.ToArray()

  let filename = "ReSharperMnemonics.xml"
  File.Delete(filename)
  let xs = new XmlSerializer(te.GetType())
  use fs = new FileStream(filename, FileMode.Create, FileAccess.Write)
  xs.Serialize(fs, te)

/// Renders a JAR for Java, Kotlin and Scala
let renderJava() =
  ()

[<EntryPoint>]
let main argv = 
    renderReSharper()
    renderJava()
    0
