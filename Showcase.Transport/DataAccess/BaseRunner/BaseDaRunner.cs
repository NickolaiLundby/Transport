using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace DataAccess.BaseRunner
{
    public interface IBaseDaRunner<out T>
    {
        void Run(Action<T> method, TransactionOptions? transactionOptions = null);
        TOut Run<TOut>(Func<T, TOut> method, TransactionOptions? transactionOptions = null);
    }

    public abstract class BaseDaRunner<T> : IBaseDaRunner<T>
    {
        private readonly TransactionOptions _default = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30)
        };

        private readonly string _connectionString;

        protected BaseDaRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected abstract T GetDaFactory(IDbConnection connection);

        public void Run(Action<T> method, TransactionOptions? transactionOptions = null)
        {
            Run<object>(factory =>
            {
                method(factory);
                return null;
            }, transactionOptions);
        }

        public TOut Run<TOut>(Func<T, TOut> method, TransactionOptions? transactionOptions = null)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions ?? _default);

            var result = Execute(method);
            scope.Complete();
            return result;
        }

        private TOut Execute<TOut>(Func<T, TOut> method)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return method(GetDaFactory(connection));
        }
    }
}