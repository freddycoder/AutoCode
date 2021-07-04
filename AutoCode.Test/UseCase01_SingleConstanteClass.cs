using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AutoCode.Test
{
    public class UseCase01_SingleConstanteClass
    {
        [Fact]
        public async Task AddCommentToFile()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase01.cs");

            Automaticly.Add.Comment.To.File(constance_dot_cs);

            var result = await File.ReadAllTextAsync(constance_dot_cs);

            Assert.Equal(Expected, result);
        }

        [Fact]
        public async Task AddCommentToCode()
        {
            var constance_dot_cs = Path.Combine("CodeExample", "UseCase01.cs");

            var result = Automaticly.Add.Comment.To.Code(await File.ReadAllTextAsync(constance_dot_cs));

            Assert.Equal(Expected, result);
        }

        private const string Expected =
@"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCode.Test.CodeExample
{
    public class Constante
    {
        /// <summary>
        /// A constante with the value ConstanteA
        /// </summary>
        public const string ConstanteA = ""ConstanteA"";

        /// <summary>
        /// A constante with the value yyy-aaa
        /// </summary>
        public const string CodeFormat = ""yyy-aaa"";

        /// <summary>
        /// A constante with the value yyyy-mm-dd
        /// </summary>
        public const string DateFormat = ""yyyy-mm-dd"";
    }
}
";
    }
}
