using System;
using System.Collections.Generic;
using System.Text;

namespace Wu17Picks.Infrastructure.Extensions
{
    public class AppSettingsHelper
    {
        public AppSettingsHelper()
        {
            //edit values in appsettings.json
            AzureStorageConnectionString = "ConnectionStringKey";
        }

        public string AzureStorageConnectionString { get; set; }
    }
}
