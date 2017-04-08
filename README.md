# User guide

### What is SharpLoader?
SharpLoader is a program that opens source code files (c#), randomizes them and compiles to an .exe assembly.  
This process reults a random file signature every time.

### How to compile a SharpLoader project?
If everything is set up properly (by developer) all that user has to do is run SharpLoader.exe and wait few seconds.

### What is seed?
Seed is a unique randomization output.  
Value is random by default but it's possible to set it manually.

### How to set custom seed value?
SharpLoader accepts custom seed value as an argument.  
`SharpLoader.exe -seed 123`

# Developer guide

## How to prepare source code files?
Currently there are available 5 randomization features (that are listed below).  
Simply add proper tags to your source code.  
```c#
//-<swap>
int a = 0;
int b = 0;
int c = 0;
//-<swap/>
```

*TIP: You can use &lt;tag&gt; or //-&lt;tag&gt; if you prefer comments*

### Features:

#### 1. Swap lines
Swaps the lines.
```c#
//-<swap>
int a = 0;
int b = 0;
int c = 0;
//-<swap/>
```

#### 2. Swap blocks
Swaps the blocks.
```c#
//-<swap>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<swap/>
```

#### 3. Trash
Adds from 1 to 6 lines of trash.  
```c#
int a = 0;
//-<trash>
int b = 0;
```

#### 4. Trash +1 argument
Adds from 1 to X lines of trash.  
```c#
int a = 0;
//-<trash 5>
int b = 0;
```

#### 5. Trash +2 arguments
Adds from X to Y lines of trash.  
```c#
int a = 0;
//-<trash 4 12>
int b = 0;
```

#### 6. Flow lines
Generates random switch-case from given lines.
```c#
//-<flow>
int a = 0;
int b = 0;
int c = 0;
//-<flow/>
```

#### 7. Flow blocks
Generates random switch-case from given blocks.
```c#
//-<flow>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<flow/>
```

#### 8. Seed
Gets replaced with compilation seed value.
```c#
string seed = <seed>.ToString();
```

#### 9. Random
Generates random int value.  
```c#
int a = <rnd>;
```

#### 10. Random +1 argument
Generates random int value from 0 to X.  
```c#
int a = <rnd 25>;
```

#### 11. Random +2 arguments
Generates random int value from X to Y.  
```c#
int a = <rnd 100 200>;
```

## How to configure SharpLoader?
All SharpLoader's configuration is stored in SharpLoader.ini file.

#### Directory
Base directory for all source files.  
`Directory=MyApplication`

#### Assemblies
All references that your application use (dlls).  
Separated by ';' character.  
`Assemblies=System.dll;System.Windows.Forms.dll`

#### Sources
Paths to all source files that your application use.  
Separated by ';' character.  
`Sources=Program.cs;Utilities.cs;Enums\State.cs`

*TIP: Alternatively you can drag'n'drop source files/directories*

#### Output
Compiled assembly name.  
`Output=MyApplication`

#### AutoRun
Should assembly be run after successful compilation.  
`AutoRun=true`

#### Arguments
Compiler arguments (unsafe, prefer 32-bit etc.).  
`Arguments=/platform:anycpu32bitpreferred`
