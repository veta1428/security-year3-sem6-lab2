using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Principal;

namespace Common
{
    public static class FileActions
    {
        private static string zip = ".zip";
        private static string hash = ".hash";

        public static string Zip(string filepath)
        {
            var zipName = GetNewFileName(filepath, zip);

            using FileStream zipToOpen = new FileStream(zipName, FileMode.Create);
            using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            ZipArchiveEntry readmeEntry = archive.CreateEntry(Path.GetFileName(filepath));
            using (StreamReader reader = new StreamReader(filepath))
            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                writer.Write(reader.ReadToEnd());

            return zipName;
        }

        public static string Copy(string filepath)
        {
            string newFilePath = GetNewFileName(filepath, Path.GetExtension(filepath));

            File.Copy(filepath, newFilePath);
            return newFilePath;
        }

        public static string Hash(string filepath)
        {
            var hashFileName = GetNewFileName(filepath, hash);

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filepath);
            File.WriteAllBytes(hashFileName, md5.ComputeHash(stream));

            return hashFileName;
        }

        public static bool IsInAdminRole()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static string GetNewFileName(string oldFileName, string extension)
        {
            string dir = Path.GetDirectoryName(oldFileName) ?? string.Empty;
            return Path.Combine(dir, Path.GetFileNameWithoutExtension(oldFileName) + Guid.NewGuid() + extension);
        }
    }
}
