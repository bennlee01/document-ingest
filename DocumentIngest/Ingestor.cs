using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DocumentIngestApp
{
    public class Ingestor
    {
        public void Ingest(UploadMeta meta, IngestConfig cfg, ByteSource src, IngestSink sink)
        {
            var result = new IngestResult();
            using var sha256 = SHA256.Create();

            var bytesCollected = new List<byte>();
            long totalBytes = 0;

            try
            {
                byte[] chunk;
                while ((chunk = src.NextChunk())?.Length > 0)
                {
                    bytesCollected.AddRange(chunk);
                    sha256.TransformBlock(chunk, 0, chunk.Length, null, 0);
                    totalBytes += chunk.Length;

                    if (totalBytes > cfg.MaxContentLength)
                        result.Errors.Add($"Exceeds maxContentLength: {cfg.MaxContentLength}");
                }
                sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                result.Size = totalBytes;
                result.Sha256 = BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
                result.DetectedMime = DetectMime(bytesCollected.ToArray());

                if (meta.ContentLength.HasValue && meta.ContentLength.Value != totalBytes)
                    result.Errors.Add($"contentLength mismatch: {meta.ContentLength.Value} vs actual {totalBytes}");

                if (!cfg.AcceptedMimes.Contains(result.DetectedMime))
                    result.Errors.Add($"detected MIME not accepted: {result.DetectedMime}");

                // Forward to sink
                sink.Persist(meta, result, new ArrayByteSource(bytesCollected.ToArray()));
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Exception during ingest: {ex.Message}");
                sink.Persist(meta, result, new ArrayByteSource(bytesCollected.ToArray()));
            }
        }

        private string DetectMime(byte[] data)
        {
            if (data.Length >= 4)
            {
                // PDF
                if (data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46)
                    return "application/pdf";
                // DOCX (ZIP-based)
                if (data[0] == 0x50 && data[1] == 0x4B)
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                // PNG
                if (data.Length >= 8 &&
                    data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                    return "image/png";
            }
            return "application/octet-stream";
        }
    }
}
