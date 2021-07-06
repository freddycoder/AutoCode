using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCode.Rewriter.Comment
{
    public class RazorPageClassTriviaRewriter : CSharpSyntaxRewriter
    {
        public RazorPageClassTriviaRewriter() : base(visitIntoStructuredTrivia: true)
        {

        }

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia == false &&
                node.BaseList?.ToString().Contains(" PageModel") == true)
            {
                var triviaAdded = node.WithLeadingTrivia(GetLeadingTrivia(node))
                                      .WithTrailingTrivia(EndlineTrivia);

                var replacedNode = node.ReplaceNode(node, triviaAdded);

                return base.VisitClassDeclaration(replacedNode);
            }

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia == false)
            {
                var triviaAdded = node.WithLeadingTrivia(GetLeadingTrivia(node))
                                      .WithTrailingTrivia(EndlineTrivia);

                var replacedNode = node.ReplaceNode(node, triviaAdded);

                return base.VisitConstructorDeclaration(replacedNode);
            }

            return base.VisitConstructorDeclaration(node);
        }

        private IEnumerable<SyntaxTrivia>? GetLeadingTrivia(ConstructorDeclarationSyntax node)
        {
            if (node.Parent is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.Members.First() != node)
            {
                foreach (var trivia in EndlineTrivia)
                {
                    yield return trivia;
                }
            }

            var comment = CSharpSyntaxTree.ParseText(string.Format(ConstructorCommentTemplate, "", node.GetLeadingTrivia().Last())).GetRoot();

            var trivias = comment.DescendantTrivia();

            foreach (var trivia in trivias)
            {
                yield return trivia;
            };

            yield return node.GetLeadingTrivia().Last();
        }

        private IEnumerable<SyntaxTrivia> GetLeadingTrivia(ClassDeclarationSyntax node)
        {
            var comment = CSharpSyntaxTree.ParseText(string.Format(ClassCommentTemplate, GenerateFriendlyName(node.Identifier.ValueText), node.GetLeadingTrivia())).GetRoot();

            var trivias = comment.DescendantTrivia().Append(node.GetLeadingTrivia().Single());

            foreach (var trivia in trivias)
            {
                yield return trivia;
            };
        }

        private string GenerateFriendlyName(string valueText)
        {
            var initial = valueText.Replace("Model", "");

            var sb = new StringBuilder();

            for (int i = 0; i < initial.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(char.ToLower(initial[i]));
                }
                else if (char.IsUpper(initial[i]))
                {
                    sb.Append(' ');
                    sb.Append(char.ToLower(initial[i]));
                }
                else
                {
                    sb.Append(initial[i]);
                }
            }

            return sb.ToString();
        }

        public string ClassCommentTemplate =
@"{1}/// <summary>
{1}/// The {0} page model
{1}/// </summary>
";

        public string ConstructorCommentTemplate =
@"{1}/// <summary>
{1}/// Constructor with dependencies
{1}/// </summary>
";

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
