using System;
using System.Collections.Generic;
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
using PetStoreCrmProject.DataLayer;


namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for AdoptionFormDlg.xaml
    /// </summary>
    public partial class AdoptionFormDlg : Window
    {
        public AdoptionFormDlg()
        {
            try
            {
                InitializeComponent();
                comboOwnerProvince.ItemsSource = Enum.GetValues(typeof(ProvinceType)).Cast<ProvinceType>();
                comboOwnerProvince.SelectedIndex = 0;
                dpAdoptionDate.SelectedDate = DateTime.Today;
                //var animal = Globals.ctx.Animals.Where(a => a.DateAdopted == null).ToList(); // or a.Owner == null or a.crateID != null
                var animal = Globals.ctx.Animals.Where(a => a.CrateID != null).ToList(); // or a.Owner == null or a.crateID != null

                comboPetId.ItemsSource = animal;
                comboPetId.DisplayMemberPath = nameof(Animal.AnimalID);
                comboPetId.SelectedValuePath = nameof(Animal.AnimalID);
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }        
        }

        private void btSaveExit_Click(object sender, RoutedEventArgs e)
        {
            /* ******** Save ******** */
            try
            {
                // validations
                string firstName = tbOwnerFirstName.Text;
                if (!Regex.IsMatch(firstName, Globals.nameRegEx) || firstName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "First name characters must be between 1 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (firstName.Length == 0 || firstName == "First name")
                {
                    MessageBox.Show(this, "Please, input first name.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string middleName = tbOwnerMiddleName.Text;
                if (!Regex.IsMatch(middleName, Globals.nameRegEx) || middleName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "Middle name characters must be between 0 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (middleName == "Middle name") // can be null
                {
                    middleName = "";
                }

                string lastName = tbOwnerLastName.Text;
                if (!Regex.IsMatch(lastName, Globals.nameRegEx) || lastName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "Last name characters must be between 1 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (lastName.Length == 0 || lastName == "Last name")
                {
                    MessageBox.Show(this, "Please, input last name.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string address = tbOwnerAddress.Text;
                if (address.Length == 0 || address.Length > 50)
                {
                    MessageBox.Show(this, "Please, input address line (less than 50 characters).", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string city = tbOwnerCity.Text;
                if (city.Length == 0 || city.Length > 50)
                {
                    MessageBox.Show(this, "Please, input city (less than 50 characters).", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string ownerPostalCode = tbOwnerZIP.Text;
                if (!Regex.IsMatch(ownerPostalCode, Globals.zipRegEx)) // RegEx
                {
                    MessageBox.Show(this, "Invalid postal code.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string primaryPhone = tbOwnerPrimaryPhone.Text;
                if (!Regex.IsMatch(primaryPhone, Globals.phoneRegEx) || primaryPhone.Length == 0) // RegEx
                {
                    MessageBox.Show(this, "Invalid primary phone number.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string secondaryPhone = tbOwnerSecondaryPhone.Text;
                if (secondaryPhone == "Secondary") // can be null
                {
                    secondaryPhone = "";
                }
                if (secondaryPhone.Length > 0)
                {
                    if (!Regex.IsMatch(secondaryPhone, Globals.phoneRegEx)) // RegEx
                    {
                        MessageBox.Show(this, "Invalid secondary phone number.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                string email = tbOwnerEmail.Text; // nvarchar(30)--> regex up to 30 characters
                if (!Globals.IsValid(email)) // RegEx
                {
                    MessageBox.Show(this, "Invalid email.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // adding an owner
                Owner owner = new Owner()
                {
                    Name = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    PrimaryPhone = primaryPhone,
                    SecondaryPhone = secondaryPhone,
                    Address = address,
                    PostalCode = ownerPostalCode,
                    City = city,
                    Province = (ProvinceType)comboOwnerProvince.SelectedValue-1, // SelectedIndex
                    Country = tbOwnerCountry.Text,
                    Email = email,
                    Fostering = false
                };
                Globals.ctx.Owners.Add(owner);
                Globals.ctx.SaveChanges();

                var animal = Globals.ctx.Animals.Find(comboPetId.SelectedValue);
                // var aaaa = Globals.ctx.Animals.FirstOrDefault(c => c.AnimalID == (int)comboPetId.SelectedValue && ||);

                //updating crate
                var crate = Globals.ctx.Crates.FirstOrDefault(c => c.CrateID == animal.CrateID);
                crate.Status = false; // removing crate id from animal table, we also change status in crate table to not occupied/false
                Globals.ctx.SaveChanges();

                // updating animal                   
                animal.DateAdopted = dpAdoptionDate.SelectedDate;
                animal.CrateID = null; // adopted pet cannot have crate anymore
                animal.OwnerID = owner.OwnerID; // we add owner id inanimal table
                                                //Globals.ctx.Entry(animal).State = System.Data.Entity.EntityState.Modified;
                Globals.ctx.SaveChanges();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Database operation failed:\n" + ex.Message);
            }

            DialogResult = true;
        }

        private void btPrint_Click(object sender, RoutedEventArgs e)
        {

            /* ******** Print ******** */
            try
            {
                this.IsEnabled = false;
                PrintDialog printDlg = new PrintDialog();
                if(printDlg.ShowDialog() == true)
                {
                    btSaveExit.Visibility = Visibility.Hidden;
                    btPrint.Visibility = Visibility.Hidden;
                    btCancel.Visibility = Visibility.Hidden;
                    printDlg.PrintVisual(this, "Adoption Form");
                }
            }
            finally
            {
                this.IsEnabled = true;
                // Make buttons visible again
                btSaveExit.Visibility = Visibility.Visible;
                btPrint.Visibility = Visibility.Visible;
                btCancel.Visibility = Visibility.Visible;
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

        // displaying Adoptive pet's information
        private void comboPetId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selAnimalID = (int)comboPetId.SelectedValue; //SelectedIndex
            var animal = Globals.ctx.Animals.FirstOrDefault(a=> a.AnimalID == selAnimalID);
            tbPetName.Text = animal.Name;
            tbSpecies.Text = animal.Species.ToString();
            tbBreed.Text = animal.Breed.BreedName;
            tbGender.Text = animal.Gender.ToString();
            tbWeight.Text = animal.Weight.ToString();
            tbMicrochipped.Text = animal.Microchipped ? "Yes" : "No";
            tbColor.Text = animal.Breed.Color.ToString();
            tbWormed.Text = animal.Wormed ? "Yes" : "No";
            tbNeutured.Text = animal.Neutured ? "Yes" : "No";
            tbDOB.Text = animal.DOB?.ToString("dd MMMM yyyy");
        }

    } // end partial class AdoptionFormDlg
}
