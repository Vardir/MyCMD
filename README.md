# MyCMD

| Branch | Version |
|:------:|:------:|
| master | 1.3.5 |

# What is the MyCMD?
**MyCMD** is simple class library you can embedd in your application if you need parsing and executing commands.

## Examples
```cmd
sum 5 5 | mul 5
sum -left 5 -right 3
summ [5, 12, 8]
```
Syntax *-left* -- parameter, *|* -- pipeline operator, *[ ]* -- array

# Installation instructions
## Visual Studio 2017 (Version 15.5 and above)
1. Run **Visual Studio**
2. Go to your project to attach the CMD to
3. Click **Add reference...** item in project's **Properties** context menu
4. Add references to core.dll, Parser.dll and FSharp.Core.dll (version 4.0 and above)
5. ...
6. To use CMD create instance of **ExecutionService**.
