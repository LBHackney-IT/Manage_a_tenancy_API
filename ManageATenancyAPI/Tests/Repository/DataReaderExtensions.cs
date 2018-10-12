using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace ManageATenancyAPI.Tests.Repository
{
    public static class DataReaderExtensions
    {
        public static IDataReader AsDataReader<TObject, TDataRow>(this IEnumerable<TObject> items, Expression<Func<TObject, TDataRow>> mapper)
        {
            return new DataReaderStub<TObject, TDataRow>(items, mapper);
        }
    }
}
