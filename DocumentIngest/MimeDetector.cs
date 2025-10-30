using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DocumentIngestApp
{
    public static class MimeDetector
    {
        public static string DetectMime(byte[] bytes)
        {
            if (bytes.Length >= 8)
            {
                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (bytes[0] == 0x89 && bytes[1] == 0x50 &&
                    bytes[2] == 0x4E && bytes[3] == 0x47 &&
                    bytes[4] == 0x0D && bytes[5] == 0x0A &&
                    bytes[6] == 0x1A && bytes[7] == 0x0A)
                {
                    return "image/png";
                }
            }

            if (bytes.Length >= 3)
            {
                // JPEG: FF D8 FF
                if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                {
                    return "image/jpeg";
                }
            }

            if (bytes.Length >= 4)
            {
                // PDF: %PDF
                if (bytes[0] == 0x25 && bytes[1] == 0x50 &&
                    bytes[2] == 0x44 && bytes[3] == 0x46)
                {
                    return "application/pdf";
                }

                // DOCX: ZIP magic number
                if (bytes[0] == 0x50 && bytes[1] == 0x4B &&
                    bytes[2] == 0x03 && bytes[3] == 0x04)
                {
                    try
                    {
                        using (var ms = new MemoryStream(bytes))
                        using (var zip = new ZipArchive(ms, ZipArchiveMode.Read, true))
                        {
                            if (zip.Entries.Any(e => 
                                e.FullName.Equals("[Content_Types].xml", StringComparison.OrdinalIgnoreCase)))
                            {
                                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            }
                        }
                    }
                    catch
                    {
                        return "application/zip";
                    }
                }
            }

            // Fallback
            return "application/octet-stream";
        }
    }
}
