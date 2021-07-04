using AutoCode.Rewriter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;

namespace AutoCode
{
    public class Comment
    {
        public Comment()
        {
            To = new To(this);
        }

        public To To { get; }

        public string CommentTemplate { get; set; } =
@"{1}/// <summary>
{1}/// A constante with the value {0}
{1}/// </summary>
";

        public void Apply(FileInfo code)
        {
            var content = File.ReadAllText(code.FullName);

            var source = CSharpSyntaxTree.ParseText(content)
                                       .WithFilePath(code.FullName);

            var rewriter = new StructuredTriviaRewriter(CommentTemplate);

            var newSource = rewriter.Visit(source.GetRoot());

            OnCompleted(source, newSource);
        }

        public Action<SyntaxTree, SyntaxNode> OnCompleted = (oldSource, newSource) =>
        {
            if (newSource != oldSource.GetRoot())
            {
                File.WriteAllText(oldSource.FilePath, newSource.ToFullString());
            }
        };

        public string Apply(string code)
        {
            var source = CSharpSyntaxTree.ParseText(code);

            var rewriter = new StructuredTriviaRewriter(CommentTemplate);

            var newSource = rewriter.Visit(source.GetRoot());

            return newSource.ToFullString();
        }
    }
}
