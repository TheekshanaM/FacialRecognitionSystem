//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FacialRecognitionSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Celebrity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Celebrity()
        {
            this.CelebrityPhotoes = new HashSet<CelebrityPhoto>();
            this.CelebritySuggestions = new HashSet<CelebritySuggestion>();
            this.CelebritySuggestions1 = new HashSet<CelebritySuggestion>();
        }
    
        public int CelebrityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Feild { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
        public int Rating { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CelebrityPhoto> CelebrityPhotoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CelebritySuggestion> CelebritySuggestions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CelebritySuggestion> CelebritySuggestions1 { get; set; }
    }
}
