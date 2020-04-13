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
    /// Logique d'interaction pour DocWindow.xaml
    /// </summary>
    public partial class DocWindow : Window
    {
        public DocWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            refresh();
            dateLiberation.SelectedDate = DateTime.Today;
        }



        //============================Listeners ===========================
        private void cbNASLiberer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dossier_Admission dos = cbNASLiberer.SelectedItem as Dossier_Admission;
            txtNomPatientLiberer.Text = dos.Patient.nomPatient;
            txtPrenomPatientLiberer.Text = dos.Patient.prenomPatient;
            
        }

        private void btnAnnulerLiber_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLibererPatient_Click(object sender, RoutedEventArgs e)
        {
            Dossier_Admission dos = cbNASLiberer.SelectedItem as Dossier_Admission;
            
           
            if (dateLiberation.SelectedDate < dos.dateAdmission)
            {
                MessageBox.Show("Veuillez saisir une date qui ne précède pas la date d'admission du patient.");
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Êtes-vous sûr de vouloir libérer ce patient?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    //Mettre la date de congé
                    dos.dateConge = dateLiberation.SelectedDate;

                    //Enregistrer le lit comme étant non-occupé
                    int numLit = (int)dos.noLit;
                    Lit litPatient = Singleton.Instance.bd.Lits.Where(l => l.noLit == numLit).FirstOrDefault();
                    litPatient.occupe = false;

                    try
                    {
                        Singleton.Instance.bd.SaveChanges();
                        MessageBox.Show("Le patient a été libéré.");
                        afficherFacture(dos);
                        refresh();

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                }
                    
            }
        }



        //============================Methodes-Services ===========================

        public void afficherFacture(Dossier_Admission dos)
        {
            string txtAfficher = "                 FACTURE\n======================\n";
            double fraisTotaux = 0;
            double fraisPrive = 571;
            double fraisSemiPrive = 267;
            double fraisPhone = 7.50;
            double fraisTV = 42.50;

            //Si le patient doit prendre en charge ses frais de lit, ajouter le montant a sa facture
            if (dos.litPayant == true)
            {
                if (dos.Lit.noTypeLit == 2)
                {

                    fraisTotaux += fraisSemiPrive;
                    txtAfficher += "Lit semi-privé: 267.00$.\n";
                }
                else if (dos.Lit.noTypeLit == 1)
                {
                    fraisTotaux += fraisPrive;
                    txtAfficher += "Lit privé:      571.00$.\n";
                }
            }

            if (dos.telephone == true)
            {
                fraisTotaux += fraisPhone;
                txtAfficher += "Téléphone:       7.50$\n";
            }

            if (dos.television == true)
            {
                fraisTotaux += fraisTV;
                txtAfficher += "Télévision:      42.50$\n";
            }
            
            txtAfficher += "======================\nTotal:              "+fraisTotaux+"$";
            MessageBox.Show(txtAfficher);

        }

        public void refresh()
        {
            List<Dossier_Admission> dossiers = Singleton.Instance.bd.Dossier_Admission.Where(d => d.dateConge == null).ToList();
            cbNASLiberer.DataContext = dossiers;

        }
    }
}