using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using PetStoreCrmProject.DataLayer;

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for ManageOwnersDlg.xaml
    /// </summary>
    public partial class ManageOwnersDlg : Window
    {
        Owner currSelOwner;
        public ManageOwnersDlg(Owner owner = null)
        {
            try
            {
                InitializeComponent();
                lvOwnerList.ItemsSource = (from o in Globals.ctx.Owners.ToList() select o).ToList();
                comboOwnerProvince.ItemsSource = Enum.GetValues(typeof(ProvinceType)).Cast<ProvinceType>();
                comboOwnerProvince.SelectedIndex = 0;
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
            
        }

        private void lvOwnerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currSelOwner = lvOwnerList.SelectedItem as Owner; //or (Owner)lvOwnerList.SelectedItem
            if (currSelOwner == null)
            {
                ClearInputs();
            }
            else
            {
                tbOwnerFirstName.Text = currSelOwner.Name;
                tbOwnerMiddleName.Text = currSelOwner.MiddleName;
                tbOwnerLastName.Text = currSelOwner.LastName;
                tbOwnerAddress.Text = currSelOwner.Address;
                tbOwnerCity.Text = currSelOwner.City;
                comboOwnerProvince.SelectedIndex = (int)currSelOwner.Province;
                tbOwnerPostalCode.Text = currSelOwner.PostalCode;
                tbOwnerPrimaryPhone.Text = currSelOwner.PrimaryPhone;
                tbOwnerSecondaryPhone.Text = currSelOwner.SecondaryPhone;
                tbOwnerEmail.Text = currSelOwner.Email;
                cbFostering.IsChecked = currSelOwner.Fostering;
                tbNote.Text = currSelOwner.Note;
                btUpdate.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
        }

        private void ClearInputs()
        {
            tbOwnerFirstName.Text = "First name";
            tbOwnerMiddleName.Text = "Middle name";
            tbOwnerLastName.Text = "Last name";
            tbOwnerAddress.Text = "";
            tbOwnerCity.Text = "";
            comboOwnerProvince.SelectedIndex = 0;
            tbOwnerPostalCode.Text = "";
            tbOwnerPrimaryPhone.Text = "Primary";
            tbOwnerSecondaryPhone.Text = "Secondary";
            tbOwnerEmail.Text = "";
            cbFostering.IsChecked = false;
            tbNote.Text = "Note:";
            lvOwnerList.SelectedItem = null;
            btUpdate.IsEnabled = false;
            btDelete.IsEnabled = false;
        }
        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            currSelOwner = lvOwnerList.SelectedItem as Owner; //or (Owner)lvOwnerList.SelectedItem
            if (currSelOwner == null)// never going to happen, because it is disabled if null
            {
                MessageBox.Show(this, "Please select an item to update", "Selection error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                currSelOwner.Name = tbOwnerFirstName.Text;
                currSelOwner.MiddleName = tbOwnerMiddleName.Text;
                currSelOwner.LastName = tbOwnerLastName.Text;
                currSelOwner.Address = tbOwnerAddress.Text;
                currSelOwner.City = tbOwnerCity.Text;
                currSelOwner.Province = (ProvinceType)(int)comboOwnerProvince.SelectedValue;
                currSelOwner.Country = tbOwnerCountry.Text;
                currSelOwner.PrimaryPhone = tbOwnerPrimaryPhone.Text;
                currSelOwner.SecondaryPhone = tbOwnerSecondaryPhone.Text;
                currSelOwner.Email = tbOwnerEmail.Text;
                currSelOwner.Fostering = cbFostering.IsChecked.GetValueOrDefault();
                currSelOwner.Note = tbNote.Text;

                Globals.ctx.SaveChanges();
                ClearInputs();
                lvOwnerList.ItemsSource = (from o in Globals.ctx.Owners.ToList() select o).ToList();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Database operation failed:\n" + ex.Message);
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            currSelOwner = lvOwnerList.SelectedItem as Owner; //or (Owner)lvOwnerList.SelectedItem
            if (currSelOwner == null)// never going to happen, because it is disabled if null
            {
                MessageBox.Show(this, "Please select an item to delete", "Selection error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Do you want to delete: " + currSelOwner.Name+ " "+ currSelOwner.LastName + " ?", "Data save", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // delete owner id and adoption date from animal
                var animal = Globals.ctx.Animals.FirstOrDefault(a => a.OwnerID == currSelOwner.OwnerID);
                animal.OwnerID = null;
                animal.DateAdopted = null;
                Globals.ctx.SaveChanges();

                // assign a crate for the animal with no owner now
                var crate = Globals.ctx.Crates.Where(c => !c.Status).ToList();
                animal.CrateID = crate[0].CrateID;
                crate[0].Status = true;
                Globals.ctx.SaveChanges();

                Globals.ctx.Owners.Remove(currSelOwner);
                Globals.ctx.SaveChanges();
                ClearInputs();
                lvOwnerList.ItemsSource = (from o in Globals.ctx.Owners.ToList() select o).ToList();
            }
        }

        //* ******** Placeholder text ******** *//
        private void tbOwnerFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbOwnerFirstName.Text == "First name")
            {
                tbOwnerFirstName.Text = "";
            }
        }

        private void tbOwnerFirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOwnerFirstName.Text))
            {
                tbOwnerFirstName.Text = "First name";
            }
        }

        private void tbOwnerMiddleName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbOwnerMiddleName.Text == "Middle name")
            {
                tbOwnerMiddleName.Text = "";
            }
        }

        private void tbOwnerMiddleName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOwnerMiddleName.Text))
            {
                tbOwnerMiddleName.Text = "Middle name";
            }
        }

        private void tbOwnerLastName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbOwnerLastName.Text == "Last name")
            {
                tbOwnerLastName.Text = "";
            }
        }

        private void tbOwnerLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOwnerLastName.Text))
            {
                tbOwnerLastName.Text = "Last name";
            }
        }

        private void tbOwnerPrimaryPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbOwnerPrimaryPhone.Text == "Primary")
            {
                tbOwnerPrimaryPhone.Text = "";
            }
        }

        private void tbOwnerPrimaryPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOwnerPrimaryPhone.Text))
            {
                tbOwnerPrimaryPhone.Text = "Primary";
            }
        }

        private void tbOwnerSecondaryPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbOwnerSecondaryPhone.Text == "Secondary")
            {
                tbOwnerSecondaryPhone.Text = "";
            }
        }

        private void tbOwnerSecondaryPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOwnerSecondaryPhone.Text))
            {
                tbOwnerSecondaryPhone.Text = "Secondary";
            }
        }

        private void tbNote_GotFocus(object sender, RoutedEventArgs e) // Note:
        {
            if (tbNote.Text == "Note:")
            {
                tbNote.Text = "";
            }
        }

        private void tbNote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNote.Text))
            {
                tbNote.Text = "Note:";
            }
        }

        // export list of owners in excel
        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            var selItems = lvOwnerList.SelectedItems;
            if (selItems.Count == 0)
            {
                MessageBox.Show(this, "Select some items first", "Input error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SaveFileDialog exportDialog = new SaveFileDialog();
            exportDialog.Filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";
            exportDialog.Title = "Export to file";
          //  exportDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (exportDialog.ShowDialog() == true)
            {
                try
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", Encoding = Encoding.UTF8, HasHeaderRecord = true };
                    using (StreamWriter writer = new StreamWriter(exportDialog.FileName))
                    using (CsvWriter csv = new CsvWriter(writer, config))
                    {
                        csv.Context.RegisterClassMap<OwnerExportMap>();
                        csv.WriteRecords(selItems);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Error exporting to csv: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


    }// end partial class ManageOwnersDlg
    public sealed class OwnerExportMap : ClassMap<Owner>
    {
        public OwnerExportMap()
        {
            Map(m => m.OwnerID);
            Map(m => m.Name);
            Map(m => m.MiddleName);
            Map(m => m.LastName);
            Map(m => m.PrimaryPhone);
            Map(m => m.SecondaryPhone);
            Map(m => m.Address);
            Map(m => m.City);
            Map(m => m.Province);
            Map(m => m.Country);
            Map(m => m.PostalCode);
            Map(m => m.Email);
            Map(m => m.Note);
        }
    }
}
