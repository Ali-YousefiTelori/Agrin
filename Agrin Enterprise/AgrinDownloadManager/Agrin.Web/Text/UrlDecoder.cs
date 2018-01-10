using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Web.Text
{
    public class UrlDecoder
    {
        public static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = System.Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        // Fields
        private int _bufferSize;
        private byte[] _byteBuffer;
        private char[] _charBuffer;
        private Encoding _encoding;
        private int _numBytes;
        private int _numChars;

        // Methods
        public UrlDecoder(int bufferSize, Encoding encoding)
        {
            this._bufferSize = bufferSize;
            this._encoding = encoding;
            this._charBuffer = new char[bufferSize];
        }

        public void AddByte(byte b)
        {
            if (this._byteBuffer == null)
            {
                this._byteBuffer = new byte[this._bufferSize];
            }
            this._byteBuffer[this._numBytes++] = b;
        }

        public void AddChar(char ch)
        {
            if (this._numBytes > 0)
            {
                this.FlushBytes();
            }
            this._charBuffer[this._numChars++] = ch;
        }

        private void FlushBytes()
        {
            if (this._numBytes > 0)
            {
                this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                this._numBytes = 0;
            }
        }

        public string GetString()
        {
            if (this._numBytes > 0)
            {
                this.FlushBytes();
            }
            if (this._numChars > 0)
            {
                return new string(this._charBuffer, 0, this._numChars);
            }
            return string.Empty;
        }
    }
}
