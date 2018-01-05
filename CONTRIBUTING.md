When contributing to the VTS please follow the coding conventions detailed below. Some of the initial development did not adhere to these guidelines but going forward, this will be the code style and structure.
## Naming Conventions
### Definitions:
* __Camel Case:__ Naming convention where the first letter of each word is capitalized with the exception of the first letter. Example: __thisIsCamelCased__
* __Pascal Case:__ Naming convention where the first letter of each work is capitalized including the first letter. Example: __ThisIsPascalCased__
* __Underscore Delimited:__ Naming convention that places underscores between the words and the first letter of each word can be capitalized. Example: __This_Is_Underscore_Delimited__<br />

These are the only naming conventions that should be used in the VTS and apply to different aspects of the code defined below.
* __Private fields__ should be prefixed with an underscore and camel cased (Note that at the start of the VTS development these fields were Pascal cased but going forward they will be camel cased).
* __Member variables__, __parameters__ and __local variables</strong> should be camel cased.
* __properties__, __functions__, __events__, __classes__ and __protected variables__ should be Pascal cased.
* __Test methods__ should be underscore delimited.
* __Interfaces__ should be prefixed with an 'I'.
## Code Ordering
Internal and external members should be grouped together in the source file and ordered as follows, __Static__ should come before __Instance__:
* Events
* Fields
* Constructors
* Properties
* Public methods
* Private methods

Note: Regions should only be used if the source file is large, comments should be used to group the members.
## Indenting
Code should be indented with 4 space characters, if a tab character is used, make sure your source-code editor replaces tabs with 4 spaces.
## Braces
An opening brace { should appear on the line after the start of a statement block and the code inside the brace should be indented 4 spaces. The closing brace } should be inline with the opening brace on it's own line.<br /> 
Example:
```c#
    if (a == 10)
    {
        //set the value of b
        b = 15;
    }
    else
    {
        b = 25;
    }
```
Braces should be used even when there is only a single line of code to avoid later issues if more code is added.<br />
In cases where a statement can be on one line, braces can begin and end on the same line.<br /> 
Example:
```c#
    string _myString
    public string MyString
    {
        get { return _myString; }
        set { _myString = value; }
    }
```
## Spacing
Spacing improves the readability of the source-code, follow these guidlines for spacing:<br /> 
Use a space after the comma between arguments in a function
```C#
    MyFunction(FirstParam, SecondParam, 10, 5);
```
Use a space after statements like __if__, __while__, __for__ and __foreach__
```c#
    if (a == 10)
    {
        //set the value of b
        b = 15;
    }
```
Use a space before and after operators with the exception of ++ and --
```c#
    for (i = 0; i &lt;= 10; i++)
    {
        Console.WriteLine(i);
    }
```
## Comments
There are 2 types of comments in the code, XML comments and regular code comments. We use the XML comments to generate our [XML Comments, click here](/VirtualPhotonics/VTS/wiki/Visual-Studio-XML-Comment-Tags.<br />
The style for the code comments is two slashes // even for multi-line comments. Avoid using /* comments */
## File Organization
There should be one public type per source file but this file can contain multiple internal classes. Source files should ave the same name as the public class in the file. Directory names should follow the namespace of the class.<br /> 
The test projects should mirror the project they are testing in both structure and naming convention. There should be one test per class and it should be named the same as the class being tested and suffixed with the word Tests.<br />
Example:<br /> 
__SolverFactory.cs__ is the name of the file for the public class SolverFactory<br /> 
__SolverFactoryTests.cs__ is the name of the file containing the tests
