using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MasterDiploma_FuzzyAssociationRules
{
    public class Repository
    {
        public Repository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public string CreateFuzzySqlTable(DataTable fuzzyTable)
        {
            var fuzzyTableName = FuzzyTableName(fuzzyTable.Columns.Count);
            fuzzyTable.ToSqlTable(fuzzyTableName, ConnectionString);

            using var bulkcopy = new SqlBulkCopy(ConnectionString);
            bulkcopy.DestinationTableName = fuzzyTableName;
            bulkcopy.WriteToServer(fuzzyTable);
            return fuzzyTableName;
        }

        public DataTable LoadTableFromSql(string tableName, string[] columns)
        {
            string columnText;
            if (columns == null || columns.Length == 0)
                columnText = "*";
            else
                columnText = string.Join(',', columns);

            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand($"Select {columnText} from {tableName}", sqlConnection);
            sqlConnection.Open();
            var dataReader = sqlCommand.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            return dataTable;
        }

        private string FuzzyTableName(int columnCount)
        {
            var tableName = $"Fuzzy_{columnCount}_{DataExtensions.RandomString(10)}";
            return tableName;
        }
    }
}

