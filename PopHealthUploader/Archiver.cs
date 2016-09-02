using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopHealthUploader
{
    public class Archiver
    {
        public Configuration Config { get; set; }
        public Logger Log { get; set; }
        public string DateSuffix { get; set; }

        public Archiver(Configuration config, Logger log)
        {
            Config = config;
            Log = log;
            DateSuffix = DateTime.Now.ToString("yyyyMMdd");
        }

        public bool Execute(string practiceId, string directory)
        {
            string archiveName = string.Format("{0}-{1}.zip", practiceId, DateSuffix);
            string archivePath = Path.Combine(Config.PracticeArchiveTempFolder, archiveName);

            var files = Directory.GetFiles(directory, "*.xml");
            if (files.Length == 0)
            {
                Log.Write(string.Format("No files were in the folder: {0}", directory));
                return true;
            }

            if (File.Exists(archivePath))
            {
                Log.Write(string.Format("Deleting existing archive at {0}", archivePath));
                File.Delete(archivePath);
            }

            Log.Write(string.Format("Adding {0} file(s) to the archive at {1}", files.Length, archivePath));
            CreateZipFile(archivePath, files);

            if (!File.Exists(archivePath))
            {
                Log.Write(string.Format("There was an error establishing the temporary archive at {0}", archivePath));
                return false;
            }

            string destinationDirectory = Path.Combine(Config.PracticeArchiveFolder, practiceId);
            if (!Directory.Exists(destinationDirectory))
            {
                Log.Write(string.Format("Creating archive folder at {0}", destinationDirectory));
                Directory.CreateDirectory(destinationDirectory);
            }

            string destinationPath = Path.Combine(destinationDirectory, archiveName);
            if (File.Exists(destinationPath))
            {
                Log.Write(string.Format("Removing existing archive file {0}", destinationPath));
                File.Delete(destinationPath);
            }

            File.Move(archivePath, destinationPath);
            Log.Write(string.Format("Moved archive to {0}", destinationPath));

            return true;
        }

        public string GetArchivePath(string practiceId)
        {
            return Path.Combine(Config.PracticeArchiveFolder, practiceId, string.Format("{0}-{1}.zip", practiceId, DateSuffix));
        }

        /// <summary>
        /// Create a ZIP file of the files provided.
        /// </summary>
        /// <param name="fileName">The full path and name to store the ZIP file at.</param>
        /// <param name="files">The list of files to be added.</param>
        private static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }
    }
}
