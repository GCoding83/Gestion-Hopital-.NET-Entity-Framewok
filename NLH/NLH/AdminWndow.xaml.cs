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
    /// Logique d'interaction pour AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            refresh();
            
            
        }

        public void refresh()
        {
            List<Dossier_Admission> dossiers = Singleton.Instance.bd.Dossier_Admission.Where(d => d.dateConge == null).OrderBy(d => d.noLit).ToList();
            dgPatientsPresents.ItemsSource = dossiers;

            List<Medecin> medecins = Singleton.Instance.bd.Medecins.ToList();
            cbMedecins.DataContext = medecins;

            List<Departement> departements = Singleton.Instance.bd.Departements.ToList();
            cbDepartementsChanger.ItemsSource = departements;

            List<Lit> litsTotaux = Singleton.Instance.bd.Lits.ToList();
            List<Lit> litsLibres = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).ToList();


            lblNbrLibres.Content = litsLibres.Count() + " lits disponibles sur un total de " + litsTotaux.Count() + " dans l'hôpital.";

            afficherNombreLits();
        }

        private void cbMedecins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medecin medecin = cbMedecins.SelectedItem as Medecin;
            List<Dossier_Admission> dossiers = Singleton.Instance.bd.Dossier_Admission.Where(d => d.idMedecin == medecin.idMedecin).Where(d => d.dateConge == null).ToList();
            dgDossiersMedecin.ItemsSource = dossiers;

            List<Medecin> autresMedecins = Singleton.Instance.bd.Medecins.Where(m => m.idMedecin != medecin.idMedecin).ToList();
            cbAutresMedecins.DataContext = autresMedecins;

            txtSpecialite.Text = medecin.specialiteMedecin;
            cbDepartementsChanger.SelectedIndex = (int)medecin.departementMedecin - 1;
        }

        private void btnENregistrer_Click(object sender, RoutedEventArgs e)
        {
            Medecin medecin = cbMedecins.SelectedItem as Medecin;
            txtSpecialite.Text = medecin.specialiteMedecin;

        }


        public void afficherNombreLits()
        {           
            lblCPr.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 1).Where(l => l.idDepartement == 1).ToList().Count.ToString();
            
            lblCSp.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 2).Where(l => l.idDepartement == 1).ToList().Count.ToString();

            lblCSt.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 3).Where(l => l.idDepartement == 1).ToList().Count.ToString();

            lblPPr.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 1).Where(l => l.idDepartement == 2).ToList().Count.ToString();

            lblPSp.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 2).Where(l => l.idDepartement == 2).ToList().Count.ToString();

            lblPSt.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 3).Where(l => l.idDepartement == 2).ToList().Count.ToString();

            lblUPr.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 1).Where(l => l.idDepartement == 3).ToList().Count.ToString();

            lblUSp.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 2).Where(l => l.idDepartement == 3).ToList().Count.ToString();

            lblUSt.Content = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == 3).Where(l => l.idDepartement == 3).ToList().Count.ToString();


        }

        private void btnChangerDpt_Click(object sender, RoutedEventArgs e)
        {
            Medecin medecin = cbMedecins.SelectedItem as Medecin;
            Departement departement = cbDepartementsChanger.SelectedItem as Departement;

            medecin.departementMedecin = departement.idDepartement;

            var reponse = MessageBox.Show("Vous souhaitez assigner le Dr. " + medecin.prenomMedecin + " " + medecin.nomMedecin + " au département "+ departement.nomDepartement + "?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (reponse == MessageBoxResult.Yes)
            {
                try
                {
                    Singleton.Instance.bd.SaveChanges();
                    MessageBox.Show("Modification effectuée.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        
        }

        private void btnRetirerMedecin_Click(object sender, RoutedEventArgs e)
        {
            Medecin medecin = cbMedecins.SelectedItem as Medecin;

            var reponse = MessageBox.Show("Vous souhaitez retirer le Dr. " + medecin.prenomMedecin + " " + medecin.nomMedecin + " du système?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            Singleton.Instance.bd.Medecins.Remove(medecin);

            //Retirer ce médecin de tous les dossiers de patients
            List<Dossier_Admission> dossiersMedecin = Singleton.Instance.bd.Dossier_Admission.Where(d => d.idMedecin == medecin.idMedecin).ToList();
            foreach (Dossier_Admission dossier in dossiersMedecin)
            {
                dossier.idMedecin = null;
            }
            
            if (reponse == MessageBoxResult.Yes)
            {
                try
                {
                    Singleton.Instance.bd.SaveChanges();
                    MessageBox.Show("Médecin effacé.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                    refresh();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }

        private void cbSansMedecin_Checked(object sender, RoutedEventArgs e)
        {
            List<Dossier_Admission> patientsOrphelins = Singleton.Instance.bd.Dossier_Admission.Where(d => d.idMedecin == null).Where(d => d.dateConge == null).ToList();
            dgDossiersMedecin.ItemsSource = patientsOrphelins;

            cbAutresMedecins.DataContext = Singleton.Instance.bd.Medecins.ToList();

            cbMedecins.IsEnabled = false;
            cbMedecins.Foreground = Brushes.Transparent;
            cbDepartementsChanger.Foreground = Brushes.Transparent;
            txtSpecialite.Foreground = Brushes.Transparent;
            cbDepartementsChanger.IsEnabled = false;
            btnChangerDpt.IsEnabled = false;
            btnChangerDpt.Foreground = Brushes.Transparent;
            btnRetirerMedecin.IsEnabled = false;
            lblGrid.Content = "Patients sans médecin";

        }

        private void cbSansMedecin_Unchecked(object sender, RoutedEventArgs e)
        {
            //Reinitialiser tous les champs et listes comme ils étaient avant d'avoir checké le checkbox.
            Medecin medecin = cbMedecins.SelectedItem as Medecin;
            List<Dossier_Admission> dossiers = Singleton.Instance.bd.Dossier_Admission.Where(d => d.idMedecin == medecin.idMedecin).Where(d => d.dateConge == null).ToList();
            dgDossiersMedecin.ItemsSource = dossiers;

            List<Medecin> autresMedecins = Singleton.Instance.bd.Medecins.Where(m => m.idMedecin != medecin.idMedecin).ToList();
            cbAutresMedecins.DataContext = autresMedecins;

            cbMedecins.IsEnabled = true;
            cbDepartementsChanger.IsEnabled = true;
            btnChangerDpt.IsEnabled = true;
            btnChangerDpt.Foreground = Brushes.Black;
            btnRetirerMedecin.IsEnabled = true;
            cbMedecins.Foreground = Brushes.Black;
            cbDepartementsChanger.Foreground = Brushes.Black;
            txtSpecialite.Foreground = Brushes.Black;
            lblGrid.Content = "Patients présentement sous charge";
        }

        private void btnAssignerMedecin_Click(object sender, RoutedEventArgs e)
        {
            Dossier_Admission dossier = dgDossiersMedecin.SelectedItem as Dossier_Admission;
            Medecin medecin = cbAutresMedecins.SelectedItem as Medecin;
            if (dossier == null)
            {
                MessageBox.Show("Veuillez sélectionner un patient dans la liste de patients.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (medecin == null)
            {
                MessageBox.Show("Veuillez sélectionner un médecin dans la liste de médecin pour assignation.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dossier.idMedecin = medecin.idMedecin;

            try
            {
                Singleton.Instance.bd.SaveChanges();
                MessageBox.Show("Médecin assigné avec succès!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                refresh();
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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NouveauMed fen = new NouveauMed();
            //Ceci est une approche que j'ai trouvée en ligne pour que les changements effectués dans mon autre fenêtre s'appliquent à cette fenêtre-ci
            //Ça me permet d'accéder aux méthodes de cette fenêtre-ci (la méthode refresh()) dans mon autre fenêtre.
            fen.Owner = this;

            fen.Show();
        }
    }
}
