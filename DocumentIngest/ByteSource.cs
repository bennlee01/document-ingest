using System;

namespace DocumentIngestApp
{
    public interface ByteSource
    {
        // Returns next chunk of bytes or empty array if EOF
        byte[] NextChunk();
    }
}