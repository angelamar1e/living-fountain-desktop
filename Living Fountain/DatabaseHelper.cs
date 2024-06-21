using System;
using System.Data;
using System.Data.SqlClient;

public class DatabaseHelper
{
    private string connectionString = "Data Source=localhost;Initial Catalog=living_fountain;Integrated Security=True;TrustServerCertificate=True";

    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataTable);
        }

        return dataTable;
    }
}
