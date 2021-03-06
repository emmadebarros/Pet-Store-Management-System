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
    
    public partial class Animal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Animal()
        {
            this.Animal_Services = new HashSet<Animal_Service>();
        }
    
        public int AnimalID { get; set; }
        public SpeciesType Species { get; set; }
        public GenderType Gender { get; set; }
        public int BreedID { get; set; }
        public decimal Weight { get; set; }
        public byte[] Photo { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Name { get; set; }
        public System.DateTime DateArrived { get; set; }
        public Nullable<System.DateTime> DateAdopted { get; set; }
        public bool Microchipped { get; set; }
        public bool Wormed { get; set; }
        public bool Neutured { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public Nullable<int> CrateID { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Animal_Service> Animal_Services { get; set; }
        public virtual Breed Breed { get; set; }
        public virtual Crate Crate { get; set; }
        public virtual Owner Owner { get; set; }
    }
}
