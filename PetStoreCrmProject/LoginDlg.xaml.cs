using System;
using System.Collections.Generic;
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

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for LoginDlg.xaml
    /// </summary>
    public partial class LoginDlg : Window
    {
        public LoginDlg()
        {
            try
            {
                InitializeComponent();
                Globals.ctx = new DataLayer.PetStoreDbContext();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            /* *** Verif 1 - Placeholder and Got Focus *** */
            if ((string.IsNullOrWhiteSpace(tbUsername.Text) || tbUsername.Text == "Username") && (string.IsNullOrWhiteSpace(tbPassword.Password) || tbPassword.Password == "example"))
            {
                MessageBox.Show(this, "Please enter a username and password to login.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(tbUsername.Text)|| tbUsername.Text == "Username")
            {
                MessageBox.Show(this, "Please enter a username to login.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(tbPassword.Password) || tbPassword.Password == "example")
            {
                MessageBox.Show(this, "Please enter a password to login.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            /* *** Verif 2 *** */
            String username = tbUsername.Text;
            String password = tbPassword.Password;
            var employee = Globals.ctx.Employees.Where(emp => emp.Email == username.Trim()).ToList();
            //var employee = Globals.ctx.Employees.Where(emp => emp.Email.Trim().CompareTo(username.Trim()) == 0).ToList();
            //if I use SingleOrDefault must catch null ref exception
            if (employee.Count == 0|| employee.Count > 1) //double verif on unique email, just in case
            {
                //Invalid Login only to prevent from hackers
                MessageBox.Show(this, "Invalid Login.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(employee[0].CrmPassword == password )
            {
                MainWindow mw = new MainWindow(employee[0].Name); //this
                Hide();
                mw.Show();
                this.Close();
            } else
            {
                //Invalid Login only to prevent from hackers
                MessageBox.Show(this, "Invalid Login.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /* *** Drag Window Around *** */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /* *** PlaceHolders *** */
        private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text == "Username")
            {
                tbUsername.Text = "";
            }
        }

        private void tbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Password == "example")
            {
                tbPassword.Password = "";
            }
        }

        private void tbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                tbUsername.Text = "Username";
            }
        }

        private void tbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                tbPassword.Password = "example";
            }
        }

        /* *** EXIT *** */
        private void btnLoginExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /* *** Enter Key *** */
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
