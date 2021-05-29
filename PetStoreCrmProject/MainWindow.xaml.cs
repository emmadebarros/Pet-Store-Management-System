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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //LoginDlg currLoginWin;

        public MainWindow(String x) //LoginDlg x
        {
            InitializeComponent();
            currUser.Content = $" {x}!";
            //currLoginWin = x;
            /*
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
            */
        }

        private void btAddAnimal_Click(object sender, RoutedEventArgs e)
        {
            AddAnimalDlg dlg = new AddAnimalDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            //get user confirmation before closing
            if (MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //LoginDlg ldlg = new LoginDlg();
                LoginDlg ldlg = new LoginDlg();
                this.Visibility = Visibility.Hidden;
                ldlg.Show();
                Close();
                //Fix Window_Closing()
            }
        }

        private void ListViewItemServices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected
            EditDeleteServiceDlg sdlg = new EditDeleteServiceDlg();
            sdlg.Owner = this;
            sdlg.ShowDialog();
        }

        private void btAdoptionForm_Click(object sender, RoutedEventArgs e)
        {
            AdoptionFormDlg dlg = new AdoptionFormDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit the program?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //currLoginWin.Close();
                Close();
                //System.Windows.Application.Current.Shutdown();
                /*
                 * Because of Login window, when going back to login, closing window wouldn't terminate program with window_closing event
                 */
            }
        }

        /* *** Drag Window Around *** */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /* *** LV Items Double Click *** */
        private void ListViewItemAnimals_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected
            AnimalsDlg adlg = new AnimalsDlg();
            adlg.Owner = this;
            adlg.ShowDialog();
        }

        private void lvitemBreeds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected
            ManageBreedsDlg dlg = new ManageBreedsDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void CratesDlg_MouseDoubleCLick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected
            CratesDlg dlg = new CratesDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void lvitemOwners_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected
            ManageOwnersDlg dlg = new ManageOwnersDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void LvStaffMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0) return; //return if nothing is selected

            /* ******** Print ******** */
            try
            {
                IsEnabled = false;
                StaffWizardDlg StaffWizDlg = new StaffWizardDlg();
                StaffWizDlg.Owner = this;
                StaffWizDlg.ShowDialog();
            }
            finally
            {
                IsEnabled = true;
            }
        }

        /* *** Add Service Btn *** */
        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            AddServiceDlg dlg = new AddServiceDlg();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        /*
            if(ldlg != null)
            {
                e.Cancel = true;
            } else
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //currLoginWin.Close();
                    e.Cancel = true;
                    //System.Windows.Application.Current.Shutdown();
                }
            }
            /*
            //get user confirmation before closing
            if (MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //currLoginWin.Close();
                e.Cancel = true;
                //System.Windows.Application.Current.Shutdown();
            }
            */
    }
}
