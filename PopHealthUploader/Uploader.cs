using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PopHealthAPI;
using PopHealthAPI.Model;

namespace PopHealthUploader
{
    public class Uploader
    {
        public Configuration Config { get; set; }
        public Logger Log { get; set; }

        public Uploader(Configuration config, Logger log)
        {
            Config = config;
            Log = log;
        }

        public bool Execute(string importPath, string practiceId, List<Query> queryTemplates)
        {
            var patientApi = new PatientApi(Config.PopHealthUser, Config.PopHealthPassword, Config.PopHealthBaseUrl);
            var practiceApi = new PracticeApi(Config.PopHealthUser, Config.PopHealthPassword, Config.PopHealthBaseUrl);
            var queryApi = new QueryApi(Config.PopHealthUser, Config.PopHealthPassword, Config.PopHealthBaseUrl);

            try
            {
                if (Path.HasExtension(importPath))
                {
                    if (Path.GetExtension(importPath).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Log.Write(string.Format("Searching for practice with alternate Id {0}", practiceId));
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
                        Log.Write("Completed searching for practice");

                        if (practice.PatientCount.HasValue && practice.PatientCount.Value > 0)
                        {
                            var responseMessage = string.Format("Practice {0} ({1}) has {2} patients loaded.\r\nYou must remove existing patients from popHealth before proceeding.",
                                practice.Name, practice.Id, practice.PatientCount.Value);
                            throw new Exception(responseMessage);
                        }

                        Log.Write("Beginning patient archive import");
                        patientApi.UploadArchive(importPath, practice);
                        Log.Write("Successfully finished patient archive import");

                        //System.Threading.Thread.Sleep(10000);

                        //Log.Write("Beginning query cache setup");
                        //foreach (var template in queryTemplates)
                        //{
                        //    if (practice.Providers != null)
                        //    {
                        //        Query query = null;
                        //        foreach (var provider in practice.Providers)
                        //        {
                        //            query = new Query(template) { Providers = new[] { provider } };
                        //            queryApi.Add(query);
                        //        }

                        //        query = new Query(template) { Providers = new[] { practice.ProviderId } };
                        //        queryApi.Add(query);
                        //    }
                        //}
                        //Log.Write("Successfully finished loading query cache jobs");
                    }
                    else
                    {
                        Log.Write("Unknown file extension.  This job will exit with no records imported.");
                    }
                }
                else
                {
                    Log.Write("No file extension could be found.  This job will exit with no records imported.");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("The following error was raised:\r\n  {0}\r\n\r\nSee {1} for more details.",
                    exc.Message, Log.LogPath);
                Log.WriteException(exc);
                return false;
            }

            return true;
        }
    }
}
