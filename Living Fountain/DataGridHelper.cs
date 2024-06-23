using System.Data;
using System.Windows.Controls;

public static class DataGridHelper
{
    private static readonly DatabaseHelper db = new DatabaseHelper();

    public static void LoadData(DataGrid dataGrid, string query, params string[] headers)
    {
        DataTable data = db.ExecuteQuery(query);
        dataGrid.ItemsSource = data.DefaultView;
    }
}
