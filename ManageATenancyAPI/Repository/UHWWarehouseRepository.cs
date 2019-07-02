using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.DbContext;
using ManageATenancyAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManageATenancyAPI.Repository
{
    public class UHWWarehouseRepository :IUHWWarehouseRepository
    {
        private UHWWarehouseDbContext _context;

        private ILoggerAdapter<UHWWarehouseRepository> _logger;
        public UHWWarehouseRepository(UHWWarehouseDbContext context, ILoggerAdapter<UHWWarehouseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<object> GetTagReferencenumber(string hackneyhomesId)
        {
            _logger.LogInformation($"GetAsync Tag Refernce number for {hackneyhomesId}");

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    _context.Database.OpenConnection();
                    command.CommandText = "GetTagReferenceNumber";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter
                    {
                        DbType = DbType.String,
                        ParameterName = "@hackneyhomeId",
                        Value = hackneyhomesId
                    });


                    var tagReference="";
                    using (var reader =  command.ExecuteReaderAsync().Result)
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            if (reader["key1"] != null)
                            {
                                tagReference = reader["key1"].ToString().Trim();
                            }
                        }
                    }


                    return tagReference;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new UHWWarehouseRepositoryException();
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }

    }

    public class UHWWarehouseRepositoryException: Exception
    {
    }

}
