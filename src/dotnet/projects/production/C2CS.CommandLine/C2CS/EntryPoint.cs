// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace C2CS
{
    internal static class EntryPoint
    {
        private static int Main(string[] args)
        {
            var rootCommand = CreateRootCommand();
            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command CreateRootCommand()
        {
            var rootCommand = new RootCommand("C to C# (C2CS)")
            {
                Handler = CommandHandler.Create(new StartDelegate(Start))
            };

            rootCommand.AddOption(InputFilePathOption());
            rootCommand.AddOption(OutputFilePathOption());
            rootCommand.AddOption(UnattendedOption());
            rootCommand.AddOption(BindingsTypeOption());
            rootCommand.AddOption(LibraryNameOption());
            rootCommand.AddOption(IncludeDirectoriesOption());
            rootCommand.AddOption(DefineMacrosOption());
            rootCommand.AddOption(AdditionalArgsOption());

            return rootCommand;
        }

        private static Option<string> InputFilePathOption()
        {
            var aliases = new[] {"--inputFilePath", "-i"};
            var description = @"
File path of the input C header file.
".Trim();

            var option = new Option<string>(aliases, description)
            {
                IsRequired = true
            };
            return option;
        }

        private static Option<string> OutputFilePathOption()
        {
            var aliases = new[] {"--outputFilePath", "-o"};
            var description = @"
File path of the output C# file.
".Trim();

            var outputFilePathOption = new Option<string>(aliases, description)
            {
                IsRequired = true
            };

            return outputFilePathOption;
        }

        private static Option<bool> UnattendedOption()
        {
            var aliases = new[] {"--unattended", "-u"};
            var description = @"
Don't ask standard input for anything. Useful when you use an automated workflow. Default value is to ask on standard input.
".Trim();
            description = description.Replace(Environment.NewLine, string.Empty);

            var option = new Option<bool>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private static Option<BindingsType?> BindingsTypeOption()
        {
            var aliases = new[] {"--bindingsType", "-t"};
            var description = @"
The type of bindings to generate. Refer to the README.md file for details. Default value is `Default`.
".Trim();

            var option = new Option<BindingsType?>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private static Option<string?> LibraryNameOption()
        {
            var aliases = new[] {"--libraryName", "-l"};
            var description = @"
The name of the library. Default value is the file name of the input file path.
".Trim();

            var option = new Option<string?>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private static Option<IEnumerable<string>?> IncludeDirectoriesOption()
        {
            var aliases = new[] {"--includeDirectories", "-s"};
            var description = @"
One or more include directories to use for parsing C code. Default value is none.
".Trim();

            var option = new Option<IEnumerable<string>?>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private static Option<IEnumerable<string>?> DefineMacrosOption()
        {
            var aliases = new[] {"--defineMacros", "-d"};
            var description = @"
One or more macros to define for parsing C code. Default value is none.
".Trim();

            var option = new Option<IEnumerable<string>?>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private static Option<IEnumerable<string>?> AdditionalArgsOption()
        {
            var aliases = new[] {"--additionalArgs", "-a"};
            var description = @"
One or more additional arguments for parsing C code. Default value is none.
".Trim();

            var option = new Option<IEnumerable<string>?>(aliases, description)
            {
                IsRequired = false
            };
            return option;
        }

        private delegate void StartDelegate(
            string inputFilePath,
            string outputFilePath,
            BindingsType bindingsType,
            bool unattended,
            string? libraryName = null,
            IEnumerable<string>? includeDirectories = null,
            IEnumerable<string>? defineMacros = null,
            IEnumerable<string>? additionalArgs = null);

        private static void Start(
            string inputFilePath,
            string outputFilePath,
            BindingsType bindingsType,
            bool unattended,
            string? libraryName = null,
            IEnumerable<string>? includeDirectories = null,
            IEnumerable<string>? defineMacros = null,
            IEnumerable<string>? additionalArgs = null)
        {
            var programState = new Program.State(
                inputFilePath,
                outputFilePath,
                bindingsType,
                unattended,
                libraryName,
                includeDirectories,
                defineMacros,
                additionalArgs);
            var program = new Program(programState);
            program.Execute();
        }
    }
}
