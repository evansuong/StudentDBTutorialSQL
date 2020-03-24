using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentDBTutorialSQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SQL objects
        private const string CONSTRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\evans\Documents\dbStudentInfoTutorial.mdf;Integrated Security = True; Connect Timeout = 30";
        private SqlConnection con;

        public MainWindow()
        {
            InitializeComponent();
            con = new SqlConnection(CONSTRING);
            Load_Table_Data();
        }

        private void Load_Table_Data()
        {
            if (con != null && con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                // build command and execute 
                string qry = "select stuId as ID, sName as Name, sAddress as Address, sPhone Phone, sYear as Year, sMajor as Major from StudentInfo";

                // build data adapter and use to fill data table
                SqlDataAdapter sda = new SqlDataAdapter(qry, con);
                DataTable dt = new DataTable("Student Info");
                sda.Fill(dt);
                StuInfoDG.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            con.Close();
          }

        private void ClearAll()
        {
            // clear text boxes
            StuNameTB.Text = "";
            StuAddressTB.Text = "";
            StuPhoneTB.Text = "";
            StuSearchTB.Text = "";
            StuIdTB.Text = "";

            // clear comboboxes
            StuYearCB.SelectedIndex = -1;
            StuMajorCB.SelectedIndex = -1;

            if (StuIdTB.IsReadOnly)
            {
                StuIdTB.IsReadOnly = false;
            }
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // open connection
                con.Open();

                // get text from textboxes
                string stuId = StuIdTB.Text.ToString();
                int stuIdI = Int32.Parse(stuId);
                string stuName = StuNameTB.Text.ToString();
                string stuAddress = StuAddressTB.Text.ToString();
                string stuPhone = StuPhoneTB.Text.ToString();
                long stuPhoneI = long.Parse(stuPhone);

                // get student info from combobox
                string stuYear = StuYearCB.SelectedItem.ToString();
                stuYear = stuYear.Substring(stuYear.IndexOf(":") + 2);
                int stuYearI = Int32.Parse(stuYear);

                string stuMajor = StuMajorCB.SelectedItem.ToString();
                stuMajor = stuMajor.Substring(stuMajor.IndexOf(":") + 2);

                // debug message
                //MessageBox.Show(stuName + stuAddress + stuPhoneI + stuYearI + stuMajor);

                // create query command
                string qry = "insert into StudentInfo values('"+stuIdI+"','"+stuName+"','"+stuAddress+"','"+stuPhoneI+"','"+stuYearI+"','"+stuMajor+"')";
                SqlCommand sc = new SqlCommand(qry, con);

                // execute query statement
                int i = sc.ExecuteNonQuery();

                // debug message
                if (i < 1)
                {
                    MessageBox.Show("student not registered");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("error is" + err.ToString());
            }

            // update the table and close connection
            Load_Table_Data();
            con.Close();
            ClearAll();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StuInfoDG.SelectedItems.Count == 1)
                {
                    con.Open();
                    string stuName = StuNameTB.Text.ToString();
                    string stuAddress = StuAddressTB.Text.ToString();

                    string stuPhone = StuPhoneTB.Text.ToString();
                    long stuPhoneI = Int64.Parse(stuPhone);

                    string stuYear = StuYearCB.Text.ToString();
                    int stuYearI = Int32.Parse(stuYear);

                    string stuMajor = StuMajorCB.SelectedItem.ToString();

                    string stuId = StuIdTB.Text.ToString();
                    int stuIdI = Int32.Parse(stuId);

                    string qry = "update StudentInfo set sName= '" + stuName + "', sAddress='" + stuAddress + "', sPhone=" + stuPhoneI + ", sYear=" + stuYearI + ", sMajor='" + stuMajor + "' where stuId='" + stuId + "'";
                    SqlCommand sc = new SqlCommand(qry, con);
                    int i = sc.ExecuteNonQuery();

                    if (i >= 1)
                        MessageBox.Show(i + " Student Updated Successfully : " + stuName);
                    else
                        MessageBox.Show(" Student  Updation Failed..... ");
                    ClearAll();
                    Load_Table_Data();
                    con.Close();
                }
            }
            catch (System.Exception exp)
            {
                MessageBox.Show(" Error is  " + exp.ToString());

            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // open connection
                con.Open();

                // get student id of selected student
                string stuId = StuIdTB.Text.ToString();
                int stuIdI = Int32.Parse(stuId);

                // create delete query command
                string qry = "delete from StudentInfo where stuId='" + stuIdI + "'";
                SqlCommand sc = new SqlCommand(qry, con);

                // execute query statement
                int i = sc.ExecuteNonQuery();

                // debug message
                if (i < 1)
                {
                    MessageBox.Show("student not registered");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("error is" + err.ToString());
            }

            // update the table and close connection
            Load_Table_Data();
            con.Close();
            ClearAll();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            con.Open();

            // get student id from search box
            string stuId = StuSearchTB.Text.ToString();

            if (stuId != "")
            {
                // build and execute select query according to id
                string qry = "select stuId as ID, sName as Name, sAddress as Address, sPhone Phone, sYear as Year, sMajor as Major from StudentInfo where stuId = " + stuId;
                SqlCommand sc = new SqlCommand(qry, con);
                SqlDataReader sdr = sc.ExecuteReader();

                // load searched for student in daragrid
                if (sdr.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                    StuInfoDG.ItemsSource = dt.DefaultView;
                }
                else
                {
                    sdr.Close();
                    MessageBox.Show("No Student Found with ID: " + stuId); ;
                    Load_Table_Data();
                }
            }

            // if search is not formatted laod normal search data
            else
            {
                Load_Table_Data();
            } 
            con.Close();
        }

        private void StuInfoDG_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // fill all text boxes with selected student info
            if (StuInfoDG.SelectedItems.Count == 1)
            {
                var items = StuInfoDG.SelectedItems;
                foreach (DataRowView item in items)
                {
                    StuNameTB.Text = item["Name"].ToString();
                    StuAddressTB.Text = item["Address"].ToString();
                    StuPhoneTB.Text = item["Phone"].ToString();

                    StuYearCB.Text = item["Year"].ToString();
                    StuMajorCB.Text = item["Major"].ToString();

                    StuIdTB.Text = item["ID"].ToString();

                    StuIdTB.IsReadOnly = true;
                }
            }
        }
    }
}
