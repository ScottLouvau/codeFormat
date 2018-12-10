// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeFormat.Rules
{
    /// <summary>
    ///  Sort usings alphabetically with an empty line between each different top level namespace and System usings first.
    /// </summary>
    public class UsingOrderRule : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            // Extract usings outside any namespace
            var rootUsings = node.Usings;
            if (node.Usings == null || node.Usings.Count == 0) { return node; }

            // Get the trivia before the first using (any top-level comment)
            var firstUsingTrivia = rootUsings.First().GetLeadingTrivia();

            // Sort them alphabetically but with System usings first
            var sortedUsings = node.Usings.OrderBy((u) => u.Name.ToFullString(), new UsingSorter());

            // Set whitespace to put one empty line between each top-level namespace
            var whitespaceCorrectedUsings = SyntaxFactory.List<UsingDirectiveSyntax>();

            string lastRootNamespace = String.Empty;
            foreach (UsingDirectiveSyntax u in sortedUsings)
            {
                string rootNamespace = u.Name.GetFirstToken().ToString();

                if(lastRootNamespace == String.Empty)
                {
                    whitespaceCorrectedUsings = whitespaceCorrectedUsings.Add(u.WithLeadingTrivia(firstUsingTrivia));
                }
                else if (rootNamespace != lastRootNamespace)
                {
                    whitespaceCorrectedUsings = whitespaceCorrectedUsings.Add(u.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed));
                }
                else
                {
                    whitespaceCorrectedUsings = whitespaceCorrectedUsings.Add(u.WithoutLeadingTrivia());
                }

                lastRootNamespace = rootNamespace;
            }

            var newRoot = node.WithUsings(whitespaceCorrectedUsings);

            if (!newRoot.IsEquivalentTo(node))
            {
                return newRoot;
            }
            else
            {
                return node;
            }
        }

        private class UsingSorter : IComparer<string>
        {
            public int Compare(string leftNs, string rightNs)
            {
                // If either namespace starts with 'System', remove the prefix to make it first ('.' before all letters)
                if (leftNs.StartsWith("System")) { leftNs = leftNs.Substring("System".Length); }
                if (rightNs.StartsWith("System")) { rightNs = rightNs.Substring("System".Length); }

                return leftNs.CompareTo(rightNs);
            }
        }
    }
}
