using System.Data;
using System.Data.Common;
using EMC.Data.Sql;
using SimphonyUtilities.Settings;

namespace SimphonyExtAppDemo.Factories.DbConnectionFactories
{
    public class SimphonyDbConnectionFactory
    {
        private readonly DatabaseSettings _databaseSettings;

        public SimphonyDbConnectionFactory()
        {
            _databaseSettings = DatabaseSettings.GetDbSettingsSafe(DatabaseSettings.DatabaseAlias.Master);
        }

        public IDbConnection CreateConnection() => new DatabaseConnection(_databaseSettings).Connection as DbConnection;
    }
}