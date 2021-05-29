using PetStoreCrmProject.DataLayer;
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

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for BreedsDlg.xaml
    /// </summary>
    public partial class BreedsDlg : Window
    {
        public BreedsDlg()
        {         
            try
            {
                InitializeComponent();
                //tbSpecies.Text = addDlg.comboSpecies.SelectedValue.ToString(); // if to update we should let it change
                comboBreedColor.ItemsSource = Enum.GetValues(typeof(ColorType)).Cast<ColorType>();
                comboBreedColor.SelectedIndex = 1;
                comboBreedSize.ItemsSource = Enum.GetValues(typeof(SizeType)).Cast<SizeType>();
                comboBreedSize.SelectedIndex = 2;
                comboBreedFurType.ItemsSource = Enum.GetValues(typeof(FurType)).Cast<FurType>();
                comboBreedFurType.SelectedIndex = 1;
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        private void btAddBreed_Click(object sender, RoutedEventArgs e)
        {

            //Enum.TryParse(tbSpecies.Text, out SpeciesType type);
            string breedName = tbBreedName.Text;
            if (!Regex.IsMatch(breedName, Globals.nameRegEx)) // nvarchar(50)
            {
                MessageBox.Show(this, "Breed name characters must be between 1 to 50, made up of letters, digits, space, %_-(),./\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (breedName.Length == 0 || breedName == "Text here...")
            {
                MessageBox.Show(this, "Please, input the breed name.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;// assign result and dismiss the dialog
        }

        // ******** Placeholder text ******** //
        private void tbBreedName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbBreedName.Text == "Text here...")
            {
                tbBreedName.Text = "";
            }
        }

        private void tbBreedName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbBreedName.Text))
            {
                tbBreedName.Text = "Text here...";
            }
        }

    } // end partial class BreedsDlg
}
