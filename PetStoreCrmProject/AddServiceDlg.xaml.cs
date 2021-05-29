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
    /// Interaction logic for AddServiceDlg.xaml
    /// </summary>
    public partial class AddServiceDlg : Window
    {
        public AddServiceDlg()
        {
            InitializeComponent();
            bindcomboAnID();
            bindcomboEMID();
            ComboServiceListing.ItemsSource = Enum.GetValues(typeof(DataLayer.ServiceType)).Cast<DataLayer.ServiceType>();
            ComboServiceListing.SelectedIndex = 1;
            calServiceDate.SelectedDate = DateTime.Today;
        }
        public List<DataLayer.Employee> employee { get; set; }
        private void bindcomboEMID()
        {
            var item = Globals.ctx.Employees.ToList();
            employee = item;
            DataContext = employee;
            
            comboServiceEmployeeID.ItemsSource = employee;
            comboServiceEmployeeID.SelectedValuePath = "EmployeeID";
            comboServiceEmployeeID.DisplayMemberPath = "EmployeeID";
            comboServiceEmployeeID.SelectedIndex = 1;
        }

        private void comboServiceEmployeeID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = comboServiceEmployeeID.SelectedItem as DataLayer.Employee;
            lblServiceEmployeeName.Content = item.Name;

        }

        public List<DataLayer.Animal> animal { get; set; }
        private void bindcomboAnID()
        {
            var item = Globals.ctx.Animals.ToList();
            animal = item;
            DataContext = animal;
            
            comboServiceAnimalID.ItemsSource = animal;
            comboServiceAnimalID.SelectedValuePath = "AnimalID";
            comboServiceAnimalID.DisplayMemberPath = "AnimalID";
            comboServiceAnimalID.SelectedIndex = 1;
        }

        private void comboServiceAnimalID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = comboServiceAnimalID.SelectedItem as DataLayer.Animal;
            lblServiceAnimalName.Content = item.Name;

        }



        private void AddService_Button(object sender, RoutedEventArgs e)
        {
            List<DataLayer.Animal_Service> animalServiceList = new List<DataLayer.Animal_Service>();
            
            DateTime sServisDate = calServiceDate.SelectedDate.Value;
            if (sServisDate == null)
            {
                {
                    MessageBox.Show(this, "Please select the date of the service", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (DateTime.Today < sServisDate)
            {
                MessageBox.Show(this, "Date sgould be not be in further then the current date", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DataLayer.Animal_Service animalServices = new DataLayer.Animal_Service()
                {
                    AnimalID = (int)comboServiceAnimalID.SelectedValue,
                    EmployeeID = (int)comboServiceEmployeeID.SelectedValue,
                    ServiceType = (DataLayer.ServiceType)ComboServiceListing.SelectedValue,
                    ServiceDate = sServisDate,
                    Description = tbServiceDescription.Text
                };

                
                Globals.ctx.Animal_Services.Add(animalServices);
                Globals.ctx.SaveChanges();
                ClearInputs();
                lblServiceResult.Content = "Record added!";
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Database operation failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ClearInputs()
        {

            comboServiceAnimalID.SelectedIndex = 1;
            comboServiceEmployeeID.SelectedIndex = 4;
            ComboServiceListing.SelectedItem = 1;
            calServiceDate.SelectedDate = null;
            tbServiceDescription.Text = "Text here...";

        }

        private void tbServiceDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbServiceDescription.Text == "Text here...")
            {
                tbServiceDescription.Text = "";
            }
        }

        private void tbServiceDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbServiceDescription.Text))
            {
                tbServiceDescription.Text = "Text here...";
            }

        }

        private void Close_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnServiceDone(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
