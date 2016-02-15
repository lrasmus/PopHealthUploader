using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PopHealthAPI;

namespace PopHealthUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayUsage();
                return;
            }

            var configuration = new Configuration();
            if (!VerifyConfiguration(configuration))
            {
                return;
            }

            var patient = new Patient(configuration.PopHealthUser, configuration.PopHealthPassword, configuration.PopHealthBaseUrl);
            //patient.UploadArchive(@"C:\Development\MSPCTRA\PopHealthUploader\h3-patients.zip");
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
