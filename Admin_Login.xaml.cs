using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace WpfLearn
{
    /// <summary>
    /// Admin_Login.xaml 的交互逻辑
    /// </summary>
    public partial class Admin_Login : Window
    {
        private static string GetSHA256(string input)
        {
            byte[] data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public Admin_Login()
        {
            InitializeComponent();
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }

        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            string userid = this.UserID.Text, password = GetSHA256(this.Password.Password);
            string connStr = "server=10.240.140.144;port=3306;user=admin;password=admin;database=course_design;";
            bool flag = false;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string sqlStr = "select * from admin where userid=@userid and password=@password";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    MessageBox.Show("登录成功");
                    flag = true;
                }
                else 
                {
                    MessageBox.Show("登录失败 请检查用户名或密码");
                    flag = false;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (flag)
            {
                Admin_Panel w = new Admin_Panel();
                w.Show();
                this.Close();
            }
        }
    }
}
