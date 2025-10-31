using System;

namespace DocumentIngestApp.Domain.Interfaces
{
    /// <summary>
    /// Represents a finite, read-once sequence of bytes.
    /// Once read, bytes cannot be re-read.
    /// Used for streaming upload content.
    /// </summary>
    public interface IByteSource
    {
        /// <summary>
        /// Reads the next chunk of bytes.
        /// Returns an empty array when the end of the stream is reached.
        /// </summary>
        /// <returns>Byte array containing the next chunk or empty array at EOF.</returns>
        byte[] NextChunk();
    }
}