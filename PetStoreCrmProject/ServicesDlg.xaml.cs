using Microsoft.Win32;
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
using System.Data;


namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for ServicesDlg.xaml
    ///// </summary>
    public partial class ServicesDlg : Window
    {
        DataLayer.Animal_Service currAnimalService;
        public ServicesDlg(DataLayer.Animal_Service animalService)
        {

            try
            {
                InitializeComponent();
                currAnimalService = animalService;
                bindcomboAnID();
                bindcomboEMID();
                ComboServiceListing.ItemsSource = Enum.GetValues(typeof(DataLayer.ServiceType)).Cast<DataLayer.ServiceType>();
                calServiceDate.SelectedDate = currAnimalService.ServiceDate;

                comboServiceAnimalID.SelectedValuePath = nameof(DataLayer.Animal.AnimalID);
                comboServiceAnimalID.DisplayMemberPath = nameof(DataLayer.Animal.AnimalID);
                comboServiceAnimalID.SelectedValue = currAnimalService.AnimalID;
                comboServiceEmployeeID.SelectedValuePath = nameof(DataLayer.Employee.EmployeeID);
                comboServiceEmployeeID.DisplayMemberPath = nameof(DataLayer.Employee.EmployeeID);
                comboServiceEmployeeID.SelectedValue = currAnimalService.EmployeeID;
                ComboServiceListing.SelectedItem = currAnimalService.ServiceType;
                calServiceDate.SelectedDate = currAnimalService.ServiceDate;
                tbServiceDescription.Text = currAnimalService.Description;


            }
            catch (SystemException ex) // catch-all for EF, SQL and many other exceptions
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error:\n" + ex.Message);
                Environment.Exit(1); // fatal error
            }


        }


        public List<DataLayer.Employee> employee { get; set; }
        private void bindcomboEMID()
        {
            var item = Globals.ctx.Employees.ToList();
            employee = item;
            DataContext = employee;

            comboServiceEmployeeID.ItemsSource = employee;
            comboServiceEmployeeID.SelectedValuePath = nameof(DataLayer.Employee.EmployeeID);
            comboServiceEmployeeID.DisplayMemberPath = nameof(DataLayer.Employee.EmployeeID);
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
            comboServiceAnimalID.SelectedValuePath = nameof(DataLayer.Animal.AnimalID);
            comboServiceAnimalID.DisplayMemberPath = nameof(DataLayer.Animal.AnimalID);
            comboServiceAnimalID.SelectedIndex = 1;
        }

        private void comboServiceAnimalID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = comboServiceAnimalID.SelectedItem as DataLayer.Animal;
            lblServiceAnimalName.Content = item.Name;

        }

        private void EditService_Button(object sender, RoutedEventArgs e)
        {
            //List<DataLayer.Animal_Service> animalServiceList = new List<DataLayer.Animal_Service>();

            DateTime sServisDate = calServiceDate.SelectedDate.Value;
            try
            {
                if (sServisDate == null)

                {
                    MessageBox.Show(this, "Please select the date of the service", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if (DateTime.Today < sServisDate)
                {
                    MessageBox.Show(this, "Date sgould be not be in further then the current date", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                currAnimalService.AnimalID = (int)comboServiceAnimalID.SelectedValue;
                currAnimalService.EmployeeID = (int)comboServiceEmployeeID.SelectedValue;
                currAnimalService.ServiceType = (DataLayer.ServiceType)ComboServiceListing.SelectedValue;
                currAnimalService.ServiceDate = sServisDate;
                currAnimalService.Description = tbServiceDescription.Text;
                Globals.ctx.SaveChanges();
                //EditDeleteServiceDlg.dataGrid.ItemsSource = Globals.ctx.Animal_Services.ToList();
                DialogResult = true;
            }
                catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Database operation failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

