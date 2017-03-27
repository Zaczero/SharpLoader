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

### Randomization features:

#### 1. Lines swap
```c#
//-<swap>
int a = 0;
int b = 0;
int c = 0;
//-<swap/>
```

#### 2. Blocks swap
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
Adds from 3 to 8 lines of trash.  
```c#
int a = 0;
//-<trash>
int b = 0;
```

#### 4. Trash +1 argument
Adds X lines of trash.  
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

## How to configure SharpLoader?
All SharpLoader's configuration is stored in SharpLoader.ini file.

#### Assemblies
All references that your application use (dlls).  
Separated by ';' character.  
`Assemblies=System.dll;System.Windows.Forms.dll`

#### Sources
Paths to all source files that your application use.  
Separated by ';' character.  
`Sources=Program.cs;Utilities.cs;Enums\State.cs`

#### Output
Compiled assembly name.  
`Output=MyApplication`

#### Arguments
Compiler arguments (unsafe, prefer 32-bit etc.).  
`Arguments=/platform:anycpu32bitpreferred`
