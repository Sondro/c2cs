# C2CS

C to C# code generator. In go `.h` file, out come `.cs`.

## Background: Why?

### Problem

When creating applications with C# (especially games), it's sometimes necessary to dip down into C/C++ for better performance. However, maintaining the bindings becomes time consuming, error-prone, and in some cases quite tricky.

### Solution

Auto generate the bindings by parsing a C `.h` file, essentially transpiling the C API to C#. This includes all functions (`static`s in C#) and all types (`struct`s in C#). This is accomplished by using [libclang](https://clang.llvm.org/docs/Tooling.html) for C and [Roslyn](https://github.com/dotnet/roslyn) for C#. All naming is left as found in the header file of the C code.

## Developers: Building from Source

### Prerequisites

1. Download and install [.NET 5](https://dotnet.microsoft.com/download).
2. Clone the repository.

### Visual Studio / Rider / MonoDevelop

Open `./src/dotnet/C2CS.sln`

### Command Line Interface (CLI)

`dotnet build ./src/dotnet/C2CS.sln`

## Using C2CS

```
c2cs:
  C to C# (C2CS)

Usage:
  c2cs [options]

Options:
  -i, --inputFilePath <inputFilePath> (REQUIRED)                     File path of the input C header file.
  -o, --outputFilePath <outputFilePath> (REQUIRED)                   File path of the output C# file.
  -u, --unattended                                                   Don't ask standard input for anything. Useful when you use an automated workflow. Default value
                                                                     is to ask on standard input.
  -t, --bindingsType <Default|Delegate|DllImport|FunctionPointer>    The type of bindings to generate. Refer to the README.md file for details. Default value is
                                                                     `Default`.
  -l, --libraryName <libraryName>                                    The name of the library. Default value is the file name of the input file path.
  -s, --includeDirectories <includeDirectories>                      One or more include directories to use for parsing C code. Default value is none.
  -d, --defineMacros <defineMacros>                                  One or more macros to define for parsing C code. Default value is none.
  -a, --additionalArgs <additionalArgs>                              One or more additional arguments for parsing C code. Default value is none.
  --version                                                          Show version information
  -?, -h, --help                                                     Show help and usage information
```

### Type of bindings

Default: See `DllImport` bindings.

`DllImport`: Use static methods with the `DllImportAttribute`. Available for any C# version. The advantages of using
`DllImport` bindings are: (1) the addresses of a native exported functions are automatically resolved (happens
on initialization of the static class for the static methods marked with `DllImportAttribute`); (2) it works with any
version of C#. The disadvantages of using `DllImport` bindings are: (1) no fine control over loading the native library;
(2) no fine control over loading the exported native functions; (3) can not unload the exported native functions;
(4) can not unload the native library.

`Delegate`: Use static fields of type delegates which can be invoked where each delegate is marked with
the `UnmanagedFunctionPointerAttribute`. Available for any C# version. The advantages of using `Delegate` bindings are:
(1) fine control over loading the native library; (2) fine control over loading the exported native functions; (3)
fine control over unloading the exported native functions; (4) fine control over unloading the native library. The
disadvantages of using `Delegate` bindings are: (1) no automatic loading of the native library; (2) no automatic loading
of the exported native functions; (3) delegates are object instances which allocate memory and thus are tracked by the
Garbage Collector (GC). Two static methods, (1) `LoadApi` and (2) `UnloadApi`, are automatically generated to
help with the disadvantages. However, the `LoadApi` method still needs the path of the native library as input.

`FunctionPointer`: Use unmanaged function pointers [new to C# 9](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/function-pointers). The advantages of using `FunctionPointer` bindings are: (1) fine control over loading the native library; (2) fine control over loading the exported native functions; (3)
fine control over unloading the exported native functions; (4) fine control over unloading the native library; (5) no memory is allocated and tracked by the Garbage Collector (GC). The disadvantages of using `FunctionPointer` bindings are: (1) no automatic loading of the native library; (2) no automatic loading of the exported native functions. Two static methods, (1) `LoadApi` and (2) `UnloadApi`, are automatically generated to help with the disadvantages. However, the `LoadApi` method still needs the path of the native library as input.

## Examples

### sokol_gfx: https://github.com/floooh/sokol

Input:
```bash
-i
"/PATH/TO/sokol/sokol_gfx.h"
-o
"./sokol_gfx.cs"
-u
-l
"sokol_gfx"
```

C
```c
...

SOKOL_GFX_API_DECL void sg_begin_default_pass(const sg_pass_action* pass_action, int width, int height);
SOKOL_GFX_API_DECL void sg_begin_pass(sg_pass pass, const sg_pass_action* pass_action);
SOKOL_GFX_API_DECL void sg_apply_viewport(int x, int y, int width, int height, bool origin_top_left);
SOKOL_GFX_API_DECL void sg_apply_scissor_rect(int x, int y, int width, int height, bool origin_top_left);
SOKOL_GFX_API_DECL void sg_apply_pipeline(sg_pipeline pip);
SOKOL_GFX_API_DECL void sg_apply_bindings(const sg_bindings* bindings);
SOKOL_GFX_API_DECL void sg_apply_uniforms(sg_shader_stage stage, int ub_index, const void* data, int num_bytes);
SOKOL_GFX_API_DECL void sg_draw(int base_element, int num_elements, int num_instances);
SOKOL_GFX_API_DECL void sg_end_pass(void);
SOKOL_GFX_API_DECL void sg_commit(void);

...
```

Output:
```cs
// <auto-generated/>
// ReSharper disable All

using System.Runtime.InteropServices;

public static unsafe class sokol_gfx
{
    ...

    [DllImport(LibraryName)]
    public static extern void sg_begin_default_pass([In] sg_pass_action* pass_action, int width, int height);

    [DllImport(LibraryName)]
    public static extern void sg_begin_pass(sg_pass pass, [In] sg_pass_action* pass_action);

    [DllImport(LibraryName)]
    public static extern void sg_apply_viewport(int x, int y, int width, int height, BlittableBoolean origin_top_left);

    [DllImport(LibraryName)]
    public static extern void sg_apply_scissor_rect(int x, int y, int width, int height, BlittableBoolean origin_top_left);

    [DllImport(LibraryName)]
    public static extern void sg_apply_pipeline(sg_pipeline pip);

    [DllImport(LibraryName)]
    public static extern void sg_apply_bindings([In] sg_bindings* bindings);

    [DllImport(LibraryName)]
    public static extern void sg_apply_uniforms(sg_shader_stage stage, int ub_index, [In] void* data, int num_bytes);

    [DllImport(LibraryName)]
    public static extern void sg_draw(int base_element, int num_elements, int num_instances);

    [DllImport(LibraryName)]
    public static extern void sg_end_pass();

    [DllImport(LibraryName)]
    public static extern void sg_commit();

    ...
}    

```

### Soloud: https://github.com/jarikomppa/soloud

Input:
```bash
-i
"/PATH/TO/soloud/include/soloud_c.h"
-o
"./soloud.cs"
-u
-l
"soloud"
```

C
```c
...

void Soloud_destroy(Soloud * aSoloud);
Soloud * Soloud_create();
int Soloud_init(Soloud * aSoloud);
int Soloud_initEx(Soloud * aSoloud, unsigned int aFlags /* = Soloud::CLIP_ROUNDOFF */, unsigned int aBackend /* = Soloud::AUTO */, unsigned int aSamplerate /* = Soloud::AUTO */, unsigned int aBufferSize /* = Soloud::AUTO */, unsigned int aChannels /* = 2 */);
void Soloud_deinit(Soloud * aSoloud);

...
```

Output:
```cs
// <auto-generated/>
// ReSharper disable All

using System.Runtime.InteropServices;

public static unsafe class soloud
{
    private const string LibraryName = "soloud";

    [DllImport(LibraryName)]
    public static extern void Soloud_destroy(void* aSoloud);

    [DllImport(LibraryName)]
    public static extern void* Soloud_create();

    [DllImport(LibraryName)]
    public static extern int Soloud_init(void* aSoloud);

    [DllImport(LibraryName)]
    public static extern int Soloud_initEx(void* aSoloud, uint aFlags, uint aBackend, uint aSamplerate, uint aBufferSize, uint aChannels);

    [DllImport(LibraryName)]
    public static extern void Soloud_deinit(void* aSoloud);

    ...
}
```

## Troubleshooting

1. macOS: `fatal error: 'stdint.h' file not found` or `fatal error: 'stdbool.h' file not found`

Install CommandLineTools if you have not already:
```bash
xcode-select --install
```

Use the following as an include directory: `/Library/Developer/CommandLineTools/usr/lib/clang/12.0.0/include`

## License

C2CS is licensed under the MIT License (`MIT`). See the [LICENSE](LICENSE) file for details.
