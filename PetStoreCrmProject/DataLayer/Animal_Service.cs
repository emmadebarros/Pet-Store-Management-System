//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PetStoreCrmProject.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Animal_Service
    {
        public int AnimalID { get; set; }
        public int EmployeeID { get; set; }
        public ServiceType ServiceType { get; set; }
        public System.DateTime ServiceDate { get; set; }
        public string Description { get; set; }
    
        public virtual Animal Animal { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
