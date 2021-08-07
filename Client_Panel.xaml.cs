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
using Microsoft.Win32;
using System.IO;

namespace WpfLearn
{
    /// <summary>
    /// Client_Panel.xaml 的交互逻辑
    /// </summary>
    public partial class Client_Panel : Window
    {
        private string phone_number;
        private MySqlConnection conn;

        private static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg.IsMatch(str);
        }

        public Client_Panel(string phone)
        {
            phone_number = phone;
            string connStr = "server=10.240.140.144;port=3306;user=client;password=client;database=course_design;";
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
            PhoneNumber.Text = phone_number;
            string sqlStr = "select balance from clientbalance where phone=@phone_number";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            cmd.Parameters.AddWithValue("@phone_number", phone_number);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Balance.Text = reader.GetInt32("balance").ToString();
            }
            else
            {
                MessageBox.Show("读取余额错误, 请重新登录");
            }
            reader.Close();
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

        private void Button_Click_Flush_Balance(object sender, RoutedEventArgs e)
        {
            string sqlStr = "select balance from clientbalance where phone=@phone_number";
            MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
            cmd.Parameters.AddWithValue("@phone_number", phone_number);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Balance.Text = reader.GetInt32("balance").ToString();
            }
            else
            {
                MessageBox.Show("读取余额错误, 请重试或重新登录");
            }
            reader.Close();
        }

        private void TextBox_GotFocus_Payment(object sender, RoutedEventArgs e)
        {
            if (Payment.Text == "请输入充值金额")
            {
                Payment.Text = "";
            }
        }

        private void TextBox_LostFocus_Payment(object sender, RoutedEventArgs e)
        {
            if (Payment.Text == "")
            {
                Payment.Text = "请输入充值金额";
            }
        }

        private void Button_Click_Pay(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(Payment.Text))
            {
                try
                {
                    int payment = Convert.ToInt32(Payment.Text);
                    if (payment > 0 && payment <= 1000)
                    {
                        string sqlStr = "update clientbalance set balance=@payment where phone=@phone_number";
                        MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                        cmd.Parameters.AddWithValue("@payment", payment + Convert.ToInt32(Balance.Text));
                        cmd.Parameters.AddWithValue("@phone_number", phone_number);
                        try
                        {
                            cmd.ExecuteNonQuery(); 
                            MessageBox.Show("充值成功 请选择发票保存路径");
                            string paydate = DateTime.Now.ToString();
                            //选择保存文件路径
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.DefaultExt = "txt";
                            saveFileDialog.Filter = "文本|*.txt";
                            saveFileDialog.FileName = "发票.txt";
                            if (!(bool)saveFileDialog.ShowDialog())
                            {
                                return;
                            }
                            string path = saveFileDialog.FileName;
                            //
                            using (StreamWriter sw = File.CreateText(path))
                            {
                                sw.WriteLine("用户手机号: " + phone_number);
                                sw.WriteLine("充值时间: " + paydate);
                                sw.WriteLine("充值金额: " + payment);
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message); 
                        }
                    }
                    else
                    {
                        MessageBox.Show("请输入不大于1000的正整数");
                    }
                }
                catch (OverflowException)
                {
                    MessageBox.Show("请输入不大于1000的正整数");
                }
            }
            else
            {
                MessageBox.Show("请输入不大于1000的正整数");
            }
        }

        private void Button_Click_Toll_Search(object sender, RoutedEventArgs e)
        {
            if (!IsNumeric(StartDate.Text) || !IsNumeric(EndDate.Text) || StartDate.Text.Length < 8 || EndDate.Text.Length < 8)
            {
                MessageBox.Show("日期输入不合法");
            }
            else
            {
                string sqlStr = "select * from toll where startdate>=@StartDate and enddate<=@EndDate and phone=@phone_number order by startdate, enddate";
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                cmd.Parameters.AddWithValue("@StartDate", StartDate.Text);
                cmd.Parameters.AddWithValue("@EndDate", EndDate.Text);
                cmd.Parameters.AddWithValue("@phone_number", phone_number);
                MySqlDataReader reader = cmd.ExecuteReader();
                Result.Text = "查询结果:\n开始日期:               截止日期:              收费:\n";
                while (reader.Read())
                {
                    Result.Text += reader.GetString("startdate") + " " + reader.GetString("enddate") + " " + reader.GetInt32("cost").ToString() + "\n";
                }
                reader.Close();

                string sqlStr1 = "select sum(cost) from toll where startdate>=@StartDate and enddate<=@EndDate group by phone having phone=@phone_number";
                MySqlCommand cmd1 = new MySqlCommand(sqlStr1, conn);
                cmd1.Parameters.AddWithValue("@StartDate", StartDate.Text);
                cmd1.Parameters.AddWithValue("@EndDate", EndDate.Text);
                cmd1.Parameters.AddWithValue("@phone_number", phone_number);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    Result.Text += "总收费: " + reader1.GetInt32("sum(cost)").ToString();
                }
                reader1.Close();
            }
        }
    }
}
