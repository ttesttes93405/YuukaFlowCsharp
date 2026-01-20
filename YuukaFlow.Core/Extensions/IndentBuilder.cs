using System.Text;

namespace YuukaFlow.Core.Extensions
{
    internal class IndentBuilder
    {
        private readonly StringBuilder _builder;
        private readonly string _indentUnit;
        private readonly int _baseLevel;

        public IndentBuilder(int baseLevel = 0, string indentUnit = "    ")
        {
            _builder = new StringBuilder();
            _indentUnit = indentUnit;
            _baseLevel = baseLevel;
        }
        
        public IndentBuilder AppendLine(int indent, string text)
        {
            AppendIndent(_baseLevel + indent).AppendLine(text);
            return this;
        }

        public IndentBuilder Append(int indent, string text)
        {
            AppendIndent(_baseLevel + indent).Append(text);
            return this;
        }

        public IndentBuilder Append(string text)
        {
            _builder.Append(text);
            return this;
        }
        
        public IndentBuilder AppendLine(string text = "")
        {
            _builder.AppendLine(text);
            return this;
        }

        private IndentBuilder AppendIndent(int level)
        {
            for (int i = 0; i < level; i++)
                _builder.Append(_indentUnit);

            return this;
        }

        public override string ToString() => _builder.ToString();
    }
}
