Mnemonics
=========
Mnemonics are shortcuts that let you quickly create data structures by typing in names which are mnemonic (of similar sound) to what you're creating.

For example, typing `c` and pressing <kbd>Tab</kbd> creates something like this:

    public class MyClass
    {
      |
    }

The vertical bar above indicates where the caret will end up once you're done editing the name of the class.

Supported Languages
===================
This project aims to support the following programming languages:

* C# and VB.NET (ReSharper)
* Java, Scala, Kotlin (IntelliJ IDEA)
* Python (PyCharm)
* Ruby (RubyMine)
* Objective-C (appCode)

Note: Java and Kotlin appear to conflict as per [this issue](http://youtrack.jetbrains.com/issue/IDEA-100302).

Installation Notes
==================
Installing mnemonics is simple.

- Go to the `downloads` directory and download the file related to your IDE.
- **ReSharper**: open **ReSharper|Templates Explorer**, press the **Import...** button, select the XML file. You're done. Note that on first use, VS might freeze for a while - this is normal and only happens once.
- **IntelliJ IDEA:** open **File|Import Settings...**, navigate to the directory with the `.jar` file, select it, press OK. IDEA will ask you to restart.