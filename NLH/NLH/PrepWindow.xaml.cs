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
    /// Logique d'interaction pour PreposeWindow.xaml
    /// </summary>
    public partial class PrepWindow : Window
    {
        public static int agePatient = -1;
        public PrepWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAssuranceNomComplet.ItemsSource = Singleton.Instance.bd.Compagnie_Assurance.ToList();
            cbMedecinNomCompletPrep.DataContext = Singleton.Instance.bd.Medecins.ToList();
            cbTypeLitPrep.DataContext = Singleton.Instance.bd.Type_Lit.ToList();
            cbTypePrefere.DataContext = Singleton.Instance.bd.Type_Lit.ToList();
            cbDepartementPrep.DataContext = Singleton.Instance.bd.Departements.ToList();
            reinitialiserSectionDossier();
            reinitialiserSectionParent();
            reinitialiserSectionPatient();
            desactiverSectionDossier();
            desactiverSectionParent();

            cbAssuranceNomComplet.SelectedIndex = 0;
        }








        //==============Methodes-Servies==========================

        public int calculerAge(DateTime dateNaissance)
        {
            int age = DateTime.Now.Year - dateNaissance.Year;
            if (DateTime.Now.DayOfYear < dateNaissance.DayOfYear)
            {
                age = age - 1;
            }
            //Assigner la valeur a la variable globale agePatient
            agePatient = age;
            return agePatient;
        }

        public static int rechercherLits(int idDep, int idType)
        {
            //NOTE: Une valeur de 0 pour l'un ou l'autre parametre signifie qu'on souhaite inclure TOUS les resultats dans la recherche
            int nbrLits = 0;
            if (idDep == 0)
            {
                //Obtenir lits disponibles au complet dans l'hopital
                if (idType == 0)
                {
                    nbrLits = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).ToList().Count();
                }

                //Obtenir lits disponibles pour le type de lit en argument
                else
                {
                    nbrLits = Singleton.Instance.bd.Lits.Where(l => l.noTypeLit == idType).Where(l => l.occupe == false).ToList().Count();

                }
            }
            else
            {
                //Obtenir lits disponibles pour le type de departement en argument
                if (idType == 0)
                {
                    nbrLits = Singleton.Instance.bd.Lits.Where(l => l.idDepartement == idDep).Where(l => l.occupe == false).ToList().Count();

                }

                //Obtenir lits disponibles pour le type le lit ET le type de departement en arguments
                else
                {
                    nbrLits = Singleton.Instance.bd.Lits.Where(l => l.noTypeLit == idType).Where(l => l.idDepartement == idDep).Where(l => l.occupe == false).ToList().Count();

                }
            }
            return nbrLits;
        }

        public void afficherLits(int idDep, int idType)
        {
            //NOTE: Une valeur de 0 pour l'un ou l'autre parametre signifie qu'on souhaite inclure TOUS les resultats dans la recherche

            if (idDep == 0)
            {
                //Obtenir lits disponibles au complet dans l'hopital
                if (idType == 0)
                {

                    cbLitsDispos.DataContext = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).ToList();
                    cbTypeLitPrep.SelectedItem = null;
                    cbDepartementPrep.SelectedItem = null;

                }

                //Obtenir lits disponibles pour le type de lit en argument
                else
                {

                    cbLitsDispos.DataContext = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == idType).ToList();                    cbTypeLitPrep.SelectedIndex = idType - 1;
                    cbDepartementPrep.SelectedItem = null;

                }
            }
            else
            {
                //Obtenir lits disponibles pour le type de departement en argument
                if (idType == 0)
                {

                    cbLitsDispos.DataContext = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.idDepartement == idDep).ToList();
                    cbTypeLitPrep.SelectedItem = null;
                    cbDepartementPrep.SelectedIndex = idDep - 1;

                }

                //Obtenir lits disponibles pour le type le lit ET le type de departement en arguments
                else
                {
                    cbLitsDispos.DataContext = Singleton.Instance.bd.Lits.Where(l => l.occupe == false).Where(l => l.noTypeLit == idType).Where(l=>l.idDepartement == idDep).ToList();
                    cbTypeLitPrep.SelectedIndex = idType - 1;
                    cbDepartementPrep.SelectedIndex = idDep - 1;

                }
            }
        }

        public void preSelectionnerLitsDispos(int idTypePrefere)
        {

            int idDpt = 3;          //Departement Urgence par defaut

            string msgAfficher = "";


            //Si le patient a 16 ans ou moins, essayer de lui assigner le departement de pediatrie
            if (agePatient <= 16)
            {
                idDpt = 2;
                msgAfficher += "Ce patient a moins de 16 ans. Il/Elle doit idéalement être dans le département de pédiatrie.\n";
            }

            //Mentionner le type de lit prefere par ce patient
            if (idTypePrefere == 0)
            {
                msgAfficher += "Ce patient n'a pas de type de lit préféré.\n";
                //Assigner type standard
                idTypePrefere = 3;
            }
            else
            {
                msgAfficher += "Ce patient préfère avoir un lit " + obtenirNomTypeLit(idTypePrefere) + ". ";
            }
            

            //Verifier si ce type de lit dans ce departement est disponible
            if (rechercherLits(idDpt, idTypePrefere) != 0)
            {
                afficherLits(idDpt, idTypePrefere);
                msgAfficher += "Il y a acutellement " + rechercherLits(idDpt, idTypePrefere) + " lit(s) " + obtenirNomTypeLit(idTypePrefere) + "(s) disponible(s) dans le département " + obtenirNomDepartement(idDpt) + ". Nous avons présélectionné un lit correspondant à ces critères. Changer cette sélection uniquement si une chirurgie est prévue. ";
            }
            //Sinon
            else
            {
                msgAfficher += "Malheureusement, il n'y a actuellement pas de lit " + obtenirNomTypeLit(idTypePrefere) + " de disponible dans le département " + obtenirNomDepartement(idDpt) + ".\n";
                msgAfficher += "Sélectionnez un autre lit qui conviendrait pour ce patient.\n";
                msgAfficher += "Tous departements confonfus, il y a présentement " + rechercherLits(0, idTypePrefere) + " lit(s) " + obtenirNomTypeLit(idTypePrefere) + "(s) dans l'hôpital. ";
                msgAfficher += "Et tout types de lit confonfus, il y a présentement " + rechercherLits(idDpt, 0) + "lit(s) de disponibles dans le departement " + obtenirNomDepartement(idDpt) + ".";
            }

            //Afficher le message.
            MessageBox.Show(msgAfficher, "Info Lits", MessageBoxButton.OK, MessageBoxImage.Warning);

        }


        public string obtenirNomDepartement(int idDept)
        {
            var nomDpt =

                        (from d in Singleton.Instance.bd.Departements
                         where d.idDepartement == idDept
                         select new { d.nomDepartement }).SingleOrDefault();

            return nomDpt.nomDepartement;
        }

        public string obtenirNomTypeLit(int idTypeLit)
        {
            var nomType =

                        (from t in Singleton.Instance.bd.Type_Lit
                         where t.idTypeLit == idTypeLit
                         select new { t.descTypeLit }).SingleOrDefault();

            return nomType.descTypeLit.ToLower();
        }

        public bool determinerSiPayant(int numLit)
        {
            //Verifier le type de lit selectionne (1=Prive, 2=Semi-Prive, 3=Standard)
            Lit litPatient = Singleton.Instance.bd.Lits.Where(l => l.noLit == numLit).SingleOrDefault();
            int numType = litPatient.noTypeLit;

            //Si le patient est assure, il ne paye pas
            if (ouiAssurance.IsChecked == true)
            {
                return false;
            }

            //Si le type de lit est standard, il ne paye pas
            if (numType == 3)
            {
                return false;
            }

            //S'il reste des lits standards dans l'hopital (et que les deux conditions precedentes ne s'appliquent pas), il paye
            if (rechercherLits(0, 3) > 0)
            {
                return true;
            }
            else
            {
                //Si le type de lit selectionne est Prive et qu'il reste des lit Semi-Prives, il paye
                if (rechercherLits(0,2) > 0 && numType == 1)
                {
                    return true;
                }
            }

            return true;
        }






        //==============================Methodes pour gerer l'affichage=========================
        public void desactiverSectionPatient()
        {
            txtNomPatientAjouter.IsEnabled = false;
            txtPrenomPatientAjouter.IsEnabled = false;
            txtCPAjouter.IsEnabled = false;
            txtNASPatient.IsEnabled = false;
            txtAdressePatient.IsEnabled = false;
            txtVilleAjouter.IsEnabled = false;
            txtTelephoneAjouter.IsEnabled = false;
            cbProvinceAjouter.IsEnabled = false;
            cbAssuranceNomComplet.IsEnabled = false;
            cbTypePrefere.IsEnabled = false;
            dtNaissanceAjouter.IsEnabled = false;
            nonAssurance.IsEnabled = false;
            ouiAssurance.IsEnabled = false;
            ouiLitPref.IsEnabled = false;
            nonLitPref.IsEnabled = false;

            btnAjouterPatientPrep.IsEnabled = false;
        }

        public void desactiverSectionParent()
        {
            txtPrenomParent.IsEnabled = false;
            txtNomParent.IsEnabled = false;
            txtAdresseParent.IsEnabled = false;
            txtVilleParent.IsEnabled = false;
            cbProvinceParent.IsEnabled = false;
            txtCPParent.IsEnabled = false;
            txtTelephoneParent.IsEnabled = false;

            btnEnregistrerParent.IsEnabled = false;
            btnViderParent.IsEnabled = false;
        }

        public void desactiverSectionDossier(bool contenu = false)
        {
            dtAdmissionPatientPrep.IsEnabled = false;
            dtCongePatientPrep.IsEnabled = false;
            dtChirurgie.IsEnabled = false;
            cbTypeLitPrep.IsEnabled = false;
            cbTypeLitPrep.Foreground = Brushes.Transparent;
            cbDepartementPrep.IsEnabled = false;
            cbDepartementPrep.Foreground = Brushes.Transparent;
            cbMedecinNomCompletPrep.IsEnabled = false;
            cbMedecinNomCompletPrep.Foreground = Brushes.Transparent;
            cbLitsDispos.IsEnabled = false;
            cbLitsDispos.Foreground = Brushes.Transparent;
            txtSpecialitePrep.IsEnabled = false;
            txtSpecialitePrep.Foreground = Brushes.Transparent;
            nonChirurgie.IsEnabled = false;
            ouiChirurgie.IsEnabled = false;
            checkTelephone.IsEnabled = false;
            checkTelevision.IsEnabled = false;
            btnEnregistrerDossier.IsEnabled = false;
            btnViderChampsDossier.IsEnabled = false;

            if (contenu == true)
            {
                cbTypeLitPrep.Foreground = Brushes.Black;
                cbDepartementPrep.Foreground = Brushes.Black;
                cbMedecinNomCompletPrep.Foreground = Brushes.Black;
                cbLitsDispos.Foreground = Brushes.Black;
                txtSpecialitePrep.Foreground = Brushes.Black;
            }
        }

        public void reinitialiserSectionPatient()
        {

            txtNomPatientAjouter.Text = "";
            txtPrenomPatientAjouter.Text = "";
            txtCPAjouter.Text = "";
            txtNASPatient.Text = "";
            dtNaissanceAjouter.SelectedDate = null;
            txtAdressePatient.Text = "";
            txtVilleAjouter.Text = "";
            txtTelephoneAjouter.Text = "";
            txtRechercherNAS.Text = "";
            cbAssuranceNomComplet.SelectedItem = null;
            cbTypePrefere.SelectedItem = null;

            txtNomPatientAjouter.IsEnabled = true;
            txtPrenomPatientAjouter.IsEnabled = true;
            txtCPAjouter.IsEnabled = true;
            txtNASPatient.IsEnabled = true;
            cbProvinceAjouter.IsEnabled = true;
            dtNaissanceAjouter.IsEnabled = true;
            txtAdressePatient.IsEnabled = true;
            txtVilleAjouter.IsEnabled = true;
            txtTelephoneAjouter.IsEnabled = true;

            nonAssurance.IsEnabled = true;
            ouiAssurance.IsEnabled = true;
            ouiLitPref.IsEnabled = true;
            nonLitPref.IsEnabled = true;

            btnAjouterPatientPrep.IsEnabled = true;
        }


        public void reinitialiserSectionDossier()
        {
            dtAdmissionPatientPrep.SelectedDate = null;
            dtCongePatientPrep.SelectedDate = null;
            dtChirurgie.SelectedDate = null;
            cbTypeLitPrep.SelectedItem = null;
            cbDepartementPrep.SelectedItem = null;
            cbLitsDispos.SelectedItem = null;
            txtSpecialitePrep.Text = "";
            nonChirurgie.IsChecked = true;
            cbMedecinNomCompletPrep.SelectedItem = null;
            checkTelephone.IsChecked = false;
            checkTelevision.IsChecked = false;

        }

        public void activerSectionDossier()
        {
            dtAdmissionPatientPrep.SelectedDate = DateTime.Now;
            dtAdmissionPatientPrep.IsEnabled = true;

            dtChirurgie.SelectedDate = null;
            dtChirurgie.IsEnabled = false;
            cbTypeLitPrep.IsEnabled = true;
            cbTypeLitPrep.Foreground = Brushes.Black;
            cbDepartementPrep.IsEnabled = true;
            cbDepartementPrep.Foreground = Brushes.Black;
            cbMedecinNomCompletPrep.IsEnabled = true;
            cbMedecinNomCompletPrep.Foreground = Brushes.Black;
            cbLitsDispos.IsEnabled = true;
            cbLitsDispos.Foreground = Brushes.Black;
            nonChirurgie.IsChecked = true;
            nonChirurgie.IsEnabled = true;
            ouiChirurgie.IsEnabled = true;
            checkTelephone.IsEnabled = true;
            checkTelevision.IsEnabled = true;
            txtSpecialitePrep.Foreground = Brushes.Black;

            btnEnregistrerDossier.IsEnabled = true;
            btnViderChampsDossier.IsEnabled = true;

        }

        public void reinitialiserSectionParent()
        {

            txtPrenomParent.Text = "";
            txtNomParent.Text = "";
            txtAdresseParent.Text = "";
            txtVilleParent.Text = "";
            txtCPParent.Text = "";
            txtTelephoneParent.Text = "";
            txtRechercherNAS.Text = "";
            txtPrenomParent.IsEnabled = true;
            txtNomParent.IsEnabled = true;
            txtAdresseParent.IsEnabled = true;
            txtVilleParent.IsEnabled = true;
            cbProvinceParent.IsEnabled = true;
            txtCPParent.IsEnabled = true;
            txtTelephoneParent.IsEnabled = true;
        }

        public void activerSectionParent()
        {
            txtPrenomParent.IsEnabled = true;
            txtNomParent.IsEnabled = true;
            txtAdresseParent.IsEnabled = true;
            txtVilleParent.IsEnabled = true;
            cbProvinceParent.IsEnabled = true;
            txtCPParent.IsEnabled = true;
            txtTelephoneParent.IsEnabled = true;

            btnEnregistrerParent.IsEnabled = true;
            btnViderParent.IsEnabled = true;
        }










        //==============Listeners==========================

        private void btnQuitterPrep_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnViderChampsParents_Click(object sender, RoutedEventArgs e)
        {
            reinitialiserSectionParent();
        }

        private void btnViderChampsDossier_Click(object sender, RoutedEventArgs e)
        {
            reinitialiserSectionDossier();
        }

        

        private void btnRechercherPatient_Click(object sender, RoutedEventArgs e)
        {
            
            bool inclureDossier = false;
            List<Dossier_Admission> dossiersRetrouves;
            Dossier_Admission dossierSelectionne = null;
            Patient patientRetrouve = null;
            Parent parentRetrouve = null;
            int recherche;

            bool ok = Int32.TryParse(txtRechercherNAS.Text, out recherche);

            if (!ok)
            {
                MessageBox.Show("Entrez seulement des chiffres sans espaces.");
                return;
            }

            //Rechercher patient
            patientRetrouve = Singleton.Instance.bd.Patients.Where(p => p.idPatient == recherche).FirstOrDefault();

            //Si patient non-trouve, terminer.
            if (patientRetrouve == null)
            {
                txtRechercherNAS.Text = "";
                MessageBox.Show("Patient non-trouvé. Essayez à nouveau ou remplissez les champs.", "Échec", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            string txtAfficher = "Patient retrouvé.\n";

            //Verifier si ce patient a un dossier existant
            dossiersRetrouves = Singleton.Instance.bd.Dossier_Admission.Where(d => d.nasPatient == patientRetrouve.idPatient).OrderByDescending(d => d.dateAdmission).ToList();
                    
            //Si oui
            if (dossiersRetrouves.Count > 0)
            {
                //Si le dossier le plus recent est toujours actif, prendre en note ce dossier pour l'afficher.
                if (dossiersRetrouves.FirstOrDefault().dateConge == null)
                {
                    txtAfficher += "Ce patient n'a pas encore été libéré par un médecin et se trouve toujours dans l'hôpital. Voici le dossier en cours pour ce patient.";
                    MessageBox.Show(txtAfficher, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    inclureDossier = true;
                    dossierSelectionne = dossiersRetrouves.FirstOrDefault();
                            
                }
                //Sinon, demander si l'usager souhaite creer un nouveau dossier.
                else
                {
                    txtAfficher += "Ce patient compte au moins un dossier clos dans notre système. Souhaitez-vous créer un nouveau dossier d'admission pour ce patient?";
                    var ouvrirNouveau = MessageBox.Show(txtAfficher, "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    //Si non, selectionner le dossier le plus recent et l'afficher.
                    if (ouvrirNouveau == MessageBoxResult.No)
                    {
                        MessageBox.Show("Voici le dossier le plus récent pour ce patient.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        inclureDossier = true;
                        dossierSelectionne = dossiersRetrouves.FirstOrDefault();
                                
                    }
                            
                    else if (ouvrirNouveau == MessageBoxResult.Cancel)
                    {
                        btnAjouterPatientPrep.IsEnabled = true;
                        return;
                    }
                }
            }

            //Reinitialiser les deux autres sections
            reinitialiserSectionParent();
            reinitialiserSectionDossier();

            //Si le patient n'a pas de parent, activer la section parent (pour que l'on puisse ajouter un parent). Sinon, remplir la section avec les informations du parent puis désactiver la section.

            parentRetrouve = Singleton.Instance.bd.Parents.Where(p => p.refPatient == patientRetrouve.idPatient).FirstOrDefault();
           
            if (parentRetrouve == null)
            {
                activerSectionParent();
            }
            else
            {
                txtPrenomParent.Text = parentRetrouve.prenomParent;
                txtNomParent.Text = parentRetrouve.nomParent;
                txtAdresseParent.Text = parentRetrouve.adresseParent;
                txtVilleParent.Text = parentRetrouve.villeParent;
                cbProvinceParent.Text = parentRetrouve.provinceParent;
                txtCPParent.Text = parentRetrouve.cpParent;
                txtTelephoneParent.Text = parentRetrouve.noTelParent;
                desactiverSectionParent();
            }
                    
            //S'il ne reste plus de lit disponibles dans l'hopital, interdire l'ajout de dossier
            if (inclureDossier == false && rechercherLits(0, 0) == 0)
            {

                MessageBox.Show("Il n'y a malheureusement aucun lit de disponible présentement dans tout l'hôpital. Vous ne pouvez donc pas ajouter de dossier pour ce patient pour l'instant. Réessayez plus tard.", "Hôpital plein", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnAjouterPatientPrep.IsEnabled = true;
                return;
            }

            //Remplir les informations du patient dans l'interface.
            txtNomPatientAjouter.Text = patientRetrouve.nomPatient;
            txtPrenomPatientAjouter.Text = patientRetrouve.prenomPatient;
            txtCPAjouter.Text = patientRetrouve.cpPatient;
            txtNASPatient.Text = patientRetrouve.idPatient.ToString();
            cbProvinceAjouter.Text = patientRetrouve.provincePatient;
            dtNaissanceAjouter.SelectedDate = patientRetrouve.dateNaissancePatient;
            txtAdressePatient.Text = patientRetrouve.adressePatient;
            txtVilleAjouter.Text = patientRetrouve.villePatient;
            txtTelephoneAjouter.Text = patientRetrouve.noTelPatient;

            if (patientRetrouve.typeLitPrefere == null)
            {
                nonLitPref.IsChecked = true;
                cbTypePrefere.SelectedItem = null;
            }
            else
            {
                ouiLitPref.IsChecked = true;
                cbTypePrefere.SelectedIndex = (int)patientRetrouve.typeLitPrefere - 1; ;
                cbTypePrefere.IsEnabled = false;

            }

            if (patientRetrouve.idCompagnie == null)
            {
                nonAssurance.IsChecked = true;
                cbAssuranceNomComplet.IsEnabled = false;
            }
            else
            {
                ouiAssurance.IsChecked = true;
                cbAssuranceNomComplet.SelectedIndex = (int)patientRetrouve.idCompagnie - 1;
                cbAssuranceNomComplet.IsEnabled = false;
            }
                    
            //Preselectionner des lits pour des nouveaux dossiers
            if (inclureDossier == false)
            {
                calculerAge(patientRetrouve.dateNaissancePatient);

                if (patientRetrouve.typeLitPrefere == null)
                {
                    preSelectionnerLitsDispos(0);

                }
                else
                {
                    preSelectionnerLitsDispos((int)patientRetrouve.typeLitPrefere);
                }

                activerSectionDossier();

            }
            //Pour les cas où l'on souhaite afficher un dossier existant, afficher le dossier
            else
            {
                dtAdmissionPatientPrep.SelectedDate = dossierSelectionne.dateAdmission;
                dtCongePatientPrep.SelectedDate = dossierSelectionne.dateConge;
                dtChirurgie.SelectedDate = dossierSelectionne.dateChirurgie;

                if (dossierSelectionne.chirurgieProgrammee == false)
                {
                    nonChirurgie.IsChecked = true;
                }
                else
                {
                    ouiChirurgie.IsChecked = true;
                }

                List<Lit> litsDisponibles = Singleton.Instance.bd.Lits.Where(l => l.noLit == dossierSelectionne.noLit).ToList();
                cbLitsDispos.ItemsSource = litsDisponibles;
                cbTypeLitPrep.Text = dossierSelectionne.Lit.Type_Lit.descTypeLit;
                cbDepartementPrep.Text = dossierSelectionne.Lit.Departement.nomDepartement;
                if (dossierSelectionne.idMedecin != null)
                {
                    cbMedecinNomCompletPrep.Text = dossierSelectionne.Medecin.prenomMedecin + " " + dossierSelectionne.Medecin.nomMedecin;
                    txtSpecialitePrep.Text = dossierSelectionne.Medecin.specialiteMedecin;
                }
                

                if (dossierSelectionne.telephone == true)
                {
                    checkTelephone.IsChecked = true;
                }
                else
                {
                    checkTelephone.IsChecked = false;
                }

                if (dossierSelectionne.television == true)
                {
                    checkTelevision.IsChecked = true;
                }
                else
                {
                    checkTelevision.IsChecked = false;
                }

                desactiverSectionDossier(true);
     
            }

            desactiverSectionPatient();
        }

        private void btnQuitterPrep_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btnAjouterPatientPrep_Click_(object sender, RoutedEventArgs e)
        {

            Patient nouvPatient = new Patient();
            int number;
            bool success = Int32.TryParse(txtNASPatient.Text, out number);

            //Si format NAS invalide
            if (!success || txtNASPatient.Text.Length != 9)
            {
                MessageBox.Show("Vous devez fournir un NAS composé de 9 chiffres uniquement, sans espace.", "Échec", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnAjouterPatientPrep.IsEnabled = true;
                btnEnregistrerDossier.IsEnabled = false;
                return;

            }
            
            Patient patientExistant = Singleton.Instance.bd.Patients.Where(p => p.idPatient == number).FirstOrDefault();
                
            //Si un patient existe deja avec ce NAS, refuser l'ajout
            if (patientExistant != null)
            {
                MessageBox.Show("Un patient avec ce NAS existe déjà.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Si aucun nom ou prenom entré
            if (txtPrenomPatientAjouter.Text == "" || txtNomPatientAjouter.Text == "")
            {
                MessageBox.Show("Vous devez minimalement fournir un NAS, nom complet et date de naissance.", "Échec", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }
            //Essayer de créer l'objet Patient
            nouvPatient.adressePatient = txtAdressePatient.Text;
            nouvPatient.cpPatient = txtCPAjouter.Text;
            nouvPatient.nomPatient = txtNomPatientAjouter.Text;
            nouvPatient.prenomPatient = txtPrenomPatientAjouter.Text;
            nouvPatient.noTelPatient = txtTelephoneAjouter.Text;
            nouvPatient.villePatient = txtVilleAjouter.Text;
            nouvPatient.idPatient = number;
            nouvPatient.provincePatient = cbProvinceAjouter.Text;
            nouvPatient.dateNaissancePatient = (DateTime)dtNaissanceAjouter.SelectedDate;

            //Ajouter compagnie si selectionnée
            if (ouiAssurance.IsChecked == true)
            {
                nouvPatient.idCompagnie = (cbAssuranceNomComplet.SelectedItem as Compagnie_Assurance).idCompagnie;
            }

            //Ajouter lit préféré si selectionnee
            if (ouiLitPref.IsChecked == true)
            {
                nouvPatient.typeLitPrefere = (cbTypePrefere.SelectedItem as Type_Lit).idTypeLit;
            }

            Singleton.Instance.bd.Patients.Add(nouvPatient);

            try
            {
                Singleton.Instance.bd.SaveChanges();
                MessageBox.Show("Patient ajouté avec succès!", "Ajout", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //S'il ne reste plus de lit disponibles dans l'hopital, interdire l'ajout de dossier
                if (rechercherLits(0, 0) == 0)
                {
                    MessageBox.Show("Il n'y a malheureusement aucun lit de disponible présentement dans tout l'hôpital. Vous ne pouvez donc pas ajouter de dossier pour ce patient pour l'instant. Réessayez plus tard.", "Hôpital plein", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnAjouterPatientPrep.IsEnabled = true;
                    btnEnregistrerDossier.IsEnabled = false;
                }
                else
                {
                    calculerAge(nouvPatient.dateNaissancePatient);
                    preSelectionnerLitsDispos((int)nouvPatient.typeLitPrefere);
                    desactiverSectionPatient();
                    activerSectionParent();
                    activerSectionDossier();
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
        private void cbMedecinNomCompletPrep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medecin med = cbMedecinNomCompletPrep.SelectedItem as Medecin;
            if (med != null)
            {
                txtSpecialitePrep.Text = med.specialiteMedecin;

            }

        }

        private void ouiAssurance_Checked(object sender, RoutedEventArgs e)
        {
            cbAssuranceNomComplet.IsEnabled = true;

            var nomAssurance =

                        (from c in Singleton.Instance.bd.Compagnie_Assurance
                         orderby c.idCompagnie
                         select new { c.nomCompagnie }).First();

            cbAssuranceNomComplet.Text = nomAssurance.nomCompagnie;
            cbAssuranceNomComplet.Foreground = Brushes.Black;

        }

        private void nonAssurance_Checked(object sender, RoutedEventArgs e)
        {
            cbAssuranceNomComplet.IsEnabled = false;
            cbAssuranceNomComplet.Foreground = Brushes.Transparent;
        }

        private void nonPrefere_Checked(object sender, RoutedEventArgs e)
        {
            cbTypePrefere.IsEnabled = false;
            cbTypePrefere.Foreground = Brushes.Transparent;
        }

        private void ouiPrefere_Checked(object sender, RoutedEventArgs e)
        {
            cbTypePrefere.IsEnabled = true;
            cbTypePrefere.Foreground = Brushes.Black;

            var nomType =

                        (from t in Singleton.Instance.bd.Type_Lit
                         orderby t.idTypeLit
                         select new { t.descTypeLit }).First();

            cbTypePrefere.Text = nomType.descTypeLit;
        }

       


        private void btnViderParent_Click(object sender, RoutedEventArgs e)
        {
            reinitialiserSectionParent();
        }

        private void btnEnregistrerParent_Click(object sender, RoutedEventArgs e)
        {
            //Si un patient n'est pas enregistré avec ce parent, avertir l'usager et ne pas autoriser l'ajout de parent. 
            //Par contre, garder les informations entrées.

            if (txtNASPatient.IsEnabled == true)
            {
                MessageBox.Show("Commencez par enregistrer un patient avant d'enregistrer son parent.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            //Si aucun nom ou prénom entrés
            if (txtPrenomParent.Text=="" || txtNomParent.Text == "")
            {
                MessageBox.Show("Un parent doit minimalement avoir un nom et un prénom.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Créer le parent
            Parent nouvParent = new Parent();

            nouvParent.nomParent = txtNomParent.Text;
            nouvParent.prenomParent = txtPrenomParent.Text;
            nouvParent.adresseParent = txtAdresseParent.Text;
            nouvParent.villeParent = txtVilleParent.Text;
            nouvParent.cpParent = txtCPParent.Text;
            nouvParent.noTelParent = txtTelephoneParent.Text;
            nouvParent.provinceParent = cbProvinceParent.Text;

            //Aller chercher le patient pour l'associer à ce parent
            int idPatientInterface = Int32.Parse(txtNASPatient.Text);
            nouvParent.refPatient = idPatientInterface;
            string prenomPatient = txtPrenomPatientAjouter.Text;
            string nomPatient = txtNomPatientAjouter.Text;

            Singleton.Instance.bd.Parents.Add(nouvParent);

            try
            {
                Singleton.Instance.bd.SaveChanges();
                MessageBox.Show("Parent ajouté avec succès à " + prenomPatient + " " + nomPatient + "!", "Ajout", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            try
            {
                Singleton.Instance.bd.SaveChanges();
                desactiverSectionParent();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnViderChampsPatient_Click(object sender, RoutedEventArgs e)
        {
            reinitialiserSectionPatient();
            reinitialiserSectionParent();
            reinitialiserSectionDossier();
            desactiverSectionParent();
            desactiverSectionDossier();
        }

        private void btnViderChampsDossier_Click_1(object sender, RoutedEventArgs e)
        {
            reinitialiserSectionDossier();
        }

        private void nonChirurgie_Checked_1(object sender, RoutedEventArgs e)
        {
            dtChirurgie.IsEnabled = false;
            dtChirurgie.SelectedDate = null;
        }

        private void ouiChirurgie_Checked_1(object sender, RoutedEventArgs e)
        {
            int idDepartementChirurgie = 1;

            //Activer le selecteur de date pour chirurgie et selectionner la date d'aujour'hui par défaut
            dtChirurgie.IsEnabled = true;
            dtChirurgie.SelectedDate = DateTime.Now;

            //N'exécuter ce qui suit que si le bouton Ajouter de la section du haut est désactivé - signifiant qu'un patient a bien été sélectionné
            if (btnAjouterPatientPrep.IsEnabled == false)
            {

                //Variable contenant l'id du lit présentement sélectionné dans le combobox
                int idTypeSelectionne = 0;
               
                if (cbTypeLitPrep.SelectedItem != null)
                {
                    idTypeSelectionne = cbTypeLitPrep.SelectedIndex + 1;

                }

                //Selectionner le departement de chirurgie dans le ComboBox
                cbDepartementPrep.SelectedIndex = 0;

                //S'il ne reste plus aucun lit de chirurgie de disponible
                if (rechercherLits(idDepartementChirurgie, 0) == 0)
                {
                    MessageBox.Show("Aucun lit de chirurgie n'est disponible pour l'instant. Selectionez un lit d'un autre departement.");

                }

                afficherLits(idDepartementChirurgie, idTypeSelectionne);

            }
        }

        private void btnEnregistrerDossier_Click_1(object sender, RoutedEventArgs e)
        {
            Dossier_Admission dossier = new Dossier_Admission();

            if (cbLitsDispos.SelectedItem == null)
            {
                MessageBox.Show("Nous ne pouvons admettre de patients qui n'ont pas de lits.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (dtAdmissionPatientPrep.SelectedDate == null)
            {
                MessageBox.Show("Veuillez saisir une date d'admission", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int numLit = Int32.Parse(cbLitsDispos.Text);

            dossier.noLit = numLit;
            dossier.nasPatient = Int32.Parse(txtNASPatient.Text);
            dossier.dateAdmission = (DateTime)dtAdmissionPatientPrep.SelectedDate;
            dossier.dateConge = null;
            if (cbMedecinNomCompletPrep.SelectedItem != null)
            {
                dossier.idMedecin = (cbMedecinNomCompletPrep.SelectedItem as Medecin).idMedecin;
            }

            //Si chirurgie prevue mais chambre de chirurgie non-selectionnee, demander confirmation
            if (ouiChirurgie.IsChecked == true && cbDepartementPrep.SelectedIndex != 0)
            {
                var resultatLitAutre = MessageBox.Show("Une chirurgie est prévue pour ce patient, mais le lit sélectionné n'est pas dans le département de chirurgie. Est-ce exact?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultatLitAutre == MessageBoxResult.No)
                {
                    MessageBox.Show("Enregistrement annulé", "Annulation", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                
                {
                    dossier.noLit = null;
                }
            }
            if (ouiChirurgie.IsChecked == true)
            {
                dossier.chirurgieProgrammee = true;
                dossier.dateChirurgie = (DateTime)dtChirurgie.SelectedDate;

            }
            else
            {
                dossier.chirurgieProgrammee = false;

            }

            if (checkTelephone.IsChecked == true)
            {
                dossier.telephone = true;
            }

            if (checkTelevision.IsChecked == true)
            {
                dossier.television = true;
            }

            //Si patient a 16 ans ou moins mais le lit n'est pas en pediatrie, demander confirmation
            if (agePatient >= 0 && agePatient <= 16 && cbDepartementPrep.SelectedIndex != 1)
            {

                var resultatNonPediatrie = MessageBox.Show("Ce patient a 16 ans ou moins, mais le lit sélectionné n'est pas dans le département de pediatrie. Est-ce exact?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultatNonPediatrie == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    dossier.noLit = null;
                }


            }

            //determiner si le patient devra payer pour le lit et demander une confirmation si c'est le cas
            dossier.litPayant = determinerSiPayant(numLit);
            
            if (determinerSiPayant(numLit) == true)
            {
                var reponse = MessageBox.Show("Ce patient devra prendre en charge ses frais de lit. Confirmez-vous l'assignation de ce lit?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (reponse == MessageBoxResult.No)
                {
                    return;
                }
            }

            //Ajouter le dossier (mais pas encore ajout dans la BD)
            Singleton.Instance.bd.Dossier_Admission.Add(dossier);
            
            //Marquer ce lit comme étant "occupé"
            int numLitAssigne = Int32.Parse(cbLitsDispos.Text);
            Lit litAssigne = Singleton.Instance.bd.Lits.Where(l => l.noLit == numLitAssigne).FirstOrDefault();
            litAssigne.occupe = true;

            try
            {
                Singleton.Instance.bd.SaveChanges();
                MessageBox.Show("Dossier ajouté avec succes!", "Ajout", MessageBoxButton.OK, MessageBoxImage.Information);

                desactiverSectionDossier(true);

               
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void cbTypeLitPrep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Mettre a jour les lits disponibles

            int numTypeLit = cbTypeLitPrep.SelectedIndex + 1;
            int numDpt = 0;

            if (cbDepartementPrep.SelectedItem != null)
            {
                numDpt = cbDepartementPrep.SelectedIndex + 1;
            }

            afficherLits(numDpt, numTypeLit);

        }

        private void cbDepartementPrep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Mettre a jour les departements

            int numTypeLit = 0;
            int numDpt = cbDepartementPrep.SelectedIndex + 1;

            if (cbTypeLitPrep.SelectedItem != null)
            {
                numTypeLit = cbTypeLitPrep.SelectedIndex + 1;
            }

            afficherLits(numDpt, numTypeLit);

        }

    }
}
