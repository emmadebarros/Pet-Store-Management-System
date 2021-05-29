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
    /// Interaction logic for StaffWizardDlg.xaml
    /// </summary>
    public partial class StaffWizardDlg : Window
    {
        public StaffWizardDlg()
        {
            InitializeComponent();
            comboPosition.ItemsSource = Enum.GetValues(typeof(PositionType)).Cast<PositionType>();
            comboProvince.ItemsSource = Enum.GetValues(typeof(ProvinceType)).Cast<PositionType>();
            comboPosition.SelectedIndex = 0;
            comboProvince.SelectedIndex = 0;
        }

        /* *** Drag Window Around *** */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void tbEmpMiddle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmpMiddle.Text == "optional")
            {
                tbEmpMiddle.Text = "";
            }
        }

        private void tbEmpMiddle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbEmpMiddle.Text))
            {
                tbEmpMiddle.Text = "optional";
            }
        }

        private void tbEmpLast_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbEmpL.Text))
            {
                Page1.CanSelectNextPage = false;
            }
            else
            {
                Page1.CanSelectNextPage = true;
            }
        }

        //FIXME: don't like this design, find a solution to get rid of code duplication when time!
        private void tbSP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbSP.Text == "optional")
            {
                tbSP.Text = "";
            }
        }

        private void tbSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSP.Text))
            {
                tbSP.Text = "optional";
            }
        }

        private void tbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbEmail.Text))
            {
                Page2.CanSelectNextPage = false;
            }
            else
            {
                Page2.CanSelectNextPage = true;
            }
        }

        private void FinishButton_Clicked(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            try
            {
                /* *** First name *** */
                string firstName = tbEmpFirst.Text;
                if (!Regex.IsMatch(firstName, Globals.nameRegEx) || firstName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "First name characters must be between 1 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (firstName.Length == 0)
                {
                    MessageBox.Show(this, "Please, input first name.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** Middle Name *** */
                string middleName = tbEmpMiddle.Text;
                if (!Regex.IsMatch(middleName, Globals.nameRegEx) || middleName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "Middle name characters must be between 0 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (middleName == "optional") // can be null
                {
                    middleName = "";
                }

                /* *** Last Name *** */
                string lastName = tbEmpL.Text;
                if (!Regex.IsMatch(lastName, Globals.nameRegEx) || lastName.Length > 50) // nvarchar(50), RegEx
                {
                    MessageBox.Show(this, "Last name characters must be between 1 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (lastName.Length == 0)
                {
                    MessageBox.Show(this, "Please, input last name.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** Address *** */
                string address = tbAddress.Text;
                if (address.Length == 0 || address.Length > 50)
                {
                    MessageBox.Show(this, "Please, input address line (less than 50 characters).", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** City *** */
                string city = tbCity.Text;
                if (city.Length == 0 || city.Length > 50)
                {
                    MessageBox.Show(this, "Please, input city (less than 50 characters).", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** Postal Code *** */
                string userPostalCode = tbPostalCode.Text;
                if (!Regex.IsMatch(userPostalCode, Globals.zipRegEx)) // RegEx
                {
                    MessageBox.Show(this, "Invalid postal code.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** Primary Phone *** */
                string primaryPhone = tbPP.Text;
                if (!Regex.IsMatch(primaryPhone, Globals.phoneRegEx) || primaryPhone.Length == 0) // RegEx
                {
                    MessageBox.Show(this, "Invalid primary phone number.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /* *** Secondary Phone *** */
                string secondaryPhone = tbSP.Text;
                if (secondaryPhone == "optional") // can be null
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

                /* *** Email *** */
                string email = tbEmail.Text; // nvarchar(30)--> regex up to 30 characters
                if (!Regex.IsMatch(email, Globals.emailRegEx)) // RegEx
                {
                    MessageBox.Show(this, "Invalid email.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime start = dpSD.SelectedDate.Value;

                string SIN = tbSin.Text;
                if (!Regex.IsMatch(SIN, Globals.SIN))
                {
                    MessageBox.Show(this, "Invalid SIN number, must be in the form '111 111 111'.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string pw = pwPassword.Password;
                var empPW = Globals.ctx.Employees.Where(em => em.CrmPassword == pw).ToList();

                if (empPW.Count != 0)
                {
                    MessageBox.Show("Invalid Password", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string note = tbNote.Text;
                if(note.Length > 200)
                {
                    MessageBox.Show("Note can't exceed 200 characters.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Employee emp = new Employee() { Name = firstName, MiddleName = middleName, LastName = lastName, Position = (PositionType)comboPosition.SelectedValue, StartDate = start, PrimaryPhone = primaryPhone, SecondaryPhone = secondaryPhone, Email = email, CrmPassword = pw, Address = address, PostalCode = userPostalCode, City = city, Province = (ProvinceType)comboProvince.SelectedValue, Country = tbCountry.Text, SIN=SIN, Note=note};

                Globals.ctx.Employees.Add(emp);
                Globals.ctx.SaveChanges();
                wiz.FinishButtonClosesWindow = true;
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
            /*
            string email = tbOwnerEmail.Text; // nvarchar(30)--> regex up to 30 characters
            if (!Regex.IsMatch(email, Globals.emailRegEx)) // RegEx
            {
                MessageBox.Show(this, "Invalid email.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        };*/
        }
    }
}
