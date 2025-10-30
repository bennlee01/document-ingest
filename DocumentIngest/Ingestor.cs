using System;
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
            long totalBytes = 0;

            // Read all bytes once
            byte[] buffer;
            using var ms = new MemoryStream();
            while ((buffer = src.NextChunk())?.Length > 0)
            {
                ms.Write(buffer, 0, buffer.Length);
                sha256.TransformBlock(buffer, 0, buffer.Length, null, 0);
                totalBytes += buffer.Length;

                if (totalBytes > cfg.MaxContentLength)
                    result.Errors.Add($"exceeds maxContentLength: {cfg.MaxContentLength}");
            }
            sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            result.Size = totalBytes;
            result.Sha256 = BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();

            // Detect MIME (simplified: file signature)
            var bytesArray = ms.ToArray();
            result.DetectedMime = DetectMime(bytesArray);

            // Validate claimedMime/contentLength
            if (meta.ContentLength.HasValue && meta.ContentLength.Value != totalBytes)
                result.Errors.Add($"contentLength mismatch: {meta.ContentLength.Value} vs actual {totalBytes}");

            if (!cfg.AcceptedMimes.Contains(result.DetectedMime))
                result.Errors.Add($"detected MIME not accepted: {result.DetectedMime}");

            // Forward to sink
            sink.Persist(meta, result, new ArrayByteSource(bytesArray));
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
                if (data.Length >= 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
                    return "image/png";
            }
            return "application/octet-stream";
        }
    }

    // Helper ByteSource for byte arrays
    public class ArrayByteSource : ByteSource
    {
        private readonly byte[] _data;
        private int _position = 0;

        public ArrayByteSource(byte[] data)
        {
            _data = data;
        }

        public byte[] NextChunk()
        {
            if (_position >= _data.Length) return Array.Empty<byte>();

            int chunkSize = Math.Min(8192, _data.Length - _position);
            var chunk = new byte[chunkSize];
            Array.Copy(_data, _position, chunk, 0, chunkSize);
            _position += chunkSize;
            return chunk;
        }
    }
}
