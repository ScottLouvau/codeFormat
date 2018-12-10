// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeFormat.Rules
{
    /// <summary>
    ///  Rewrite if statements which have a single following statement so that they look like:
    ///  if (condition) { statment; }
    /// </summary>
    public class SingleLineIfBracesRule : CSharpSyntaxRewriter
    {
        public override SyntaxNode Visit(SyntaxNode node)
        {
            IfStatementSyntax ifNode = node as IfStatementSyntax;
            StatementSyntax statementSyntax = ifNode?.Statement;

            if (statementSyntax != null && statementSyntax.Kind() != SyntaxKind.Block)
            {
                StatementSyntax formattedStatement = statementSyntax.WithLeadingTrivia(SyntaxFactory.Space).WithTrailingTrivia(SyntaxFactory.Space);
                BlockSyntax singleLineBlock = SyntaxFactory.Block(formattedStatement).WithLeadingTrivia(SyntaxFactory.Space).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

                return ifNode
                    .ReplaceNode(statementSyntax, singleLineBlock)
                    .WithCondition(ifNode.Condition.WithoutTrivia())
                    .WithIfKeyword(ifNode.IfKeyword.WithTrailingTrivia())
                    .WithOpenParenToken(ifNode.OpenParenToken.WithoutTrivia().WithLeadingTrivia(SyntaxFactory.Space))
                    .WithCloseParenToken(ifNode.CloseParenToken.WithoutTrivia());
            }
            else
            {
                return base.Visit(node);
            }
        }
    }
}