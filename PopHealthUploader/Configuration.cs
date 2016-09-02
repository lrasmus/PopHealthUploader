using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopHealthUploader
{
    public class Configuration
    {
        public string PopHealthUser { get; set; }
        public string PopHealthPassword { get; set; }
        public string PopHealthBaseUrl { get; set; }
        public string LogPath { get; set; }
        public string JobConfigurationPath { get; set; }
    }
}
