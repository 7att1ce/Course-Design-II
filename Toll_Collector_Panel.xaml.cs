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
using System.IO;
using Microsoft.Win32;


namespace WpfLearn
{
    /// <summary>
    /// Toll_Collector_Panel.xaml 的交互逻辑
    /// </summary>
    public partial class Toll_Collector_Panel : Window
    {
        private MySqlConnection conn;
        private string userid;

        private static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg.IsMatch(str);
        }

        public Toll_Collector_Panel(string str)
        {
            userid = str;
            string connStr = "server=10.240.140.144;port=3306;user=tollcollector;password=tollcollector;database=course_design;";
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
            UserID.Text = userid;
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

        private void Button_Click_Balance_Search(object sender, RoutedEventArgs e)
        {
            string phone_number = PhoneNumber.Text;
            if (IsNumeric(phone_number) && phone_number.Length <= 11)
            {
                string sqlStr = "select * from clientbalance where phone like @phone_number order by phone";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number + "%");
                MySqlDataReader reader = cmd.ExecuteReader();
                BalanceResult.Text = "查询结果:\n手机号:            余额:\n";
                while (reader.Read())
                {
                    BalanceResult.Text += reader.GetString("phone") + " " + reader.GetInt32("balance").ToString() + "\n";
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("请输入正确的手机号");
            }
        }

        private void Button_Click_Balance_change(object sender, RoutedEventArgs e)
        {
            string phone_number = PhoneNumber.Text, balance_ = Balance.Text;
            if (IsNumeric(phone_number) && IsNumeric(balance_) && phone_number.Length == 11 && balance_.Length <= 6)
            {
                string sqlStr = "update clientbalance set balance=@balance_ where phone=@phone_number";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                cmd.Parameters.AddWithValue("@balance_", Convert.ToInt32(balance_));
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("修改成功 结果请重新查询");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请正确输入信息 余额不能大于999999");
            }
        }

        private void Button_Click_Toll_Search(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            if (IsNumeric(phone_number) && IsNumeric(start_date) && IsNumeric(end_date) && phone_number.Length <= 11 && start_date.Length >= 8 && end_date.Length >= 8)
            {
                string sqlStr = "select * from toll where startdate>=@start_date and enddate<=@end_date and phone like @phone_number order by phone, startdate, enddate";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
                cmd.Parameters.AddWithValue("@phone_number", phone_number + "%");
                MySqlDataReader reader = cmd.ExecuteReader();
                TollResult.Text = "查询结果\n手机号:            开始日期:              截止日期:              收费:\n";
                while (reader.Read())
                {
                    TollResult.Text += reader.GetString("phone") + " " + reader.GetString("startdate") + " " + reader.GetString("enddate") + " " + reader.GetInt32("cost").ToString() + "\n";
                }
                reader.Close();

                string sqlStr1 = "select sum(cost) from toll where startdate>=@StartDate and enddate<=@EndDate group by phone having phone=@phone_number";
                MySqlCommand cmd1 = new MySqlCommand(sqlStr1, conn);
                cmd1.Parameters.AddWithValue("@StartDate", start_date);
                cmd1.Parameters.AddWithValue("@EndDate", end_date);
                cmd1.Parameters.AddWithValue("@phone_number", phone_number);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    TollResult.Text += "总收费: " + reader1.GetInt32("sum(cost)").ToString();
                }
                reader1.Close();
            }
            else
            {
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_Click_Toll_Add(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text, cost = TollCharge.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            bool flag1 = IsNumeric(phone_number) && IsNumeric(cost) && IsNumeric(start_date) && IsNumeric(end_date);
            bool flag2 = phone_number.Length == 11 && start_date.Length == 14 && end_date.Length == 14 && cost.Length <= 4;
            if (flag1 && flag2)
            {
                string sqlStr = "insert into toll value (@phone_number, @start_date, @end_date, @cost)";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
                cmd.Parameters.AddWithValue("@cost", Convert.ToInt32(cost));
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("插入成功");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_CLick_Toll_Delete(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            bool flag1 = IsNumeric(phone_number) && IsNumeric(start_date) && IsNumeric(end_date);
            bool flag2 = phone_number.Length == 11 && start_date.Length >= 8 && end_date.Length >= 8;
            if (flag1 && flag2)
            {
                string sqlStr = "delete from toll where startdate>=@start_date and enddate<=@end_date and phone=@phone_number";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
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
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_Click_Toll_Update_Start_Date(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text, cost = TollCharge.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            string sqlStr = "update toll set startdate=@start_date where phone=@phone_number and enddate=@end_date and cost=@cost";
            bool flag1 = IsNumeric(phone_number) && IsNumeric(cost) && IsNumeric(start_date) && IsNumeric(end_date);
            bool flag2 = phone_number.Length == 11 && start_date.Length == 14 && end_date.Length == 14 && cost.Length <= 4;
            if (flag1 && flag2)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
                cmd.Parameters.AddWithValue("@cost", Convert.ToInt32(cost));
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
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_Click_Toll_Update_End_Date(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text, cost = TollCharge.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            string sqlStr = "update toll set enddate=@end_date where phone=@phone_number and startdate=@start_date and cost=@cost";
            bool flag1 = IsNumeric(phone_number) && IsNumeric(cost) && IsNumeric(start_date) && IsNumeric(end_date);
            bool flag2 = phone_number.Length == 11 && start_date.Length == 14 && end_date.Length == 14 && cost.Length <= 4;
            if (flag1 && flag2)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
                cmd.Parameters.AddWithValue("@cost", Convert.ToInt32(cost));
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
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_Click_Toll_Update_Cost(object sender, RoutedEventArgs e)
        {
            string phone_number = TollPhoneNumber.Text, cost = TollCharge.Text;
            string start_date = TollStartDate.Text, end_date = TollEndDate.Text;
            string sqlStr = "update toll set cost=@cost where phone=@phone_number and startdate=@start_date and enddate=@end_date";
            bool flag1 = IsNumeric(phone_number) && IsNumeric(cost) && IsNumeric(start_date) && IsNumeric(end_date);
            bool flag2 = phone_number.Length == 11 && start_date.Length == 14 && end_date.Length == 14 && cost.Length <= 4;
            if (flag1 && flag2)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                cmd.Parameters.AddWithValue("@start_date", start_date);
                cmd.Parameters.AddWithValue("@end_date", end_date);
                cmd.Parameters.AddWithValue("@cost", Convert.ToInt32(cost));
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
                MessageBox.Show("请输入正确格式的信息");
            }
        }

        private void Button_Click_Export(object sender, RoutedEventArgs e)
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

        private void Button_Click_Export_Client(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "表格|*.csv";
            saveFileDialog.FileName = "客户余额信息.csv";
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }
            string path = saveFileDialog.FileName;
            string sqlStr = "select * from clientbalance";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("phone_number,balance");
                while (reader.Read())
                {
                    sw.WriteLine(reader.GetString("phone") + "," + reader.GetInt32("balance").ToString());
                }
            }
            reader.Close();
        }
    }
}
