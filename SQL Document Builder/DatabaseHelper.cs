using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The database helper.
    /// </summary>
    internal class DatabaseHelper
    {
        /// <summary>
        /// Executes the SQL statement
        /// </summary>
        /// <param name="sql">The sql.</param>
        public static void ExecuteSQL(string sql)
        {
            using var conn = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            try
            {
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //ETLEvent.AddError("ExecuteSQL", sql);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The sql.</param>
        internal static void ExecuteNonQuery(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            using var conn = new SqlConnection(connectionString);
            try
            {
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>A DataTable? .</returns>
        internal static DataTable? GetDataTable(string sql)
        {
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            return null;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A DataTable? .</returns>
        internal static DataTable? GetDataTable(string sql, string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return null;

            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
            return null;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A DataTable? .</returns>
        internal static DataTable? GetDataTable(string sql, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                connection.Open(); // Ensure the connection is opened before executing commands
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                using var reader = command.ExecuteReader();

                var dataTable = new DataTable();

                // Fill DataTable manually with support for cancellation
                LoadFromReader(reader, dataTable, cancellationToken);

                return dataTable;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Operation was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null; // Or handle it as needed
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// Gets the row count of a table or view.
        /// </summary>
        /// <param name="fullName">The full name of object.</param>
        /// <returns>An int.</returns>
        /// <summary>
        /// Gets the row count of a table or view asynchronously.
        /// </summary>
        /// <param name="fullName">The full name of the object.</param>
        /// <returns>A Task<int> representing the row count.</returns>
        internal static async Task<int> GetRowCountAsync(string fullName)
        {
            var sql = $"SELECT COUNT(*) FROM {fullName}";
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 50000
                };

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }


        /// <summary>
        /// Gets the scalar value.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>An Object? .</returns>
        internal static Object? GetScalarValue(string sql)
        {
            try
            {
                using var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
                using var command = new SqlCommand(sql, connection)
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandTimeout = 50000
                };
                connection.Open();
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // show error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }

            return null;
        }

        /// <summary>
        /// Loads the from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private static void LoadFromReader(SqlDataReader reader, DataTable dataTable, CancellationToken cancellationToken)
        {
            // Add columns to DataTable based on reader fields
            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataTable.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
            }

            // Read rows from reader
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool read;
                try
                {
                    read = reader.Read();
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    throw new InvalidOperationException("Error reading data from the reader.", ex);
                }

                if (!read) break; // Exit loop if no more rows

                var row = dataTable.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                }
                dataTable.Rows.Add(row);
            }
        }
    }
}