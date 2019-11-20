using System;
using System.IO;
using System.Text;

namespace Test
{
   class TeeTextWriter: TextWriter {
        private TextWriter _actualWriter;
        private string _linePrefix = null;
        private StringBuilder _text = new StringBuilder();

        private bool _isLineStart = true;

        public string OutputText { get { return _text.ToString(); }}

        public void Reinit() {
            _text.Clear();
            if (! _isLineStart) {
                _actualWriter.Write('\n');
                _isLineStart = true;
            }
        }

        public override System.Text.Encoding Encoding { get {return _actualWriter.Encoding; } }

        public TeeTextWriter(TextWriter actualWriter) {
            _actualWriter = actualWriter;
        }

        public TeeTextWriter(TextWriter actualWriter, string linePrefix) {
            _actualWriter = actualWriter;
            _linePrefix = linePrefix;
        }

        public override void Write(char c) {
            if (_linePrefix != null) 
            {
                if (_isLineStart) {
                    _actualWriter.Write(_linePrefix);
                }
                _isLineStart = (c == '\n');
            }
            _actualWriter.Write(c);
            
            _text.Append(c);
        }
    }
}