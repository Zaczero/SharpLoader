# User guide

### What is SharpLoader?
SharpLoader is a program that opens source code files (c#), randomizes them and compiles to an .exe assembly.  
This process reults a random file signature every time.

![](http://i.imgur.com/JePqmzS.png)

### How to compile a SharpLoader project?
If everything is set up properly all you have to do is run SharpLoader.exe, select project .zip and click compile.

### What is seed?
Seed is a unique randomization output.  
Value is random by default but it's possible to set it manually.

### How to set custom seed value?
Use -seed argument.
`SharpLoader.exe -seed 123`

### How to run SharpLoader in cmd?
Use -cmd argument.
`SharpLoader.exe -cmd`

# Developer guide

## How to prepare source code files?
Currently there are available 8 randomization features (that are listed below).  
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

#### 1. Swap
```c#
//-<swap>
int a = 0;
int b = 0;
int c = 0;
//-<swap/>
```
```c#
//-<swap>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<swap/>
```

#### 2. Trash
Adds from 1 to 6 lines of trash.  
```c#
int a = 0;
//-<trash>
int b = 0;
```

#### 2.1. Trash +1 argument
Adds from 1 to X lines of trash.  
```c#
int a = 0;
//-<trash 5>
int b = 0;
```

#### 2.2. Trash +2 arguments
Adds from X to Y lines of trash.  
```c#
int a = 0;
//-<trash 4 12>
int b = 0;
```

#### 3. Flow
Generates random code flow.
```c#
//-<flow>
int a = 0;
int b = 0;
int c = 0;
//-<flow/>
```
```c#
//-<flow>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<flow/>
```

#### 4. Seed
Gets replaced with compilation seed value.
```c#
int seed = <seed>;
```

#### 5. Random
Generates random int value.  
```c#
int a = <rnd>;
```

#### 5.1. Random +1 argument
Generates random int value from 0 to X.  
```c#
int a = <rnd 25>;
```

#### 5.2. Random +2 arguments
Generates random int value from X to Y.  
```c#
int a = <rnd 100 200>;
```
#### 6. Random String
Generates random string with 8-16 length.  
```c#
string a = <rnds>;
```

#### 6.1. Random String +1 argument
Generates random string with 1-X length.  
```c#
string a = <rnds 25>;
```

#### 6.2. Random String +2 arguments
Generates random string with X-Y length.  
```c#
string a = <rnds 100 200>;
```

#### 7. Encrypt
Encrpypts value.
```c#
int a = <enc 58>;
```
```c#
char a = <enc 'C'>;
```
```c#
string a = <enc "Hello World">;
```

#### 8. Proxy
Generates proxy functions.
```c#
//-<proxy>
int a = 0;
int b = 0;
int c = 0;
//-<proxy/>
```
```c#
//-<proxy>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<proxy/>
```

#### 8.1. Proxy +1 argument
Generates proxy functions inside X namespace.
```c#
//-<proxy MyNamepsace>
int a = 0;
int b = 0;
int c = 0;
//-<proxy/>
```
```c#
//-<proxy MyNamepsace>
if (a == 0)
{ }
//-<block>
if (b == 0)
{ }
//-<proxy/>
```

#### 8.2. Proxy +1 argument
Generates proxy variables for X argument.
```c#
int a = <proxy 58>;
```
```c#
char a = <proxy 'C'>;
```
```c#
string a = <proxy "Hello World">;
```

## How to configure SharpLoader?
All SharpLoader's configuration is stored in SharpLoader.ini file.  
You can generate this file by running SharpLoader in cmd mode.

#### References
All references that your application use (dlls).  
Separated by ';' character.  
`References=System.dll;System.Windows.Forms.dll`

#### Directory
Main project directory.  
`Directory=MySources`

*TIP: Alternatively you can drag'n'drop source files/directories*

#### Output
Compiled assembly name.  
`Output=MyApplication`

#### Arguments
Compiler arguments (unsafe, prefer 32-bit etc.).  
`Arguments=/platform:anycpu32bitpreferred`

#### AutoRun
Should assembly be run after successful compilation.  
`AutoRun=true`
