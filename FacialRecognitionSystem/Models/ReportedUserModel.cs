using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacialRecognitionSystem.Models;


namespace FacialRecognitionSystem.Models
{
    public class ReportedUserModel
    {
        public int ReportedUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<String> Comments { get; set; }
        public int TotalReports { get; set; }

     


    }
}
