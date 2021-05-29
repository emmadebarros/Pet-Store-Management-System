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
    /// Interaction logic for ManageBreedsDlg.xaml
    /// </summary>
    public partial class ManageBreedsDlg : Window
    {
        Breed currSelBreed;
        public ManageBreedsDlg(Breed breed = null)
        {
            try
            {
                InitializeComponent();
                lvBreedList.ItemsSource = (from i in Globals.ctx.Breeds.ToList() select i).ToList();
                comboSpecies.ItemsSource = Enum.GetValues(typeof(SpeciesType)).Cast<SpeciesType>();
                comboSpecies.SelectedIndex = 0;
                comboColor.ItemsSource = Enum.GetValues(typeof(ColorType)).Cast<ColorType>();
                comboColor.SelectedIndex = 0;
                comboSize.ItemsSource = Enum.GetValues(typeof(SizeType)).Cast<SizeType>();
                comboSize.SelectedIndex = 0;
                comboFurType.ItemsSource = Enum.GetValues(typeof(FurType)).Cast<FurType>();
                comboFurType.SelectedIndex = 0;
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
            
        }

        private void lvBreedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currSelBreed = lvBreedList.SelectedItem as Breed; // (Breed)lvBreedList.SelectedItem
            if (currSelBreed == null) //
            {
                ClearInputs();
            }
            else
            {
                lbBreedId.Content = currSelBreed.BreedID;
                comboSpecies.SelectedIndex = (int)(SpeciesType)currSelBreed.SpeciesCode-1; // currSelBreed.SpeciesCode // glitch -1
                tbBreedName.Text = currSelBreed.BreedName;
                comboColor.SelectedValue = currSelBreed.Color;
                comboSize.SelectedValue = currSelBreed.Size;
                comboFurType.SelectedValue = currSelBreed.FurType;
                btAdd.IsEnabled = false;
                btUpdate.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
        }

        private void ClearInputs()
        {
            lbBreedId.Content = "";
            comboSpecies.SelectedIndex = 0;
            tbBreedName.Text = "";
            comboColor.SelectedIndex = 0;
            comboSize.SelectedIndex = 0;
            comboFurType.SelectedIndex = 0;
            btAdd.IsEnabled = true;
            btUpdate.IsEnabled = false;
            btDelete.IsEnabled = false;
            lvBreedList.SelectedItem = null;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = tbBreedName.Text;
                if (!Regex.IsMatch(name, Globals.nameRegEx) || name.Length > 50 || name.Length == 0) // nvarchar(50) RegEx
                {
                    MessageBox.Show(this, "Name characters must be between 1 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Breed breed = new Breed
                {
                    SpeciesCode = (int)comboSpecies.SelectedValue, 
                    BreedName = name,
                    Color = (ColorType)comboColor.SelectedValue,
                    Size = (SizeType)comboSize.SelectedValue,
                    FurType = (FurType)comboFurType.SelectedValue
                };
                Globals.ctx.Breeds.Add(breed);
                Globals.ctx.SaveChanges();
                ClearInputs();
                lvBreedList.ItemsSource = (from i in Globals.ctx.Breeds.ToList() select i).ToList();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Database operation failed:\n" + ex.Message);
            }

        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            currSelBreed = lvBreedList.SelectedItem as Breed; // (Breed)lvBreedList.SelectedItem
            if (currSelBreed == null) // never going to happen, because it is disabled if null
            {
                MessageBox.Show(this, "Please select an item to update", "Selection error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                currSelBreed.SpeciesCode = (int)comboSpecies.SelectedValue;
                currSelBreed.BreedName = tbBreedName.Text;
                currSelBreed.Color = (ColorType)comboColor.SelectedValue;
                currSelBreed.Size = (SizeType)comboSize.SelectedValue;
                currSelBreed.FurType = (FurType)comboFurType.SelectedValue;

                Globals.ctx.SaveChanges();
                ClearInputs();
                lvBreedList.ItemsSource = (from i in Globals.ctx.Breeds.ToList() select i).ToList();
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Database operation failed:\n" + ex.Message);
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            currSelBreed = lvBreedList.SelectedItem as Breed; // (Breed)lvBreedList.SelectedItem
            if (currSelBreed == null) // never going to happen, because it is disabled if null
            {
                MessageBox.Show(this, "Please select an item to delete", "Selection error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Do you want to delete: " + currSelBreed.BreedName + " ?", "Data save", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Globals.ctx.Breeds.Remove(currSelBreed);
                Globals.ctx.SaveChanges();
                ClearInputs(); 
                lvBreedList.ItemsSource = (from i in Globals.ctx.Breeds.ToList() select i).ToList();
            }
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;// assign result and dismiss the dialog
        }

    }// end partial class ManageBreedsDlg
}
