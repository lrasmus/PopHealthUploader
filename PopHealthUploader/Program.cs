using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PopHealthAPI;
using PopHealthAPI.Model;

namespace PopHealthUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new Configuration();
            if (!VerifyConfiguration(configuration))
            {
                return;
            }

            var logger = new Logger(DateTime.Now.ToString("yyyyMMddHHmmssfff"), configuration.LogPath);
            logger.Write("Beginning import job");

            var queryTemplates = JsonConvert.DeserializeObject<List<Query>>(File.ReadAllText(configuration.JobConfigurationPath));
            if (queryTemplates == null || queryTemplates.Count == 0)
            {
                LogAndDisplay("The job configuration data returned no templates", logger);
                Environment.Exit(-1);
            }

            // Get a list of all of the practice folders that we have data for.  This will drive the rest of the program.
            // If there are no practices, we end assuming there is just no data for us to process.
            var practices = GetPractices(configuration);
            if (practices.Count == 0)
            {
                LogAndDisplay("There were no directories to process", logger);
                Environment.Exit(-1);
            }

            var archiver = new Archiver(configuration, logger);
            foreach (var practice in practices)
            {
                if (!archiver.Execute(practice.Key, practice.Value))
                {
                    LogAndDisplay(string.Format("Failed to create the archive for practice {0} from {1}", practice.Key, practice.Value), logger);
                    Environment.Exit(-1);
                }

                //var archivePath = archiver.GetArchivePath(practice.Key);
                //var uploader = new Uploader(configuration, logger);
                //if (!uploader.Execute(archivePath, practice.Key, queryTemplates))
                //{
                //    logger.Write("There was an error performing the upload");
                //    Environment.Exit(-1);
                //}
            }

            logger.Write("Ending import job");
        }

        public static Dictionary<string, string> GetPractices(Configuration configuration)
        {
            var practices = new Dictionary<string, string>();
            var directories = Directory.GetDirectories(configuration.PracticeDataInputDirectory);
            var regex = new Regex(configuration.PracticeFolderPattern);
            foreach (var directory in directories)
            {
                var match = regex.Match(directory);
                if (match.Success)
                {
                    practices.Add(match.Groups[1].Value, directory);
                }
            }

            return practices;
        }

        /// <summary>
        /// Ensure all of the settings we need to operate are present
        /// </summary>
        /// <returns></returns>
        public static bool VerifyConfiguration(Configuration configuration)
        {
            bool valid = true;
            configuration.PopHealthUser = ConfigurationManager.AppSettings["popHealthUser"];
            if (string.IsNullOrWhiteSpace(configuration.PopHealthUser))
            {
                Console.WriteLine("popHealthUser must be specified in the App.config");
                valid = false;
            }

            configuration.PopHealthPassword = ConfigurationManager.AppSettings["popHealthPassword"];
            if (string.IsNullOrWhiteSpace(configuration.PopHealthPassword))
            {
                Console.WriteLine("popHealthPassword must be specified in the App.config");
                valid = false;
            }

            configuration.PopHealthBaseUrl = ConfigurationManager.AppSettings["popHealthBaseUrl"];
            if (string.IsNullOrWhiteSpace(configuration.PopHealthBaseUrl))
            {
                Console.WriteLine("popHealthBaseUrl must be specified in the App.config");
                valid = false;
            }

            configuration.LogPath = ConfigurationManager.AppSettings["LogPath"];
            if (string.IsNullOrWhiteSpace(configuration.LogPath))
            {
                Console.WriteLine("LogPath must be specified in the App.config");
                valid = false;
            }

            configuration.JobConfigurationPath = ConfigurationManager.AppSettings["JobConfigurationPath"];
            if (string.IsNullOrWhiteSpace(configuration.JobConfigurationPath))
            {
                Console.WriteLine("JobConfigurationPath must be specified in the App.config");
                valid = false;
            }

            configuration.PracticeDataInputDirectory = ConfigurationManager.AppSettings["PracticeDataInputDirectory"];
            if (string.IsNullOrWhiteSpace(configuration.PracticeDataInputDirectory))
            {
                Console.WriteLine("PracticeDataInputDirectory must be specified in the App.config");
                valid = false;
            }

            configuration.PracticeFolderPattern = ConfigurationManager.AppSettings["PracticeFolderPattern"];
            if (string.IsNullOrWhiteSpace(configuration.PracticeFolderPattern))
            {
                Console.WriteLine("PracticeFolderPattern must be specified in the App.config");
                valid = false;
            }

            configuration.PracticeArchiveTempFolder = ConfigurationManager.AppSettings["PracticeArchiveTempFolder"];
            if (string.IsNullOrWhiteSpace(configuration.PracticeArchiveTempFolder))
            {
                Console.WriteLine("PracticeArchiveTempFolder must be specified in the App.config");
                valid = false;
            }

            configuration.PracticeArchiveFolder = ConfigurationManager.AppSettings["PracticeArchiveFolder"];
            if (string.IsNullOrWhiteSpace(configuration.PracticeArchiveFolder))
            {
                Console.WriteLine("PracticeArchiveFolder must be specified in the App.config");
                valid = false;
            }

            if (!valid)
            {
                Console.WriteLine();
            }

            return valid;
        }

        private static void LogAndDisplay(string message, Logger logger)
        {
            logger.Write(message);
            Console.WriteLine(message);
        }
    }
}
