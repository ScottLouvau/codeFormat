// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

using CodeFormat.Rules;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeFormat
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: CodeFormat [Path to File or Folder]");
                return -1;
            }

            CSharpSyntaxRewriter[] rewriters = new CSharpSyntaxRewriter[]
            {
                new SingleLineIfBracesRule(),
                new UsingOrderRule(),
                new CopyrightCommentRule(),
            };

            string path = args[0];
            if (Directory.Exists(path))
            {
                Traverse(path, rewriters);
            }
            else if (Path.GetExtension(path).ToLowerInvariant().Equals(".cs"))
            {
                Transform(path, rewriters);
            }
            else
            {
                Console.WriteLine($"Error: \"{path}\" was not a directory or a C# class file.");
                Console.WriteLine("Usage: CodeFormat [Path to File or Folder]");
                return -1;
            }

            return 0;
        }

        static void Traverse(string directoryPath, CSharpSyntaxRewriter[] rewriters)
        {
            foreach (string filePath in Directory.GetFiles(directoryPath, "*.cs"))
            {
                Transform(filePath, rewriters);
            }

            foreach (string subdirectory in Directory.GetDirectories(directoryPath))
            {
                Traverse(subdirectory, rewriters);
            }
        }

        static void Transform(string filePath, CSharpSyntaxRewriter[] rewriters)
        {
            Console.Write($"  {filePath}");

            SourceText text = SourceText.From(File.ReadAllText(filePath));
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            CompilationUnitSyntax current = root;

            foreach (CSharpSyntaxRewriter rewriter in rewriters)
            {
                current = (CompilationUnitSyntax)current.Accept(rewriter);
            }

            if (!current.Equals(root))
            {
                Console.WriteLine("*");
                File.WriteAllText(filePath, current.ToFullString());
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}
