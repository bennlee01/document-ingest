using System;

namespace DocumentIngestApp
{
    public class ArrayByteSource : ByteSource
    {
        private readonly byte[] _data;
        private int _position = 0;
        private readonly int _chunkSize;

        public ArrayByteSource(byte[] data, int chunkSize = 8192)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _chunkSize = chunkSize;
        }

        public byte[] NextChunk()
        {
            if (_position >= _data.Length)
                return Array.Empty<byte>();

            int remaining = _data.Length - _position;
            int size = Math.Min(_chunkSize, remaining);
            byte[] chunk = new byte[size];
            Array.Copy(_data, _position, chunk, 0, size);
            _position += size;
            return chunk;
        }
    }
}