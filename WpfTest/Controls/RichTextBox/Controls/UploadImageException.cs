using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WpfTest.Controls.RichTextBox.Controls
{

    [Serializable]
    public class UploadImageException : Exception
    {
        public UploadImageException() { }

        public UploadImageException(string message) : base(message) { }

        public UploadImageException(string message, Exception inner) : base(message, inner) { }

        protected UploadImageException( SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
