using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ManageATenancyAPI.Tests.Repository
{
    //Created a stub to test the ado.net execute reader return object
    public class DataReaderStub<TObject, TRow> : IDataReader
    {
        private readonly IEnumerator<TObject> _enumerator;
        private readonly Func<TObject, object[]> _objectToArray;
        private readonly Dictionary<string, int> _nameIndex;
        private readonly string[] _name;
        private readonly Type[] _type;

        private bool _isClosed;
        private object[] _row;

        public DataReaderStub(IEnumerable<TObject> items, Expression<Func<TObject, TRow>> mapper)
        {
            _enumerator = items.GetEnumerator();

            var newExpression = (NewExpression)mapper.Body;

            var members = newExpression.Members.Cast<PropertyInfo>().ToArray();

            var count = members.Length;
            _nameIndex = new Dictionary<string, int>(count);
            _name = new string[count];
            _type = new Type[count];

            for (int i = 0; i < count; i++)
            {
                var member = members[i];
                var name = member.Name;
                var type = member.PropertyType;

                _name[i] = name;
                _type[i] = Nullable.GetUnderlyingType(type) ?? type;
                _nameIndex.Add(name, i);
            }

            var parameter = ((MemberExpression)newExpression.Arguments[0]).Expression as ParameterExpression;

            var arrayItems = newExpression.Arguments.Select(x => Expression.Convert(x, typeof(object)));

            var newArray = Expression.NewArrayInit(typeof(object), arrayItems);
            var toArray = Expression.Lambda<Func<TObject, object[]>>(newArray, parameter);
            _objectToArray = toArray.Compile();
        }

        public void Close()
        {
            _isClosed = true;
        }

        public int Depth
        {
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { return _isClosed; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            _row = null;

            var ok = _enumerator.MoveNext();

            if (ok)
                _row = _objectToArray(_enumerator.Current);

            return ok;
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public int FieldCount
        {
            get { return _name.Length; }
        }

        public bool GetBoolean(int i)
        {
            return (bool)_row[i];
        }

        public byte GetByte(int i)
        {
            return (byte)_row[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var binary = (byte[])_row[i];
            Array.Copy(binary, fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        public char GetChar(int i)
        {
            return (char)_row[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var binary = (char[])_row[i];
            Array.Copy(binary, fieldoffset, buffer, bufferoffset, length);
            return length;
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)_row[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)_row[i];
        }

        public double GetDouble(int i)
        {
            return (double)_row[i];
        }

        public Type GetFieldType(int i)
        {
            return _type[i];
        }

        public float GetFloat(int i)
        {
            return (float)_row[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)_row[i];
        }

        public short GetInt16(int i)
        {
            return (short)_row[i];
        }

        public int GetInt32(int i)
        {
            return (int)_row[i];
        }

        public long GetInt64(int i)
        {
            return (long)_row[i];
        }

        public string GetName(int i)
        {
            return _name[i];
        }

        public int GetOrdinal(string name)
        {
            return _nameIndex[name];
        }

        public string GetString(int i)
        {
            return (string)_row[i];
        }

        public object GetValue(int i)
        {
            return _row[i];
        }

        public int GetValues(object[] values)
        {
            _row.CopyTo(values, 0);
            return _row.Length;
        }

        public bool IsDBNull(int i)
        {
            return _row[i] == null;
        }

        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }
    }
}
