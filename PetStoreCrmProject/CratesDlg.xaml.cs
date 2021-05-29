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
    /// Interaction logic for CratesDlg.xaml
    /// </summary>
    public partial class CratesDlg : Window
    {
        public CratesDlg()
        {
            InitializeComponent();
        }


        private void SeeAllCrates_Click(object sender, RoutedEventArgs e)
        {
            var crates = from c in Globals.ctx.Crates
                         select c;
            this.dgAllBreeds.ItemsSource = crates.ToList();
        }

        private void SeeAvailableCrates_Click(object sender, RoutedEventArgs e)
        {
            var crates = from c in Globals.ctx.Crates
                         where c.Status == false
                         select c;

            
            this.dgAllBreeds.ItemsSource = crates.ToList();
        }

        private void SeeOccupiedCrates_Click(object sender, RoutedEventArgs e)
        {
            var animals = from a in Globals.ctx.Animals
                          join c in Globals.ctx.Crates
                          on a.CrateID equals c.CrateID
                          where a.CrateID != null
                          select new
                          {
                              CrateID = a.CrateID,
                              AnimalID = a.AnimalID,
                              AnimalName = a.Name,
                              Status = c.Status,
                              Size = c.Size
                          };


            this.dgAllBreeds.ItemsSource = animals.ToList();
        }
    }
}
