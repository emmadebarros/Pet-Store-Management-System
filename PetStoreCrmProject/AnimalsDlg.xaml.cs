using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using PetStoreCrmProject.DataLayer;

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for AnimalsDlg.xaml
    /// </summary>
    public partial class AnimalsDlg : Window
    {
        public AnimalsDlg()
        {
            //var animals = Globals.ctx.Animals;
            try
            {
                InitializeComponent();
                /* Grid All */
                loadDataAll();
                /* Grid Adopted */
                loadDataAdopted();
                /* Grid Available */
                loadDataAvailable();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void loadDataAll()
        {
            var animals = from a in Globals.ctx.Animals
                          join b in Globals.ctx.Breeds on a.BreedID equals b.BreedID
                          join c in Globals.ctx.Owners on a.OwnerID equals c.OwnerID into o
                          from c in o.DefaultIfEmpty() //left-outer join
                                                       //join d in Globals.ctx.Crates on a.CrateID equals d.CrateID into s from d in s.DefaultIfEmpty()  
                          select new //AnimalForDisplay()
                          {
                              ID = a.AnimalID,
                              Name = a.Name,
                              Species = a.Species,
                              Breed = b.BreedName,
                              Gender = a.Gender,
                              Weight = a.Weight + " lbs",
                              Photo = a.Photo != null ? "Yes" : "No",
                              DOB = a.DOB, //Fix System.DateTime?
                              Arrived = a.DateArrived, //Fix System.DateTime?
                              Adopted = a.DateAdopted, //Fix System.DateTime?
                              Mirochipped = a.Microchipped ? "Yes" : "No",
                              Wormed = a.Wormed ? "Yes" : "No",
                              Sterilized = a.Neutured ? "Yes" : "No",
                              OwnerID = a.OwnerID,
                              OwnerName = c.Name + " " + c.LastName,
                              Crate = a.Crate != null ? "Yes" : "No",
                              Description = a.Description
                          }
            ;
            /* Display */
            gridAll.ItemsSource = animals.ToList();
        }

        public void loadDataAdopted()
        {
            var animalsAdopted = from a in Globals.ctx.Animals
                                 join b in Globals.ctx.Breeds on a.BreedID equals b.BreedID
                                 join c in Globals.ctx.Owners on a.OwnerID equals c.OwnerID
                                 select new
                                 {
                                     ID = a.AnimalID,
                                     Name = a.Name,
                                     Species = a.Species,
                                     Breed = b.BreedName,
                                     Gender = a.Gender,
                                     Weight = a.Weight + " lbs",
                                     Photo = a.Photo != null ? "Yes" : "No",
                                     DOB = a.DOB, //Fix System.DateTime?
                                     Arrived = a.DateArrived, //Fix System.DateTime?
                                     Adopted = a.DateAdopted, //Fix System.DateTime?
                                     Mirochipped = a.Microchipped ? "Yes" : "No",
                                     Wormed = a.Wormed ? "Yes" : "No",
                                     Sterilized = a.Neutured ? "Yes" : "No",
                                     OwnerID = a.OwnerID,
                                     OwnerName = c.Name + " " + c.LastName,
                                     Description = a.Description
                                 }
            ;
            /* Display */
            gridAdopted.ItemsSource = animalsAdopted.ToList();
        }

        public void loadDataAvailable()
        {
            var animalsAvailable = from a in Globals.ctx.Animals
                                   join b in Globals.ctx.Breeds on a.BreedID equals b.BreedID
                                   join c in Globals.ctx.Crates on a.CrateID equals c.CrateID
                                   select new
                                   {
                                       ID = a.AnimalID,
                                       Name = a.Name,
                                       Species = a.Species,
                                       Breed = b.BreedName,
                                       Gender = a.Gender,
                                       Weight = a.Weight + " lbs",
                                       Photo = a.Photo != null ? "Yes" : "No",
                                       DOB = a.DOB, //Fix System.DateTime?
                                       Arrived = a.DateArrived, //Fix System.DateTime?
                                       Mirochipped = a.Microchipped ? "Yes" : "No",
                                       Wormed = a.Wormed ? "Yes" : "No",
                                       Sterilized = a.Neutured ? "Yes" : "No",
                                       CrateID = a.CrateID,
                                       CrateSize = c.Size,
                                       Description = a.Description
                                   }
            ;
            /* Display */
            gridAvailable.ItemsSource = animalsAvailable.ToList();
        }

        /* *** DELETE *** */
        /* FIXME: Get rid of code duplication */

        private void miAllDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridAll.SelectedItems.Count == 0 || gridAll.SelectedItems.Count > 1) return;

            //DataLayer.Animal a = (DataLayer.Animal)gridAll.SelectedItem;

            try
            {
                int id = (int)gridAll.SelectedItem.GetType().GetProperty("ID").GetValue(gridAll.SelectedItem, null);

                var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toDelete != null)
                {
                    //double check
                    MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete this animal?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        if (toDelete.CrateID != null)
                        {
                            var crate = Globals.ctx.Crates.Where(c => c.CrateID == toDelete.CrateID).SingleOrDefault();
                            if(crate != null)
                            {
                                crate.Status = false; // false = available
                            }
                        }
                        //do we delete owner if owner has no other animal....?
                        Globals.ctx.Animals.Remove(toDelete);
                        Globals.ctx.SaveChanges();
                        //gridAll.ItemsSource = Globals.ctx.Animals.ToList();
                        loadDataAll();
                        loadDataAdopted();
                        loadDataAvailable();
                    }
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void miAdoptedDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridAdopted.SelectedItems.Count == 0 || gridAdopted.SelectedItems.Count > 1) return;

            //DataLayer.Animal a = (DataLayer.Animal)gridAll.SelectedItem;

            try
            {
                int id = (int)gridAdopted.SelectedItem.GetType().GetProperty("ID").GetValue(gridAdopted.SelectedItem, null);

                var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toDelete != null)
                {
                    //double check
                    MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete this animal?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        //no crate because adopted
                        //do we delete owner if owner has no other animal....?
                        Globals.ctx.Animals.Remove(toDelete);
                        Globals.ctx.SaveChanges();
                        loadDataAdopted();
                        loadDataAll();
                    }
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void miAvailableDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridAvailable.SelectedItems.Count == 0 || gridAvailable.SelectedItems.Count > 1) return;

            //DataLayer.Animal a = (DataLayer.Animal)gridAll.SelectedItem;

            try
            {
                int id = (int)gridAvailable.SelectedItem.GetType().GetProperty("ID").GetValue(gridAvailable.SelectedItem, null);

                var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toDelete != null)
                {
                    //double check
                    MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete this animal?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        if (toDelete.CrateID != null)
                        {
                            var crate = Globals.ctx.Crates.Where(c => c.CrateID == toDelete.CrateID).SingleOrDefault();
                            if (crate != null)
                            {
                                crate.Status = false; // false = available
                            }
                        }
                        //do we delete owner if owner has no other animal....?
                        Globals.ctx.Animals.Remove(toDelete);
                        Globals.ctx.SaveChanges();
                        loadDataAvailable();
                        loadDataAll();
                    }
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        /* *** UPDATE *** */

        private void gridAllUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridAll.SelectedItems.Count == 0 || gridAll.SelectedItems.Count > 1) return;

            try
            {
                int id = (int)gridAll.SelectedItem.GetType().GetProperty("ID").GetValue(gridAll.SelectedItem, null);

                var toUpdate = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toUpdate != null)
                {
                    AddAnimalDlg dlg = new AddAnimalDlg(toUpdate);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        try
                        {                          
                            loadDataAll();
                        }
                        catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
                        {
                            Console.WriteLine(ex.StackTrace);
                            MessageBox.Show("Database operation failed:\n" + ex.Message);
                        }
                    }                       
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void gridAdoptedUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridAdopted.SelectedItems.Count == 0 || gridAdopted.SelectedItems.Count > 1) return;

            try
            {
                int id = (int)gridAdopted.SelectedItem.GetType().GetProperty("ID").GetValue(gridAdopted.SelectedItem, null);

                var toUpdate = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toUpdate != null)
                {
                    AddAnimalDlg dlg = new AddAnimalDlg(toUpdate);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        try
                        {
                            loadDataAdopted();
                        }
                        catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
                        {
                            Console.WriteLine(ex.StackTrace);
                            MessageBox.Show("Database operation failed:\n" + ex.Message);
                        }
                    }                   
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void gridAvailableUpdate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridAvailable.SelectedItems.Count == 0 || gridAvailable.SelectedItems.Count > 1) return;

            try
            {
                int id = (int)gridAvailable.SelectedItem.GetType().GetProperty("ID").GetValue(gridAvailable.SelectedItem, null);

                var toUpdate = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();

                if (toUpdate != null)
                {
                    AddAnimalDlg dlg = new AddAnimalDlg(toUpdate);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        try
                        {
                            loadDataAvailable();
                        }
                        catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
                        {
                            Console.WriteLine(ex.StackTrace);
                            MessageBox.Show("Database operation failed:\n" + ex.Message);
                        }
                    }                   
                }
                else
                {
                    //should never happen but in case...
                    MessageBox.Show($"No animal with ID {id} found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //var toDelete = Globals.ctx.Animals.Where(a => a.AnimalID == gridAll.SelectedItem())
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }      
    }
}
