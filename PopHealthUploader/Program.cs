using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PopHealthAPI;

namespace PopHealthUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                DisplayUsage();
                return;
            }

            var configuration = new Configuration();
            if (!VerifyConfiguration(configuration))
            {
                return;
            }

            var logger = new Logger(DateTime.Now.ToString("yyyyMMddHHmmssfff"), configuration.LogPath);
            logger.Write("Beginning import job");

            string importPath = args[0];
            logger.Write(string.Format("Importing: {0}", importPath));
            string practiceId = args[1];

            var patientApi = new PatientApi(configuration.PopHealthUser, configuration.PopHealthPassword, configuration.PopHealthBaseUrl);
            var practiceApi = new PracticeApi(configuration.PopHealthUser, configuration.PopHealthPassword, configuration.PopHealthBaseUrl);
            try
            {
                if (Path.HasExtension(importPath))
                {
                    if (Path.GetExtension(importPath).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                    {
                        logger.Write(string.Format("Searching for practice with alternate Id {0}", practiceId));
                        var practices = practiceApi.SearchForPracticesByAlternateId(practiceId);
                        if (practices == null || practices.Count == 0)
                        {
                            var responseMessage = string.Format("No practices were found with alternate Id {0}.", practiceId);
                            throw new Exception(responseMessage);
                        }
                        if (practices.Count > 1)
                        {
                            var responseMessage = string.Format("{0} practices were found with alternate Id {1}.",
                                practices.Count, practiceId);
                            throw new Exception(responseMessage);
                        }
                        var practice = practiceApi.Get(practices.First().Id);
                        logger.Write("Completed searching for practice");

                        if (practice.PatientCount.HasValue && practice.PatientCount.Value > 0)
                        {
                            var responseMessage = string.Format("Practice {0} ({1}) has {2} patients loaded.\r\nYou must remove existing patients from popHealth before proceeding.",
                                practice.Name, practice.Id, practice.PatientCount.Value);
                            throw new Exception(responseMessage);
                        }

                        logger.Write("Beginning patient archive import");
                        patientApi.UploadArchive(importPath, practice);
                        logger.Write("Successfully finished patient archive import");
                    }
                    else
                    {
                        logger.Write("Unknown file extension.  This job will exit with no records imported.");
                        DisplayUsage();
                    }
                }
                else
                {
                    logger.Write("No file extension could be found.  This job will exit with no records imported.");
                    DisplayUsage();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("The following error was raised:\r\n  {0}\r\n\r\nSee {1} for more details.",
                    exc.Message, logger.LogPath);
                logger.WriteException(exc);
                Environment.Exit(-1);
            }

            logger.Write("Ending import job");
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

            if (!valid)
            {
                Console.WriteLine();
            }

            return valid;
        }

        /// <summary>
        /// Display the usage information for this application, including expected parameters and a
        /// description of those parameters.
        /// </summary>
        public static void DisplayUsage()
        {
            Console.WriteLine("popHealth Patient Uploader");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("  PopHealthUploader directory | zip_file");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  directory - The directory to process, containing XML and/or JSON files");
            Console.WriteLine("  zip_file  - An existing zipped archive file");
            Console.WriteLine("");
        }
    }
}
