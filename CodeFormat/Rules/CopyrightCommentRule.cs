// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeFormat.Rules
{
    /// <summary>
    ///  Ensure the file starts with the correct copyright comment.
    /// </summary>
    public class CopyrightCommentRule : CSharpSyntaxRewriter
    {
        public string CommentText;
        public SyntaxTriviaList Comment;

        public CopyrightCommentRule()
        {
            CommentText = File.ReadAllText("CopyrightComment.txt").TrimEnd() + "\r\n\r\n";
            Comment = CSharpSyntaxTree.ParseText(CommentText).GetRoot().GetLeadingTrivia();
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            string leadingComment = node.GetLeadingTrivia().ToFullString();
            if (leadingComment.Equals(CommentText))
            {
                return node;
            }
            else
            {
                return node.WithLeadingTrivia(Comment);
            }
        }
    }
}