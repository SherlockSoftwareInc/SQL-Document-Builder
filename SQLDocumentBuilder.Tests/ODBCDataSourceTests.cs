using System.Collections;
using System.Data;
using System.Data.Common;
using SQL_Document_Builder;
using Xunit;

namespace SQLDocumentBuilder.Tests;

public class ODBCDataSourceTests
{
    [Fact]
    public void CreateDataTableWithoutConstraints_AllowsNullValuesAndDuplicateColumnNames()
    {
        using var reader = new FakeConstraintViolatingReader();

        var table = ODBCDataSource.CreateDataTableWithoutConstraints(reader);

        Assert.Equal(2, table.Columns.Count);
        Assert.Equal("id", table.Columns[0].ColumnName);
        Assert.Equal("id_2", table.Columns[1].ColumnName);
        Assert.True(table.Columns[1].AllowDBNull);
        Assert.Single(table.Rows);
        Assert.Equal(DBNull.Value, table.Rows[0][1]);
    }

    private sealed class FakeConstraintViolatingReader : DbDataReader
    {
        private bool _read;

        public override int FieldCount => 2;

        public override bool Read()
        {
            if (_read)
            {
                return false;
            }

            _read = true;
            return true;
        }

        public override string GetName(int ordinal) => "id";

        public override Type GetFieldType(int ordinal) => typeof(string);

        public override object GetValue(int ordinal) => ordinal == 0 ? "1" : DBNull.Value;

        public override int GetValues(object[] values)
        {
            values[0] = "1";
            values[1] = DBNull.Value;
            return 2;
        }

        public override bool HasRows => true;
        public override bool IsClosed => false;
        public override int RecordsAffected => 0;
        public override int Depth => 0;
        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => name == "id" ? GetValue(0) : DBNull.Value;

        public override bool NextResult() => false;
        public override bool IsDBNull(int ordinal) => GetValue(ordinal) == DBNull.Value;
        public override int GetOrdinal(string name) => 0;
        public override string GetDataTypeName(int ordinal) => "varchar";
        public override IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();

        public override bool GetBoolean(int ordinal) => throw new NotSupportedException();
        public override byte GetByte(int ordinal) => throw new NotSupportedException();
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override char GetChar(int ordinal) => throw new NotSupportedException();
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override DateTime GetDateTime(int ordinal) => throw new NotSupportedException();
        public override decimal GetDecimal(int ordinal) => throw new NotSupportedException();
        public override double GetDouble(int ordinal) => throw new NotSupportedException();
        public override float GetFloat(int ordinal) => throw new NotSupportedException();
        public override Guid GetGuid(int ordinal) => throw new NotSupportedException();
        public override short GetInt16(int ordinal) => throw new NotSupportedException();
        public override int GetInt32(int ordinal) => throw new NotSupportedException();
        public override long GetInt64(int ordinal) => throw new NotSupportedException();
        public override string GetString(int ordinal) => (string)GetValue(ordinal);
        public override DataTable? GetSchemaTable() => null;
    }
}
