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
using System.Windows.Shapes;

namespace NLH
{
    /// <summary>
    /// Logique d'interaction pour NouveauMed.xaml
    /// </summary>
    public partial class NouveauMed : Window
    {
        public NouveauMed()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbDepartement.ItemsSource = Singleton.Instance.bd.Departements.ToList();


        }

        private void btnAjouterMed_Click(object sender, RoutedEventArgs e)
        {
            Medecin med = new Medecin();
            med.nomMedecin = txtNom.Text;
            med.prenomMedecin = txtPrenom.Text;
            med.specialiteMedecin = txtSpecialite.Text;
            Departement dep = cbDepartement.SelectedItem as Departement;
            med.departementMedecin = dep.idDepartement;

            Singleton.Instance.bd.Medecins.Add(med);

            try
            {
                Singleton.Instance.bd.SaveChanges();
                MessageBox.Show("Médecin ajouté avec succès.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                //J'utilise la méthode refresh() de la fenêtre "owner" (i.e. AdminWindow).
                ((AdminWindow)this.Owner).refresh();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
