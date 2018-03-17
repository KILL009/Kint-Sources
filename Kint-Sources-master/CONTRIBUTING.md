> This document is kanged from [jittuu's Coding Guidelines](https://msdn.microsoft.com/en-us/library/ff926074.aspx).

# Coding Guidelines
Let's face it. No matter what coding guidelines we choose, we're not going to make everyone happy. While we would like to embrace everyone's individual style, working together on the same codebase would be utter chaos if we don't enforce some consistency. When it comes to coding guidelines, consistency can be even more important than being "right."

## Definitions

- [Camel case][] is a casing convention where the first letter is lower-case, words are not separated by any character but have their first letter capitalized. Example: `thisIsCamelCased`.
- [Pascal case][] is a casing convention where the first letter of each word is capitalized, and no separating character is included between words. Example: `ThisIsPascalCased`.

## C# coding conventions

### Tabs & Indenting
Tab characters `\0x09` should not be used in code. All indentation should be done with 4 space characters.

### Bracing
Open braces should always be at the beginning of the line after the statement that begins the block. Contents of the brace should be indented by 4 spaces. For example:
```csharp
if (someExpression)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```
Braces should never be considered optional. Even for single statement blocks, you should always use braces. This increases code readability and maintainability.
```csharp
for (int i=0; i < 100; i++) { DoSomething(i); }
```

### Single line statements
Single line statements can have braces that begin and end on the same line.

```csharp
public class Foo
{
    int bar;

    public int Bar
    {
      get { return bar; }
      set { bar = value; }
    }
}
```

### Commenting
Only comment the "**Why**" and not the "What". Jeff posted a good [blog post][jeff-comment] about it.

### Naming
- :x: DO NOT use Hungarian notation
- :white_check_mark: DO use a prefix with an underscore `_` and camelCased for private fields.
- :white_check_mark: DO use camelCasing for member variables
- :white_check_mark: DO use camelCasing for parameters
- :white_check_mark: DO use camelCasing for local variables
- :white_check_mark: DO use PascalCasing for function, property, event, and class names
- :white_check_mark: DO prefix interfaces names with “I”
- :x: DO NOT prefix enums, classes, or delegates with any letter

### Region
 - :white_check_mark: DO use region where useful.

## Roslynator
We will use [Roslynator](https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2017) to follow general coding guidelines, 
as well as a few rules set in visual studio settings, for Roslynator we included the [file](ProjectRuleset.ruleset) in our repository to automatically check for 
errors on build and analysis, [CodeMaid](http://www.codemaid.net/) config can be found [here](CodeMaid.config) as well as StyleCop settings [here](Settings.StyleCop), please utilize this extension as much as possible.

[nuget]: http://docs.nuget.org/docs/contribute/Coding-Guidelines
[ms-coding-guidelines]: http://blogs.msdn.com/b/brada/archive/2005/01/26/361363.aspx
[Camel case]: http://en.wikipedia.org/wiki/CamelCase
[Pascal case]: http://c2.com/cgi/wiki?PascalCase
[jeff-comment]: http://blog.codinghorror.com/coding-without-comments/
