using System;
using System.Collections.Generic;
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

namespace WpfLearn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Client_Login(object sender, RoutedEventArgs e)
        {
            Client_Login w = new Client_Login();
            w.Show();
            this.Close();
            //MessageBox.Show("Client");
        }

        private void Button_Click_Toll_Collector_Login(object sender, RoutedEventArgs e)
        {
            Toll_Collector_Login w = new Toll_Collector_Login();
            w.Show();
            this.Close();
            //MessageBox.Show("Toll Collector");
        }

        private void Button_Click_Admin_Login(object sender, RoutedEventArgs e)
        {
            Admin_Login w = new Admin_Login();
            w.Show();
            this.Close();
            //MessageBox.Show("Admin");
        }
    }
}
