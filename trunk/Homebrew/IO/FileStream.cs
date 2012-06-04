using System;
using System.IO;

namespace Homebrew.IO
{
    public class FileStream : Stream
    {
        private File m_file;

        internal FileStream(File file)
        {
            m_file = file;
        }

        public override bool CanRead
        {
            get
            {
                return (m_file.FileAccess & FileAccess.Read) == FileAccess.Read;
            }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get
            {
                return (m_file.FileAccess & FileAccess.Write) == FileAccess.Write;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return m_file.Length; }
        }

        public override long Position
        {
            get { return m_file.Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_file.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_file.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
			m_file.Write(buffer, (uint)offset, (uint)count);
        }

        public override void Close()
        {
            m_file.Close();
            base.Close();
        }
    }



}
