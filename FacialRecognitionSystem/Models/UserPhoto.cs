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
    
    public partial class UserPhoto
    {
        public int PhotoID { get; set; }
        public string Link { get; set; }
        public bool DeleteStatus { get; set; }
        public int UploaderID { get; set; }
    
        public virtual User User { get; set; }
    }
}
