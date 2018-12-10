// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

using CodeFormat.Rules;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sample
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) { return -1; }

            string path = args[0];
            if (Directory.Exists(path))
            {
                return 2;
            }
            else if (Path.GetExtension(path).ToLowerInvariant().Equals(".cs"))
            {
                return 1;
            }
            else
            {
                Console.WriteLine($"Error: \"{path}\" was not a directory or a C# class file.");
                return -1;
            }

            return 0;
        }
    }
}
