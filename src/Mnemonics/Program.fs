open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text
open System.Xml.Serialization
open Ionic.Zip
open Types
open DotNet
open CSharp
open VB
open Java
open Kotlin

let version = "0.5"

type StringBuilder with
  member x.AppendString (s:string) = ignore <| x.Append s
  member x.AppendStrings (ss:string list) =
    for s in ss do ignore <| x.Append s

let rec pairs l = seq {
  for a in l do
    for b in l do 
      yield (a,b)
  }

let newGuid() = Guid.NewGuid().ToString().ToLower()

/// Renders an XML template for C#, VB.NET and F#
let renderReSharper() =
  let te = new TemplatesExport(family = "Live Templates")
  let templates = new List<TemplatesExportTemplate>()

  // debugging switches :)
  let renderCSharp, renderVBNET = true, true

  let printExpressions expressions (vars:List<TemplatesExportTemplateVariable>) defValue =
    let rec impl exps (builder:StringBuilder) =
      match exps with
      | Text(txt) :: t ->
        builder.AppendString txt
        impl t builder

      | DefaultValue :: t ->
        builder.AppendString defValue
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
          if not(vars.Any(fun v' -> v.name.Equals(v'.name))) then vars.Add(v)
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
  if renderCSharp then
    for (s,exprs) in cSharpStructureTemplates do
      let t = new TemplatesExportTemplate(shortcut=s)
      let vars = new List<TemplatesExportTemplateVariable>()
      t.description <- String.Empty
      t.reformat <- "True"
      t.uid <- newGuid()
      t.text <- printExpressions exprs vars String.Empty

      t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
      t.Variables <- vars.ToArray()
      templates.Add t
    done

  if renderVBNET then
    for (s,exprs) in vbStructureTemplates do
      let t = new TemplatesExportTemplate(shortcut=s)
      let vars = new List<TemplatesExportTemplateVariable>()
      t.description <- String.Empty
      t.reformat <- "False" // critical difference with C#!!!
      t.uid <- newGuid()
      t.text <- printExpressions exprs vars String.Empty
      t.Context <- new TemplatesExportTemplateContext(VBContext = vbContext)
      t.Variables <- vars.ToArray()
      templates.Add t
    done

  // now process members
  if renderCSharp then
    for (s,doc,exprs) in cSharpMemberTemplates do
      // simple types; methods can be void
      let types = (if Char.ToLower(s.Chars(0)) ='m' then ("", "void", "") :: csharpTypes else csharpTypes)
      for (tk,tv,defValue) in types do
        let t = new TemplatesExportTemplate(shortcut=(s+tk))
        let vars = new List<TemplatesExportTemplateVariable>()
        t.description <- printExpressions doc vars defValue
        t.reformat <- "True"
        t.shortenQualifiedReferences <- "True"
        t.text <- (printExpressions exprs vars defValue)
                  .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
        t.uid <- newGuid()
        t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
        t.Variables <- vars.ToArray()
        templates.Add t
      done

      // generically specialized types
      for (gk,gv,genArgCount) in dotNetGenericTypes do
        match genArgCount with
        | 1 ->
          for (tk,tv,_) in csharpTypes do
            let t0 = new TemplatesExportTemplate(shortcut=s+gk+tk)
            let vars0 = new List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "<" + tv + ">"
            let defValue = "new " + genericArgs + "()"
            t0.description <- (printExpressions doc vars0 defValue).Replace("$typename$", genericArgs)
            t0.reformat <- "True"
            t0.shortenQualifiedReferences <- "True"
            t0.text <- (printExpressions exprs vars0 defValue).Replace("$typename$", genericArgs)
            t0.uid <- newGuid()
            t0.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
            t0.Variables <- vars0.ToArray()
            templates.Add t0
          done
        | 2 -> // maybe this is not such a good idea because we get n^2 templates
          for ((tk0,tv0,_),(tk1,tv1,_)) in pairs csharpTypes do
            let t = new TemplatesExportTemplate(shortcut=s+gk+tk0+tk1)
            let vars = List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "<" + tv0 + "," + tv1 + ">"
            let defValue = "new " + genericArgs + "()"
            t.description <- (printExpressions doc vars defValue).Replace("$typename$", genericArgs)
            t.reformat <- "True"
            t.shortenQualifiedReferences <- "True"
            t.text <- (printExpressions exprs vars defValue).Replace("$typename$", genericArgs)
            t.uid <- newGuid()
            t.Context <- new TemplatesExportTemplateContext(CSharpContext = csContext)
            t.Variables <- vars.ToArray()
            templates.Add t
          done
        | _ -> raise <| new Exception("We don't support this few/many args")
      done
    done

  if renderVBNET then
    for (s,doc,exprs) in vbMemberTemplates do
      // simple types; methods can be void
      for (tk,tv,defValue) in vbTypes do
        let t = new TemplatesExportTemplate(shortcut=(s+tk))
        let vars = new List<TemplatesExportTemplateVariable>()
        t.description <- printExpressions doc vars defValue
        t.reformat <- "True"
        t.shortenQualifiedReferences <- "True"
        t.text <- (printExpressions exprs vars defValue)
                  .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
        t.uid <- newGuid()
        t.Context <- new TemplatesExportTemplateContext(VBContext = vbContext)
        t.Variables <- vars.ToArray()
        templates.Add t
      done

      // generically specialized types
      for (gk,gv,genArgCount) in dotNetGenericTypes do
        match genArgCount with
        | 1 ->
          for (tk,tv,_) in vbTypes do
            let t0 = new TemplatesExportTemplate(shortcut=s+gk+tk)
            let vars0 = new List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "(Of " + tv + ")"
            let defValue = "new " + genericArgs + "()"
            t0.description <- (printExpressions doc vars0 defValue).Replace("$typename$", genericArgs)
            t0.reformat <- "True"
            t0.shortenQualifiedReferences <- "True"
            t0.text <- (printExpressions exprs vars0 defValue).Replace("$typename$", genericArgs)
            t0.uid <- newGuid()
            t0.Context <- new TemplatesExportTemplateContext(VBContext = vbContext)
            t0.Variables <- vars0.ToArray()
            templates.Add t0
          done
        | 2 -> // maybe this is not such a good idea because we get n^2 templates
          for ((tk0,tv0,_),(tk1,tv1,_)) in pairs vbTypes do
            let t = new TemplatesExportTemplate(shortcut=s+gk+tk0+tk1)
            let vars = List<TemplatesExportTemplateVariable>()
            let genericArgs = gv + "(Of " + tv0 + ", Of" + tv1 + ")"
            let defValue = "new " + genericArgs + "()"
            t.description <- (printExpressions doc vars defValue).Replace("$typename$", genericArgs)
            t.reformat <- "True"
            t.shortenQualifiedReferences <- "True"
            t.text <- (printExpressions exprs vars defValue).Replace("$typename$", genericArgs)
            t.uid <- newGuid()
            t.Context <- new TemplatesExportTemplateContext(VBContext = vbContext)
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
  let javaDeclContext =
    [| new templateSetTemplateOption(name="JAVA_DECLARATION",value=true) |]
    
  let kotlinDeclContext =
    [| new templateSetTemplateOption(name="KOTLIN_EXPRESSION",value=true) |]


  let printExpressions expressions (vars:List<templateSetTemplateVariable>) defValue =
    let rec impl exps (builder:StringBuilder) =
      match exps with
      | Text(txt) :: t ->
        builder.AppendString txt
        impl t builder

      | DefaultValue :: t ->
        builder.AppendString defValue
        impl t builder

      | Variable(name, value) :: t ->
        let v = new templateSetTemplateVariable()
        v.name <- name
        v.expression <- value
        v.alwaysStopAt <- true
        vars.Add(v)
        if not (vars.Any(fun v' -> v.name.Equals(v'.name))) then vars.Add(v)
        builder.AppendStrings ["$"; name; "$"]
        impl t builder

      | Constant(name,text) :: t ->
        if name <> "END" then begin
          let v = new templateSetTemplateVariable()
          v.name <- name
          v.defaultValue <- "\"" + text + "\"" // note the quotes
          v.expression <- String.Empty
          v.alwaysStopAt <- true
          if not (vars.Any(fun v' -> v.name.Equals(v'.name))) then vars.Add(v)
        end
        builder.AppendStrings ["$"; name; "$"]
        impl t builder
      
      | Scope(content) :: t ->
        builder.AppendStrings [ideaLineBreak; "{"; ideaLineBreak]
        impl content builder
        builder.AppendStrings [ideaLineBreak; "}"]
        impl t builder

      | FixedType :: t ->
        builder.AppendString "$typename$" // replaced later
        impl t builder
      
      | [] -> ()
    let sb = new StringBuilder()
    impl expressions sb
    sb.ToString();


  // this saves the template set under a filename
  let saveFile filename ts =
    let xs = new XmlSerializer(ts.GetType())
    use sw = new StringWriter()
    xs.Serialize(sw, ts)
    let textToWrite = sw.ToString().Replace("&#xA;", entity10) // .NET knows better :)
    File.WriteAllText(filename, textToWrite)

  Directory.CreateDirectory(".\\jar") |> ignore
  Directory.CreateDirectory(".\\jar\\templates") |> ignore

  (***************** JAVA **********************************************)
  let ts = new templateSet()
  let templates = new List<templateSetTemplate>()
  ts.group <- "mnemnics-java" // todo: investigate 'properietary' groups
  let filename = ".\\jar\\templates\\" + ts.group + ".xml"

  // java structures
  for (s, exprs) in javaStructureTemplates do
    let t = new templateSetTemplate(name=s)
    let vars = new List<templateSetTemplateVariable>()
    t.description <- String.Empty
    t.toReformat <- true
    t.toShortenFQNames <- true
    t.context <- javaDeclContext
    t.value <- (printExpressions exprs vars String.Empty)
    t.variable <- vars.ToArray()
    templates.Add t
  done

  // java members
  for (s, doc, exprs) in javaMemberTemplates do
    // simple types; methods can be void
    let types = if Char.ToLower(s.Chars(0)) = 'm' 
                then ("", "void", "") :: javaPrimitiveTypes
                else javaPrimitiveTypes
    for (tk,tv,defValue) in types do
      let t = new templateSetTemplate()
      let vars = new List<templateSetTemplateVariable>()
      t.name <- s + tk
      t.description <- (printExpressions doc vars defValue)
                        .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
      t.toReformat <- true
      t.toShortenFQNames <- true
      t.context <- javaDeclContext
      t.value <- (printExpressions exprs vars defValue)
                  .Replace("$typename$", if String.IsNullOrEmpty(tv) then "void" else tv)
      t.variable <- vars.ToArray()
      templates.Add t
    done
  done

  ts.template <- templates.ToArray()
  saveFile filename ts

  (***************** KOTLIN ********************************************)
  let ts = new templateSet()
  let templates = new List<templateSetTemplate>()
  ts.group <- "mnemnics-kotlin" // todo: investigate 'properietary' groups
  let filename = ".\\jar\\templates\\" + ts.group + ".xml"

  // structures
  for (s, exprs) in kotlinStructureTemplates do
    let t = new templateSetTemplate(name=s)
    let vars = new List<templateSetTemplateVariable>()
    t.description <- String.Empty
    t.toReformat <- true
    t.toShortenFQNames <- true
    t.context <- kotlinDeclContext
    t.value <- (printExpressions exprs vars String.Empty)
    t.variable <- vars.ToArray()
    templates.Add t
  done

  // members
  for (s, doc, exprs) in kotlinMemberTemplates do
    // simple types; methods can be void
    let types = if Char.ToLower(s.Chars(0)) = 'm' 
                then ("", "Unit", "") :: kotlinPrimitiveTypes
                else kotlinPrimitiveTypes
    for (tk,tv,defValue) in types do
      let t = new templateSetTemplate()
      let vars = new List<templateSetTemplateVariable>()
      t.name <- s + tk
      t.description <- (printExpressions doc vars defValue)
                        .Replace("$typename$", if String.IsNullOrEmpty(tv) then "Unit" else tv)
      t.toReformat <- true
      t.toShortenFQNames <- true
      t.context <- kotlinDeclContext
      t.value <- (printExpressions exprs vars defValue)
                  .Replace("$typename$", if String.IsNullOrEmpty(tv) then "Unit" else tv)
      t.variable <- vars.ToArray()
      templates.Add t
    done
  done

  ts.template <- templates.ToArray()
  saveFile filename ts

  // TODO: java and kotlin generics
  let ideaFileName = "IntelliJ IDEA Global Settings"
  File.WriteAllText(".\\jar\\" + ideaFileName, String.Empty)

  // now wrap it in a jar. use of 3rd-party zipper unavoidable
  let jarFileName = "IdeaMnemonics.jar"
  File.Delete jarFileName
  let jarFile = new ZipFile(jarFileName)
  let templatesDir = jarFile.AddDirectory(".\\jar")
  jarFile.Save()
  printfn "%A IDEA templates exported" templates.Count

[<EntryPoint>]
let main argv = 
    renderReSharper()
    renderJava()
    //Console.ReadKey() |> ignore
    0
