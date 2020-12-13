// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Generic;
using System.IO;
using ClangSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace C2CS
{
    public class GeneratePlatformInvokeCodeUseCase
    {
        private readonly CodeCExplorer _explorer = new CodeCExplorer();
        private readonly CodeCStructLayoutCalculator _layoutCalculator = new CodeCStructLayoutCalculator();
        private readonly CodeCSharpGenerator _codeGenerator;

        private readonly List<FieldDeclarationSyntax> _fields = new List<FieldDeclarationSyntax>();
        private readonly List<EnumDeclarationSyntax> _enums = new List<EnumDeclarationSyntax>();
        private readonly List<StructDeclarationSyntax> _structs = new List<StructDeclarationSyntax>();
        private readonly List<MethodDeclarationSyntax> _methods = new List<MethodDeclarationSyntax>();

        public GeneratePlatformInvokeCodeUseCase(string libraryName)
        {
            _codeGenerator = new CodeCSharpGenerator(libraryName);
            _explorer.EnumFound += TranspileEnum;
            _explorer.FunctionFound += TranspileFunction;
            _explorer.RecordFound += TranspileRecord;
            _explorer.TypeAliasFound += TranspileTypeAlias;
        }

        public string GenerateCode(TranslationUnit translationUnit, string libraryName)
        {
            _explorer.Explore(translationUnit);

            const string comment = @"
//-------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the following tool:
//        https://github.com/lithiumtoast/c2cs
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ReSharper disable All
//-------------------------------------------------------------------------------------

using System.Runtime.InteropServices;";
            var commentFormatted = comment.TrimStart() + "\r\n";

            var className = Path.GetFileNameWithoutExtension(libraryName);

            var @class = _codeGenerator.CreatePInvokeClass(
                    className,
                    _fields,
                    _enums,
                    _structs,
                    _methods)
                .WithLeadingTrivia(SyntaxFactory.Comment(commentFormatted));

            return @class
                .Format()
                .ToFullString();
        }

        private void TranspileRecord(RecordDecl recordC)
        {
            var isTypeForward = recordC != recordC.Definition;
            if (isTypeForward)
            {
                return;
            }

            var name = recordC.Name;
            if (string.IsNullOrEmpty(recordC.Name))
            {
                name = recordC.TypeForDecl.AsString;
            }

            var @struct = _codeGenerator.CreateStruct(name, recordC, _layoutCalculator);
            _structs.Add(@struct);
        }

        private void TranspileFunction(FunctionDecl functionC)
        {
            var method = _codeGenerator.CreateExternMethod(functionC);
            _methods.Add(method);
        }

        private void TranspileConstant(EnumConstantDecl constantC)
        {
            var constant = _codeGenerator.CreateConstant(constantC);
            _fields.Add(constant);
        }

        private void TranspileEnum(EnumDecl enumC)
        {
            var name = enumC.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = enumC.TypeForDecl.AsString;
            }

            if (string.IsNullOrEmpty(name))
            {
                foreach (var constantC in enumC.Enumerators)
                {
                    TranspileConstant(constantC);
                }
            }
            else
            {
                var @enum = _codeGenerator.CreateEnum(enumC);
                _enums.Add(@enum);
            }
        }

        private void TranspileTypeAlias(TypedefDecl typeAlias)
        {
            _codeGenerator.AddAlias(typeAlias);
        }
    }
}