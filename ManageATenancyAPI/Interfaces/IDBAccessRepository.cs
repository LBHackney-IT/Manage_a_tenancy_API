using ManageATenancyAPI.Helpers;
using System.Data;
using System.Data.SqlClient;

namespace ManageATenancyAPI.Interfaces
{
    public interface IDBAccessRepository
    {       
        IDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText);

        IDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters);

        IDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership);

        void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters);

        void AttachParameters(SqlCommand command, SqlParameter[] commandParameters);
    }   

}
