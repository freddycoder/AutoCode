using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCode.Rewriter
{
    public class StructuredTriviaRewriter : CSharpSyntaxRewriter
    {
        public StructuredTriviaRewriter(string commentXml) : base(visitIntoStructuredTrivia: true)
        {
            CommentXml = commentXml;
        }

        public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia == false &&
                node.Modifiers.Any(m => m.ValueText == "public" || m.ValueText == "protected"))
            {
                var triviaAdded = node.WithLeadingTrivia(GetLeadingTrivia(node))
                                      .WithTrailingTrivia(EndlineTrivia);
                
                var replacedNode = node.ReplaceNode(node, triviaAdded);

                return replacedNode;
            }

            return base.VisitFieldDeclaration(node);
        }

        private IEnumerable<SyntaxTrivia> GetLeadingTrivia(FieldDeclarationSyntax node)
        {
            var comment = CSharpSyntaxTree.ParseText(GenerateCommentFor(node)).GetRoot();

            var trivias = comment.DescendantTrivia().Append(node.GetLeadingTrivia().Single());

            if (node.Parent is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.Members.First() != node)
            {
                foreach (var endl in EndlineTrivia)
                {
                    yield return endl;
                }
            }

            foreach (var trivia in trivias)
            {
                yield return trivia;
            }
        }

        private string GenerateCommentFor(FieldDeclarationSyntax node)
        {
            return string.Format(CommentXml, 
                                 ParseValue(node.Declaration.Variables.ToString()), 
                                 node.GetLeadingTrivia());
        }

        private static string? ParseValue(object? value)
        {
            return value?.ToString()?.Split('=').LastOrDefault()?.Replace("\"", "").Replace(" ", "");
        }

        public string CommentXml { get; set; }

        private static readonly string TrailingTriviaTemplate = $"{Environment.NewLine}";

        private IEnumerable<SyntaxTrivia>? _defaultSyntaxeTrivia;

        public IEnumerable<SyntaxTrivia> EndlineTrivia
        {
            get
            {
                if (_defaultSyntaxeTrivia != null) return _defaultSyntaxeTrivia;

                var triviaNode = CSharpSyntaxTree.ParseText(TrailingTriviaTemplate).GetRoot();

                _defaultSyntaxeTrivia = triviaNode.DescendantTrivia();

                return _defaultSyntaxeTrivia;
            }
        }
    }
}
