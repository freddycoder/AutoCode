using AutoCode.Rewriter;
using AutoCode.Rewriter.Comment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
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

            CSharpSyntaxRewriter rewriter = new ConstanteFieldTriviaRewriter(CommentTemplate);

            var newSource = rewriter.Visit(source.GetRoot());

            rewriter = new RazorPageClassTriviaRewriter();

            newSource = rewriter.Visit(newSource);

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

            var rewriter = new ConstanteFieldTriviaRewriter(CommentTemplate);

            var newSource = rewriter.Visit(source.GetRoot());

            return newSource.ToFullString();
        }
    }
}
