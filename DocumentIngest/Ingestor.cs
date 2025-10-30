using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DocumentIngestApp
{
    public class Ingestor
    {
        public void Ingest(UploadMeta meta, IngestConfig cfg, ByteSource src, IngestSink sink)
        {
            if (meta == null) throw new ArgumentNullException(nameof(meta));
            if (cfg == null) throw new ArgumentNullException(nameof(cfg));
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (sink == null) throw new ArgumentNullException(nameof(sink));

            var errors = new List<string>();
            long totalBytes = 0;

            using (var sha256 = SHA256.Create())
            {
                // Read all chunks into a single list
                var bufferChunks = new List<byte>();

                byte[] chunk;
                while ((chunk = src.NextChunk()).Length > 0)
                {
                    sha256.TransformBlock(chunk, 0, chunk.Length, null, 0);
                    bufferChunks.AddRange(chunk); // Flatten
                    totalBytes += chunk.Length;
                }

                // Finalize SHA256
                sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                string hashHex = BitConverter.ToString(sha256.Hash!).Replace("-", "").ToLowerInvariant();

                // Detect MIME
                string detectedMime = MimeDetector.DetectMime(bufferChunks.ToArray());

                // Validation
                if (meta.ContentLength.HasValue && meta.ContentLength.Value != totalBytes)
                    errors.Add($"Content length mismatch: expected {meta.ContentLength.Value}, actual {totalBytes}");

                if (totalBytes > cfg.MaxContentLength)
                    errors.Add($"Content exceeds max allowed length of {cfg.MaxContentLength} bytes");

                if (!cfg.AcceptedMimes.Contains(detectedMime))
                    errors.Add($"Detected MIME '{detectedMime}' is not in the accepted MIME types");

                bool ok = errors.Count == 0;

                // Build result
                var result = new IngestResult(detectedMime, totalBytes, hashHex, ok, errors);

                // Forward bytes to sink
                sink.Persist(meta, result, new ArrayByteSource(bufferChunks.ToArray()));
            }
        }
    }
}
