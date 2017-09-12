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

        /// <summary>
        /// Execute the archiver and return the list of archive file(s).  There may be more than
        /// one archive file if there are a lot of files and it exceeds our maximum files per
        /// archive.
        /// </summary>
        /// <param name="practiceId"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public List<string> Execute(string practiceId, string directory)
        {
            var archiveList = new List<string>();
            string archiveName = string.Format("{0}-{1}.zip", practiceId, DateSuffix);
            string archivePath = Path.Combine(Config.PracticeArchiveTempFolder, archiveName);

            var files = Directory.GetFiles(directory, Config.UploadFilePattern);
            if (files.Length == 0)
            {
                Log.Write(string.Format("No files were in the folder: {0}", directory));
                return archiveList;
            }

            // We could probably do all of this with the same code if there fewer than the
            // maximum number of files, but we want to preserve the naming convention ("..-index")
            // just when we absolutely need to split up the archive.
            if (files.Length > Config.MaxFilesPerArchive)
            {
                Log.Write(
                    string.Format(
                        "There are {0} files, which exceeds the maximum per archive of {1}.  We will split this into multiple archives",
                        files.Length, Config.MaxFilesPerArchive));
                var sublists = SplitToSublists(files, Config.MaxFilesPerArchive);
                int subListIndex = 1;
                foreach (var sublist in sublists)
                {
                    archiveName = string.Format("{0}-{1}-{2}.zip", practiceId, DateSuffix, subListIndex);
                    archivePath = Path.Combine(Config.PracticeArchiveTempFolder, archiveName);
                    if (!CreateArchive(archiveName, archivePath, practiceId, sublist.ToArray()))
                    {
                        return null;
                    }
                    archiveList.Add(Path.Combine(Config.PracticeArchiveFolder, practiceId, archiveName));
                    subListIndex++;
                }
            }
            else
            {
                if (!CreateArchive(archiveName, archivePath, practiceId, files))
                {
                    return null;
                }
                archiveList.Add(Path.Combine(Config.PracticeArchiveFolder, practiceId, archiveName));
            }
            

            return archiveList;
        }

        /// <summary>
        /// Split a list into smaller lists of at most "size" elements
        /// From https://stackoverflow.com/a/18986420/5670646
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private List<List<string>> SplitToSublists(string[] source, int size)
        {
            return source
                     .Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / size)
                     .Select(x => x.Select(v => v.Value).ToList())
                     .ToList();
        }

        /// <summary>
        /// Given an archive file (named by archiveName, located at archivePath) and a list of files, populate
        /// the files in the archive file.
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="archivePath"></param>
        /// <param name="practiceId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private bool CreateArchive(string archiveName, string archivePath, string practiceId, string[] files)
        {
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
