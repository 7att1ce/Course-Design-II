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
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace WpfLearn
{
    /// <summary>
    /// Admin_Panel.xaml 的交互逻辑
    /// </summary>
    public partial class Admin_Panel : Window
    {
        private MySqlConnection conn;

        private static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg.IsMatch(str);
        }

        private static bool IsUserIDLegal(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^([_a-zA-Z0-9])\w+$");
            return reg.IsMatch(str);
        }

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

        public Admin_Panel()
        {
            string connStr = "server=10.240.140.144;port=3306;user=admin;password=admin;database=course_design;";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
            InitializeComponent();
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            conn.Close();
        }

        private void Button_Click_Log_Out(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }

        private void Button_Click_Search_All_Client(object sender, RoutedEventArgs e)
        {
            string sqlStr = "select * from client";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            Result.Text = "查询结果\n";
            while (reader.Read())
            {
                Result.Text += reader.GetString("phone") + " " + reader.GetString("password").Substring(0, 5) + "\n";
            }
            reader.Close();
        }

        private void Button_Click_Search_Client(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(Client_Search.Text) && Client_Search.Text.Length > 0)
            {
                string sqlStr = "select * from client where phone like @phone_number";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", Client_Search.Text + "%");
                MySqlDataReader reader = cmd.ExecuteReader();
                Result.Text = "查询结果\n";
                while (reader.Read())
                {
                    Result.Text += reader.GetString("phone") + " " + reader.GetString("password").Substring(0, 5) + "\n";
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("请正确输入手机号");
            }
        }

        private void Button_Click_Create_Client(object sender, RoutedEventArgs e)
        {
            string phone_number = Create_Client_Phone_Number.Text;
            string password = Create_Client_Password.Password;
            string password_check = Create_Client_Password_Check.Password;
            if (IsNumeric(phone_number) && phone_number.Length == 11)
            {
                if (password.Length > 0 && password_check.Length > 0 && password == password_check)
                {
                    string sqlStr = "insert into client value (@phone_number, @password)";
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                    cmd.Parameters.AddWithValue("@phone_number", phone_number);
                    cmd.Parameters.AddWithValue("@password", GetSHA256(password));
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("创建成功");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请正确输入密码");
                }
            }
            else
            {
                MessageBox.Show("请输入正确的手机号");
            }
        }

        private void Button_Click_Update_Client(object sender, RoutedEventArgs e)
        {
            string phone_number = Update_Client_Phone_Number.Text;
            string password = Update_Client_Password.Password;
            string password_check = Update_Client_Password_Check.Password;
            if (IsNumeric(phone_number) && phone_number.Length == 11)
            {
                if (password.Length > 0 && password_check.Length > 0 && password == password_check)
                {
                    string sqlStr = "update client set password=@password where phone=@phone_number";
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                    cmd.Parameters.AddWithValue("@phone_number", phone_number);
                    cmd.Parameters.AddWithValue("@password", GetSHA256(password));
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("修改成功");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请正确输入密码");
                }
            }
            else
            {
                MessageBox.Show("请输入正确的手机号");
            }
        }

        private void Button_Click_Delete_Client(object sender, RoutedEventArgs e)
        {
            string phone_number = Delete_Client_Phone_Number.Text;
            if (IsNumeric(phone_number) && phone_number.Length == 11)
            {
                string sqlStr = "delete from client where phone=@phone_number";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("删除成功");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请输入正确的手机号");
            }
        }

        private void Button_Click_Search_All_Toll(object sender, RoutedEventArgs e)
        {
            string sqlStr = "select * from tollcollector";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            Result.Text = "查询结果\n";
            while (reader.Read())
            {
                Result.Text += reader.GetString("userid") + " " + reader.GetString("password").Substring(0, 5) + "\n";
            }
            reader.Close();
        }

        private void Button_Click_Search_Toll(object sender, RoutedEventArgs e)
        {
            if (Toll_Search.Text.Length > 0)
            {
                string sqlStr = "select * from tollcollector where userid like @userid";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@userid", "%" + Toll_Search.Text + "%");
                MySqlDataReader reader = cmd.ExecuteReader();
                Result.Text = "查询结果\n";
                while (reader.Read())
                {
                    Result.Text += reader.GetString("userid") + " " + reader.GetString("password").Substring(0, 5) + "\n";
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("请正确输入用户名");
            }
        }

        private void Button_Click_Create_Toll(object sender, RoutedEventArgs e)
        {
            string userid = Create_Toll_Phone_Number.Text;
            string password = Create_Toll_Password.Password;
            string password_check = Create_Toll_Password_Check.Password;
            if (IsUserIDLegal(userid) && userid.Length > 0)
            {
                if (password.Length > 0 && password_check.Length > 0 && password == password_check)
                {
                    string sqlStr = "insert into tollcollector value (@userid, @password)";
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@password", GetSHA256(password));
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("创建成功");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请正确输入密码");
                }
            }
            else
            {
                MessageBox.Show("请输入合法的用户名 用户名只允许大小写字母, 数字或下划线");
            }
        }

        private void Button_Click_Update_Toll(object sender, RoutedEventArgs e)
        {
            string userid = Update_Toll_Phone_Number.Text;
            string password = Update_Toll_Password.Password;
            string password_check = Update_Toll_Password_Check.Password;
            if (IsUserIDLegal(userid) && userid.Length > 0)
            {
                if (password.Length > 0 && password_check.Length > 0 && password == password_check)
                {
                    string sqlStr = "update tollcollector set password=@password where userid=@userid";
                    MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@password", GetSHA256(password));
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("修改成功");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请正确输入密码");
                }
            }
            else
            {
                MessageBox.Show("请输入正确的用户名");
            }
        }

        private void Button_Click_Delete_Toll(object sender, RoutedEventArgs e)
        {
            string userid = Delete_Toll_Phone_Number.Text;
            if (IsUserIDLegal(userid) && userid.Length > 0)
            {
                string sqlStr = "delete from tollcollector where userid=@userid";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@userid", userid);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("删除成功");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请输入正确的用户名");
            }
        }

        private void Button_Click_Backup_Client(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "表格|*.csv";
            saveFileDialog.FileName = "客户信息.csv";
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }
            string path = saveFileDialog.FileName;
            string sqlStr = "select client.*, clientbalance.balance from client, clientbalance where client.phone=clientbalance.phone";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("phone_number,password,balance");
                while (reader.Read())
                {
                    sw.WriteLine(reader.GetString("phone") + "," + reader.GetString("password") + "," + reader.GetInt32("balance").ToString());
                }
            }
            reader.Close();
        }

        private void Button_Click_Backup_TollCollector(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "表格|*.csv";
            saveFileDialog.FileName = "收费员信息.csv";
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }
            string path = saveFileDialog.FileName;
            string sqlStr = "select * from tollcollector";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("userid,password");
                while (reader.Read())
                {
                    sw.WriteLine(reader.GetString("userid") + "," + reader.GetString("password"));
                }
            }
            reader.Close();
        }

        private void Button_Click_Backup_Toll(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "表格|*.csv";
            saveFileDialog.FileName = "收费记录信息.csv";
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }
            string path = saveFileDialog.FileName;
            string sqlStr = "select * from toll";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("phone,startdate,enddate,cost");
                while (reader.Read())
                {
                    sw.WriteLine(reader.GetString("phone") + "," + reader.GetString("startdate") + "," + reader.GetString("enddate") + "," + reader.GetInt32("cost").ToString());
                }
            }
            reader.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var process = new Process())
                {
                    string cmdline = "debian run \"mysqldump -uroot -proot -h 10.240.140.144 course_design > /mnt/c/Users/yoooo/Desktop/backup.sql\"";
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.StandardInput.AutoFlush = true;
                    process.StandardInput.WriteLine(cmdline + "&exit");
                    process.WaitForExit();
                    process.Close();
                    MessageBox.Show("备份成功, 文件保存在桌面");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var process = new Process())
                {
                    string cmdline = "debian run \"mysql -uroot -proot -h 10.240.140.144 course_design < /mnt/c/Users/yoooo/Desktop/backup.sql\"";
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.StandardInput.AutoFlush = true;
                    process.StandardInput.WriteLine(cmdline + "&exit");
                    process.WaitForExit();
                    process.Close();
                    MessageBox.Show("恢复成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
