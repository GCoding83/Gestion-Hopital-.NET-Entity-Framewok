//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NLH
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dossier_Admission
    {
        public int idDossier { get; set; }
        public Nullable<bool> chirurgieProgrammee { get; set; }
        public Nullable<System.DateTime> dateChirurgie { get; set; }
        public Nullable<System.DateTime> dateConge { get; set; }
        public int nasPatient { get; set; }
        public Nullable<int> noLit { get; set; }
        public Nullable<int> idMedecin { get; set; }
        public Nullable<System.DateTime> dateAdmission { get; set; }
        public Nullable<bool> television { get; set; }
        public Nullable<bool> telephone { get; set; }
        public Nullable<bool> litPayant { get; set; }
    
        public virtual Lit Lit { get; set; }
        public virtual Medecin Medecin { get; set; }
        public virtual Patient Patient { get; set; }
    }
}