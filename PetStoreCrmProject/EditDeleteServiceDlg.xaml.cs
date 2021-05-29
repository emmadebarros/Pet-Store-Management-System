using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetStoreCrmProject
{
    /// <summary>
    /// Interaction logic for EditDeleteServiceDlg.xaml
    /// </summary>
    public partial class EditDeleteServiceDlg : Window
    {
        public EditDeleteServiceDlg()
        {
            InitializeComponent();
            FillDataGrid();

        }

        public static DataGrid dataGrid;

        private void FillDataGrid()
        {

            var anSer = from a in Globals.ctx.Animal_Services
                        select new
                        {
                            AnimalID = a.AnimalID,
                            EmployeeID = a.EmployeeID,
                            ServiceType = a.ServiceType,
                            ServiceDate = a.ServiceDate,
                            Description = a.Description
                        };
            gridAnService.ItemsSource = anSer.ToList();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (gridAnService.SelectedItems.Count == 0 || gridAnService.SelectedItems.Count > 1) return;
            try
            {
                int Id = (int)gridAnService.SelectedItem.GetType().GetProperty("AnimalID").GetValue(gridAnService.SelectedItem, null);
                int emid = (int)gridAnService.SelectedItem.GetType().GetProperty("EmployeeID").GetValue(gridAnService.SelectedItem, null);
                DateTime date = (DateTime)gridAnService.SelectedItem.GetType().GetProperty("ServiceDate").GetValue(gridAnService.SelectedItem, null);
                


                var toEdit = Globals.ctx.Animal_Services.Where(a => a.AnimalID == Id && a.EmployeeID == emid && a.ServiceDate == date).SingleOrDefault();

                if (toEdit != null)
                {
                    ServicesDlg sdlg = new ServicesDlg(toEdit);
                    sdlg.Owner = this;
                    sdlg.ShowDialog();
                    FillDataGrid();
                }
                else
                {
                    MessageBox.Show($"No service with given information found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            if (gridAnService.SelectedItems.Count == 0 || gridAnService.SelectedItems.Count > 1) return;

            try
            {
                int anid = (int)gridAnService.SelectedItem.GetType().GetProperty("AnimalID").GetValue(gridAnService.SelectedItem, null);
                int emid = (int)gridAnService.SelectedItem.GetType().GetProperty("EmployeeID").GetValue(gridAnService.SelectedItem, null);
                Enum type = (Enum)gridAnService.SelectedItem.GetType().GetProperty("ServiceType").GetValue(gridAnService.SelectedItem, null);
                DateTime date = (DateTime)gridAnService.SelectedItem.GetType().GetProperty("ServiceDate").GetValue(gridAnService.SelectedItem, null);
                string description = (string)gridAnService.SelectedItem.GetType().GetProperty("Description").GetValue(gridAnService.SelectedItem, null);


                var toDelete = Globals.ctx.Animal_Services.Where(a => a.AnimalID == anid && a.EmployeeID == emid && a.ServiceType.Equals(type) && a.ServiceDate == date && a.Description == description).SingleOrDefault();

                if (toDelete != null)
                {
                    Globals.ctx.Animal_Services.Remove(toDelete);
                    Globals.ctx.SaveChanges();
                    FillDataGrid();
                }
                else
                {
                    MessageBox.Show($"No service with given information found in DB", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Fatal error: Database connection failed:\n" + ex.Message);
                Environment.Exit(1);
            }
        }


    }
}