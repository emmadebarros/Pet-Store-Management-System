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
    /// Interaction logic for AddAnimalDlg.xaml
    /// </summary>
    public partial class AddAnimalDlg : Window
    {
        public byte[] currPetImage;
        Animal currAnimal;
        bool isUpdate = false;
        List<Crate> crate = new List<Crate>();
        public AddAnimalDlg(Animal animal = null)
        {
            try
            {
                InitializeComponent();
                tbInvisible.Visibility = Visibility.Hidden;
                comboGender.ItemsSource = Enum.GetValues(typeof(GenderType)).Cast<GenderType>();
                comboSpecies.ItemsSource = Enum.GetValues(typeof(SpeciesType)).Cast<SpeciesType>();
                if(animal == null)
                {
                    crate = Globals.ctx.Crates.Where(c => !c.Status).ToList();
                    comboCrateId.ItemsSource = crate;
                    comboCrateId.DisplayMemberPath = nameof(Crate.CrateID);
                    comboCrateId.SelectedValuePath = nameof(Crate.CrateID);
                    comboGender.SelectedIndex = 0;
                    comboSpecies.SelectedIndex = 1;
                    dpDateArrived.SelectedDate = DateTime.Today;
                } else
                {
                    currAnimal = animal;
                    btnAdd.Content = "Update";
                    crate = Globals.ctx.Crates.Where(c => !c.Status || (currAnimal.CrateID.HasValue && c.CrateID==currAnimal.CrateID)).ToList();
                    comboCrateId.ItemsSource = crate;
                    comboCrateId.DisplayMemberPath = nameof(Crate.CrateID);
                    comboCrateId.SelectedValuePath = nameof(Crate.CrateID);                  
                    tbPetName.Text = currAnimal.Name;
                    comboSpecies.SelectedValue = currAnimal.Species;
                    comboBreed.SelectedValue = currAnimal.BreedID;//
                    comboGender.SelectedValue = currAnimal.Gender;
                    dpDOB.SelectedDate = currAnimal.DOB;
                    dpDateArrived.SelectedDate = currAnimal.DateArrived;
                    dpDateAdopted.SelectedDate = currAnimal.DateAdopted;
                    sliderWeight.Value = (double)currAnimal.Weight;
                    cbMicrochipped.IsChecked = currAnimal.Microchipped;
                    cbWormed.IsChecked = currAnimal.Wormed;
                    cbNeutered.IsChecked = currAnimal.Neutured;
                    tbDescription.Text = currAnimal.Description;
                    if (currAnimal.Photo != null)
                    {
                        imagePreview.Source = ByteArrayToBitmapImage(currAnimal.Photo);
                    }
                    comboCrateId.SelectedValue = currAnimal.CrateID;
                    tbInvisible.Text = animal.AnimalID.ToString();// for animal update
                }
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }
        }

        public void ClearInputs()
        {
            tbPetName.Text = "Text here...";
            comboSpecies.SelectedIndex = 1;
            comboBreed.SelectedIndex = 0;
            comboGender.SelectedIndex = 0;
            dpDOB.SelectedDate = null;
            dpDateArrived.SelectedDate = DateTime.Today;
            dpDateAdopted.SelectedDate = null;
            sliderWeight.Value = 18.2;
            cbMicrochipped.IsChecked = false;
            cbWormed.IsChecked = false;
            cbNeutered.IsChecked = false;
            tbDescription.Text = "Description...";
            tbImage.Visibility = Visibility.Visible;
            imagePreview.Source = null;
            comboCrateId.SelectedIndex = -1;

            if(isUpdate == false)
            {
                crate = Globals.ctx.Crates.Where(c => !c.Status).ToList();
                comboCrateId.ItemsSource = crate;
            }
        }

        private void btAdd(object sender, RoutedEventArgs e)
        {
            try
            {
                // validations
                string name = tbPetName.Text;
                if (!Regex.IsMatch(name, Globals.nameRegEx) || name.Length > 50) // nvarchar(50) RegEx
                {
                    MessageBox.Show(this, "Name characters must be between 0 to 50, made up of letters, digits, space, %_-(),/\\.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (name == "Text here...") // can be null
                {
                    name = "";
                }

                string description = tbDescription.Text; // nvarchar(200)
                if (description.Length > 200)
                {
                    MessageBox.Show(this, "Descrption must be less than 200 characters.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (description == "Description...")
                {
                    description = "";
                }

                GenderType gender = (GenderType)comboGender.SelectedValue;
                if (comboGender.SelectedIndex == -1)
                {
                    {
                        MessageBox.Show(this, "Please, select the gender.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                DateTime? dateAdopted = dpDateAdopted.SelectedDate;
                DateTime? dateOfBirth = dpDOB.SelectedDate;
                DateTime dateArrived = dpDateArrived.SelectedDate.Value;
                if (dateArrived == null)
                {
                    {
                        MessageBox.Show(this, "Please select the date arrived", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (dateAdopted < dateArrived)
                {
                    MessageBox.Show(this, "Date arrrived should not be less than date adopted.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dateArrived < dateOfBirth)
                {
                    MessageBox.Show(this, "Date arrived should not be less than date of birth.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                int id = 0;
                if(!String.IsNullOrEmpty(tbInvisible.Text))
                {
                    if(Convert.ToInt32(tbInvisible.Text)>0)
                    {
                        isUpdate = true;
                        id = Convert.ToInt32(tbInvisible.Text);
                    }
                }
                if(isUpdate)
                {
                    var toUpdate = Globals.ctx.Animals.Where(a => a.AnimalID == id).SingleOrDefault();
                    // Update animal
                    toUpdate.Species = (SpeciesType)(int)comboSpecies.SelectedValue;
                    toUpdate.Gender = (GenderType)(int)gender;
                    toUpdate.BreedID = (int)comboBreed.SelectedValue;
                    toUpdate.Weight = (decimal)sliderWeight.Value;
                    toUpdate.Photo = currPetImage;
                    toUpdate.DOB = dateOfBirth; //can be nullable
                    toUpdate.Name = name;
                    toUpdate.DateArrived = dateArrived;
                    toUpdate.DateAdopted = dateAdopted; //can be nullable
                    toUpdate.Microchipped = cbMicrochipped.IsChecked.GetValueOrDefault();
                    toUpdate.Wormed = cbWormed.IsChecked.GetValueOrDefault();
                    toUpdate.Neutured = cbNeutered.IsChecked.GetValueOrDefault();
                    toUpdate.CrateID = (int?)comboCrateId.SelectedValue; // can be nullable
                    toUpdate.Description = description;
                    Globals.ctx.SaveChanges();

                    //Crate status that is being changed - change status "not occupied" / false
                    var crateNotOccupied = Globals.ctx.Crates.FirstOrDefault(c => c.CrateID == toUpdate.CrateID);
                    if (crateNotOccupied != null)
                    {
                    crateNotOccupied.Status = false; // change status in crate table to false/"not occupied"
                    }
                    Globals.ctx.SaveChanges();

                    //Current crate status change to occupied/true
                    var crateOccupied = Globals.ctx.Crates.Find(comboCrateId.SelectedValue);
                    if (crateNotOccupied != null)
                    {
                        crateOccupied.Status = true; // change status in crate table to true/"occupied"
                    }
                    Globals.ctx.SaveChanges();

                    lblRecordAdded.Content = "Record updated!";
                    ClearInputs();
                }
                else
                {
                    var crateId = comboCrateId.SelectedValue;
                    if (crateId == null)
                    {
                        MessageBox.Show(this, "Please, select a crate for the pet.", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Animal add
                    Animal animal = new Animal
                    {
                        Name = name,
                        Species = (SpeciesType)comboSpecies.SelectedValue,
                        BreedID = (int)comboBreed.SelectedValue,
                        Gender = (GenderType)(int)gender,
                        DOB = dateOfBirth,//can be nullable
                        DateArrived = dateArrived,
                        DateAdopted = dateAdopted, //can be nullable
                        Weight = (decimal)sliderWeight.Value,
                        Microchipped = cbMicrochipped.IsChecked.GetValueOrDefault(),
                        Wormed = cbWormed.IsChecked.GetValueOrDefault(),
                        Neutured = cbNeutered.IsChecked.GetValueOrDefault(),
                        Description = description,
                        Photo = currPetImage,
                        CrateID = (int)crateId // cannot be null when added
                    };
                    Globals.ctx.Animals.Add(animal);
                    Globals.ctx.SaveChanges();

                    //Crate status change
                    var crate = Globals.ctx.Crates.Find(comboCrateId.SelectedValue);
                    crate.Status = true; // change status in crate table to true(occupied)
                    Globals.ctx.SaveChanges();

                    lblRecordAdded.Content = animal.Name + " recorded!";
                    ClearInputs();
                }
            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Database operation failed:\n" + ex.Message);
            }
        }

        public void btImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.jpe;*.gif;*.png;*.bmp;*.tiff;*.tif;*.gif)|*.jpg;*.jpeg;*.jpe;*.gif;*.png;*.bmp;*.tiff;*.tif;*.gif|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    currPetImage = File.ReadAllBytes(openFileDialog.FileName); // IO Exception
                    tbImage.Visibility = Visibility.Hidden;
                    BitmapImage bitmap = ByteArrayToBitmapImage(currPetImage); // ex: SystemException
                    imagePreview.Source = bitmap;
                }
                catch (Exception ex) when (ex is SystemException || ex is IOException)
                {
                    MessageBox.Show(ex.Message, "File reading failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public BitmapImage ByteArrayToBitmapImage(byte[] currentOwnerImage)
        {
            MemoryStream ms = new MemoryStream(currentOwnerImage);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.EndInit();
            return bmp;
        }

        private void ComboSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpeciesType selSpecies = (SpeciesType)comboSpecies.SelectedValue;
            var breed = Globals.ctx.Breeds.Where(b => b.SpeciesCode == (int)selSpecies).OrderBy(b => b.BreedName).ToList();
            comboBreed.ItemsSource = breed;
            comboBreed.DisplayMemberPath = "BreedName";
            comboBreed.SelectedValuePath = "BreedID";
            comboBreed.SelectedIndex = 1;
        }

        // we don't close the window after adding an animal, so that the user can add another animal without closing and opening the window
        // if new animal is added we display in the label, that the record was added
        private void btDone(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        //* ******** Placeholder text ******** *//
        private void tbPetName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPetName.Text == "Text here...")
            {
                tbPetName.Text = "";
            }
        }

        private void tbPetName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbPetName.Text))
            {
                tbPetName.Text = "Text here...";
            }
        }

        private void tbDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbDescription.Text == "Description...")
            {
                tbDescription.Text = "";
            }
        }

        private void tbDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbDescription.Text))
            {
                tbDescription.Text = "Description...";
            }
        }

        // ******** Add Breed ******** // 
        private void btAddBreed_Click(object sender, RoutedEventArgs e)
        {
            BreedsDlg dlg = new BreedsDlg();
            dlg.tbSpecies.Text = comboSpecies.SelectedValue.ToString();
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Enum.TryParse(dlg.tbSpecies.Text, out SpeciesType type);
                    Breed breed = new Breed
                    {
                        BreedName = dlg.tbBreedName.Text,
                        Color = (ColorType)dlg.comboBreedColor.SelectedIndex,
                        Size = (SizeType)dlg.comboBreedSize.SelectedIndex,
                        FurType = (FurType)dlg.comboBreedFurType.SelectedIndex,
                        SpeciesCode = (int)type
                    };
                    Globals.ctx.Breeds.Add(breed);
                    Globals.ctx.SaveChanges();

                    SpeciesType selSpecies = (SpeciesType)comboSpecies.SelectedValue;
                    var breeds = Globals.ctx.Breeds.Where(b => b.SpeciesCode == (int)selSpecies).OrderBy(b => b.BreedName).ToList();
                    comboBreed.ItemsSource = breeds;
                    comboBreed.DisplayMemberPath = "BreedName";
                    comboBreed.SelectedValuePath = "BreedID";
                    comboBreed.SelectedValue = breed.BreedID;
                }
                catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
                {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show("Database operation failed:\n" + ex.Message);
                }
            }
        }

    }// end partial class AddUpdateAnimalDialog
}
