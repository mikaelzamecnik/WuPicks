using System;
using System.Collections.Generic;
using System.Text;

namespace Wu17Picks.Infrastructure.Extensions
{
    public class AppConfigHelper
    {
        public AppConfigHelper()
        {
            //edit values in appsettings.json
            AzureStorageConnectionString = "ConnectionStringKey";
            BasePath = "FirstStoragePath";
            AuxPath = "SecondaryStoragePath";
        }

        public string AzureStorageConnectionString { get; set; }
        public string BasePath { get; set; }
        public string AuxPath { get; set; }
    }
}
