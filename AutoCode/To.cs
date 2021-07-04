using System.IO;

namespace AutoCode
{
    public class To
    {
        private readonly Comment _comment;

        public To(Comment comment)
        {
            _comment = comment;
        }

        public void File(string path)
        {
            if (System.IO.File.Exists(path))
            {
                _comment.Apply(new FileInfo(path));
            }
            else if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
                {
                    _comment.Apply(new FileInfo(file));
                }
            }
        }

        public string Code(string code)
        {
            return _comment.Apply(code);
        }
    }
}