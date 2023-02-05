using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MasterDiploma_FuzzyAssociationRules
{
    public class DataSettings
    {
        public string DbConnectionString { get; set; }
        public string[] Columns { get; set; }
        public string RawDataTableName { get; set; }
        public bool ShouldFuzzify { get; set; }
        public bool UsePreComputedRules { get; set; }
        public double LargeItemSetTreshold { get; set; }
        public double RuleTreshold { get; set; }
        public bool AddFuzzifiedTableToDb { get; set; }
        public bool GroupRulesByTheirRHS { get; set; }
        public int TakeTopNFromEachGroup { get; set; }
        public int OutputTopNRules { get; set; }
        public FuzzyParameter[] FuzzyParameters { get; set; }
        public string OutputDirectory { get; set; }
        public static DataSettings ReadFromFile(string settingsFilePath)
        {
            using var sr = new StreamReader(settingsFilePath);
            var settingsText = sr.ReadToEnd();
            var settings = JsonConvert.DeserializeObject<DataSettings>(settingsText);
            return settings;
        }
    }
}
