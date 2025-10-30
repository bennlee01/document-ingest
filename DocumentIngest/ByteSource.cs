using System;

namespace DocumentIngestApp
{
    // Finite, read-once sequence of bytes
    public interface ByteSource
    {
        byte[] NextChunk(); // returns empty array at EOF
    }
}