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

namespace NLH
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnecter_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomUsager.Text=="med" && txtMotPasse.Password == "med" || txtNomUsager.Text == "doc" && txtMotPasse.Password == "doc")
            {
                MessageBox.Show("Bienvenue Docteur!");
                DocWindow win = new DocWindow();
                win.Top = 200;
                win.Left = 500;
                win.WindowStyle = WindowStyle.None;
                win.ResizeMode = ResizeMode.NoResize;
                win.Show();
                    
            }
            else if (txtNomUsager.Text == "prep" && txtMotPasse.Password == "prep")
            {
                MessageBox.Show("Bienvenue Préposé/e!", "Ouverture", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                PrepWindow win = new PrepWindow();
                win.Top = 50 ;
                win.Left = 300;
                win.WindowStyle = WindowStyle.None;
                win.ResizeMode = ResizeMode.NoResize;
                win.Show();

            }
           
            else if (txtNomUsager.Text == "admin" && txtMotPasse.Password == "admin")
            {
                MessageBox.Show("Bienvenue Administrateur!");
                AdminWindow win = new AdminWindow();
                win.Top = 50;
                win.Left = 300;
                win.WindowStyle = WindowStyle.None;
                win.ResizeMode = ResizeMode.NoResize;
                win.Show();

            }
            else
            {
                MessageBox.Show("Authentification inexacte. Veuillez recommencer.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
