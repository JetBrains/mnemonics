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

let rec pairs l = seq {  
  match l with 
  | h::t -> for e in l do yield h, e
            yield! pairs t
  | _ -> () } 

let newGuid() = Guid.NewGuid().ToString().ToLower()

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
        impl t builder

      | Constant(name,text) :: t ->
        if name <> "END" then begin
          let v = new TemplatesExportTemplateVariable()
          v.name <- name
          v.initialRange <- 0
          v.expression <- "constant(\"" + text + "\")"
          vars.Add(v)
        end
        builder.AppendStrings ["$"; name; "$"]
        impl t builder
      
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
    let t = new TemplatesExportTemplate(shortcut=s)
    let vars = new List<TemplatesExportTemplateVariable>();
    t.description <- String.Empty
    t.reformat <- "True"
    t.uid <- newGuid()
    t.text <- printExpressions exprs vars

    t.Context <- new TemplatesExportTemplateContext()
    t.Context.CSharpContext <- new TemplatesExportTemplateContextCSharpContext
      (
        context = "TypeMember, TypeAndNamespace",
        minimumLanguageVersion = 2.0M
      )
    t.Variables <- vars.ToArray()
    templates.Add t
  done

  // now process members
  for (s,doc,exprs) in cSharpMemberTemplates do
    // simple types
    for (tk,tv) in dotNetSimpleTypes do
      let t = new TemplatesExportTemplate(shortcut=(s+tk))
      let vars = new List<TemplatesExportTemplateVariable>()
      t.description <- printExpressions doc vars
      t.reformat <- "True"
      t.shortenQualifiedReferences <- "True"

      t.text <- (printExpressions exprs vars)
                .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
                
      t.uid <- newGuid()

      t.Context <- new TemplatesExportTemplateContext()
      t.Context.CSharpContext  <- new TemplatesExportTemplateContextCSharpContext
        (
          context = "TypeMember",
          minimumLanguageVersion = 2.0M
        )
      t.Variables <- vars.ToArray()
      templates.Add t
    done

    // generic types - these need additional args for the generic params
    for (gk,gv,genArgCount) in dotNetGenericTypes do
      match genArgCount with
      | 1 ->
        for (tk,tv) in dotNetSimpleTypes do
          let t0 = new TemplatesExportTemplate(shortcut=s+gk+tk)
          let vars0 = new List<TemplatesExportTemplateVariable>()
          t0.description <- (printExpressions doc vars0).Replace("$typename$", gv + "<" + tv + ">")
          t0.reformat <- "True"
          t0.shortenQualifiedReferences <- "True"

          t0.text <- (printExpressions exprs vars0).Replace("$typename$", gv + "<" + tv + ">")
          t0.uid <- newGuid()
          t0.Context <- new TemplatesExportTemplateContext()
          t0.Context.CSharpContext  <- new TemplatesExportTemplateContextCSharpContext
            (
              context = "TypeMember",
              minimumLanguageVersion = 2.0M
            )
          t0.Variables <- vars0.ToArray()
          templates.Add t0
        done
      | 2 ->
        for ((tk0,tv0),(tk1,tv1)) in pairs dotNetSimpleTypes do
          let t = new TemplatesExportTemplate(shortcut=s+gk+tk0+tk1)
          let vars = List<TemplatesExportTemplateVariable>()
          let genericArgs = gv + "<" + tv0 + "," + tv1 + ">"
          t.description <- (printExpressions doc vars).Replace("$typename$", genericArgs)
          t.reformat <- "True"
          t.shortenQualifiedReferences <- "True"

          t.text <- (printExpressions exprs vars).Replace("$typename$", genericArgs)
          t.uid <- newGuid()
          t.Context <- new TemplatesExportTemplateContext()
          t.Context.CSharpContext <- new TemplatesExportTemplateContextCSharpContext
            (
              context = "TypeMember",
              minimumLanguageVersion = 2.0M
            )
          t.Variables <- vars.ToArray()
          templates.Add t
        done
      | _ -> raise <| new Exception("We don't support this few/many args")
    done
  done
  
  te.Template <- templates.ToArray()

  let filename = "ReSharperMnemonics.xml"
  File.Delete(filename)
  let xs = new XmlSerializer(te.GetType())
  use fs = new FileStream(filename, FileMode.Create, FileAccess.Write)
  xs.Serialize(fs, te)

  printfn "%A ReSharper templates exported" (te.Template.Length)

/// Renders a JAR for Java, Kotlin and Scala
let renderJava() =
  ()

[<EntryPoint>]
let main argv = 
    renderReSharper()
    renderJava()
    Console.ReadKey() |> ignore
    0
