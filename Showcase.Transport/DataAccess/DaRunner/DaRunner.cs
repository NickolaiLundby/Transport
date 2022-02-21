using System.Data;
using DataAccess.BaseRunner;

namespace DataAccess.DaRunner
{
    public class DaRunner : BaseDaRunner<IDaFactory>
    {
        public DaRunner(string connectionString) : base(connectionString){}

        protected override IDaFactory GetDaFactory(IDbConnection connection)
        {
            return new DaFactory(connection);
        }
    }
}