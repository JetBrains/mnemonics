[![official JetBrains project](http://jb.gg/badges/official-flat-square.svg)](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub)

Mnemonics
=========
Mnemonics are templates for ReSharper and IntelliJ that let you quickly generate code and data structures by typing in names which are based on mnemonics - a structured abbreviation of the code you're trying to create.

For example, in C#, typing `c` and pressing <kbd>Tab</kbd> creates something like this:

    public class MyClass
    {
      |
    }

The vertical bar above indicates where the caret will end up once you're done editing the name of the class.

Similarly, `C` will create a static class, `m` creates a method, `M` creates a static method, and so on, including fields, variables, properties and more. You can then extend this to include the return type of the method:

* `mf` - generates a method that returns `float`
* `ms` - a method that returns `string`
* `m~s` - a method that returns an `IEnumerable<string>`

This extends to more complex examples such as:

* `pgh.sb` - a property with only a getter, of type `HashSet<StringBuilder>`

Please take a look through the templates in ReSharper or IntelliJ to see the full list. Note that currently the shortcuts that include a period (`.`) do *not* work, and are unlikely to work in the future.

Supported Languages
===================
The project aims to support the following languages - currently supported ones are in **bold**:

* **C# and VB.NET (ReSharper)**
* **Java**, Scala, **Kotlin** (IntelliJ IDEA)
* Python (PyCharm)
* Ruby (RubyMine)
* Objective-C (appCode)

Installation Notes
==================
Installing mnemonics is simple.

- Go to the `downloads` directory and download the file related to your IDE.
- **ReSharper:**
 - 8.0 and later: Look for the **mnemonics** extension package in the **ReSharper|Extension Manager**.
 - pre-8.0: Open **ReSharper|Templates Explorer**, press the **Import...** button, select the XML file. You're done. Note that on first use, VS might freeze for a while - this is normal and only happens once.
- **IntelliJ IDEA:** open **File|Import Settings...**, navigate to the directory with the `.jar` file, select it, press OK. IDEA will ask you to restart.
