using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace WinFormsApp2;

public partial class Form1 : Form
{
    SqlConnection? conn = null;
    int authorsId;
    public Form1()
    {
        InitializeComponent();

        conn = new SqlConnection(ConfigurationManager.AppSettings.Get("Key0"));

        AuthorsReader(comboBox1);
    }

    #region Methods
    private void AuthorsReader(ComboBox cbox)
    {
        SqlDataReader? reader = null;

        try
        {
            conn?.Open();

            using SqlCommand cmdAuthors = new SqlCommand("SELECT * FROM Authors", conn);
            reader = cmdAuthors.ExecuteReader();

            bool flag = false;

            while (reader.Read())
            {
                if (flag)
                {
                    StringBuilder field = new StringBuilder();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i == 0)
                            field.Append(reader[i].ToString() + '.');
                        else
                            field.Append(reader[i].ToString() + ' ');
                    }

                    cbox.Items.Add(field.ToString());
                }
                else
                    flag = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn?.Close();
            reader?.Close();
        }
    }
    public void CategoriesReader(ComboBox cbox)
    {
        SqlDataReader? reader = null;

        try
        {
            conn?.Open();

            using SqlCommand cmdCategories = new SqlCommand(@"SELECT DISTINCT Categories.Name FROM Books
JOIN Authors
ON Books.Id_Author = Authors.Id
JOIN Categories
ON Categories.Id = Books.Id_Category
WHERE Authors.Id = @id", conn);

            StringBuilder? value = new();

            comboBox2.Items.Clear();

            for (int i = 0; i < comboBox1.SelectedItem.ToString()?.Length; i++)
            {
                if (comboBox1.SelectedItem.ToString()?[i].ToString() != ".")
                    value.Append(comboBox1.SelectedItem.ToString()?[i].ToString());
                else
                    break;
            }

            authorsId = int.Parse(value.ToString());

            cmdCategories.Parameters.AddWithValue("@id", authorsId);

            reader = cmdCategories.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    cbox.Items.Add(reader[i].ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn?.Close();
            reader?.Close();
        }
    }
    private void BooksReader(ListView lView)
    {
        SqlDataReader? reader = null;

        try
        {
            conn?.Open();

            using SqlCommand cmdCategories = new SqlCommand(@"SELECT Books.Id, Books.Name AS [Books], Authors.FirstName + ' ' + Authors.LastName AS [FullName of Authors], Categories.Name AS [Categories] FROM Books
JOIN Authors
ON Books.Id_Author = Authors.Id
JOIN Categories
ON Categories.Id = Books.Id_Category
WHERE Categories.Name = @categoryName AND Authors.Id = @authorsId", conn);

            StringBuilder? value = new();

            lView.Items.Clear();
            lView.Columns.Clear();

            cmdCategories.Parameters.AddWithValue("@categoryName", comboBox2.SelectedItem.ToString());
            cmdCategories.Parameters.AddWithValue("@authorsId", authorsId);

            reader = cmdCategories.ExecuteReader();

            lView.View = View.Details;
            lView.Columns.Add("Number");
            lView.Columns.Add("Books Id");
            lView.Columns.Add("Books Name");
            lView.Columns.Add("FullName of Authors");
            lView.Columns.Add("Category");

            int line = 0;
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem((line + 1).ToString());

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    item.SubItems.Add(reader[i].ToString());

                }
                lView.Items.Add(item);
                ++line;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn?.Close();
            reader?.Close();
        }
    }
    private void BooksSearch(ListView lView, TextBox textBox)
    {
        SqlDataReader? reader = null;

        try
        {
            conn?.Open();

            using SqlCommand cmdCategories = new SqlCommand(@"SELECT Books.Id, Books.Name AS [Books], Authors.FirstName + ' ' + Authors.LastName AS [FullName of Authors], Categories.Name AS [Categories] FROM Books
JOIN Authors
ON Books.Id_Author = Authors.Id
JOIN Categories
ON Categories.Id = Books.Id_Category
WHERE Books.Name = @booksName OR Books.Id = @booksId", conn);

            StringBuilder? value = new();

            listView.View = View.Tile;
            lView.Items.Clear();
            lView.Columns.Clear();

            if (int.TryParse(textBox.Text, out int id))
            {
                cmdCategories.Parameters.AddWithValue("@booksId", id);
                cmdCategories.Parameters.AddWithValue("@booksName", "");
            }
            else
            {
                cmdCategories.Parameters.AddWithValue("@booksId", 0);
                cmdCategories.Parameters.AddWithValue("@booksName", textBox.Text);
            }
            reader = cmdCategories.ExecuteReader();

            lView.View = View.Details;
            lView.Columns.Add("Number");
            lView.Columns.Add("Books Id");
            lView.Columns.Add("Books Name");
            lView.Columns.Add("FullName of Authors");
            lView.Columns.Add("Category");

            int line = 0;
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem((line + 1).ToString());

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    item.SubItems.Add(reader[i].ToString());

                }
                lView.Items.Add(item);
                ++line;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            conn?.Close();
            reader?.Close();
        }
    }

    #endregion

    #region Events
    private void comboBox_SelectedValueChanged(object sender, EventArgs e)
    {
        ComboBox? cbox = sender as ComboBox;

        if (cbox == comboBox1)
            CategoriesReader(comboBox2);
        else if (cbox == comboBox2)
            BooksReader(listView);
    }
    private void button1_Click(object sender, EventArgs e)
    {
        BooksSearch(listView, textBox1);
    }
    private void textBox1_Click(object sender, EventArgs e)
    {
        textBox1.Text = "";
    }
    #endregion
}