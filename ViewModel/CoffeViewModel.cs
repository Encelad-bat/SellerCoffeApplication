using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SellerCoffeApplication.Model;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Dapper;

namespace SellerCoffeApplication.ViewModel
{
    class CoffeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public ObservableCollection<Coffe> Drinks { get; set; }

        private Coffe selectedCoffe;

        public Coffe SelectedCoffe
        {
            get { return selectedCoffe; }
            set { selectedCoffe = value; OnPropertyChanged("SelectedCoffe"); }
        }


        private RelayCommand sellCommand;
        public RelayCommand SellCommand {

            get
            {
                return sellCommand ?? (sellCommand = new RelayCommand(selled =>
                {
                    if ((selled as Coffe).Quanity != 1)
                    {
                        (selled as Coffe).Quanity--;
                        int id = Drinks.IndexOf((selled as Coffe));
                        Drinks.Remove((selled as Coffe));
                        Drinks.Insert(id,(selled as Coffe));
                        using (IDbConnection connection = new SqlConnection("Data Source=SQL5103.site4now.net;Initial Catalog=DB_A717C9_MAIN;User Id=DB_A717C9_MAIN_admin;Password=Gorb_bc24"))
                        {
                            connection.Open();

                            using(var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    connection.Execute("UPDATE Coffe set Quanity=@quanity WHERE Id=@id", new { quanity=(selled as Coffe).Quanity, id=(selled as Coffe).Id},transaction);
                                    transaction.Commit();
                                }
                                catch
                                {

                                    transaction.Rollback();
                                }
                            }
                        }
                    }
                    else
                    {
                        Drinks.Remove((selled as Coffe));
                        using (IDbConnection connection = new SqlConnection("Data Source=SQL5103.site4now.net;Initial Catalog=DB_A717C9_MAIN;User Id=DB_A717C9_MAIN_admin;Password=Gorb_bc24"))
                        {
                            connection.Open();

                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    connection.Execute("DELETE FROM Coffe WHERE Id=@id", new {id = (selled as Coffe).Id }, transaction);
                                    transaction.Commit();
                                }
                                catch
                                {

                                    transaction.Rollback();
                                }
                            }
                        }
                    }
                }));
            }
                
        }

        public CoffeViewModel()
        {
            using (IDbConnection connection = new SqlConnection("Data Source=SQL5103.site4now.net;Initial Catalog=DB_A717C9_MAIN;User Id=DB_A717C9_MAIN_admin;Password=Gorb_bc24"))
            {
                Drinks = new ObservableCollection<Coffe>(connection.Query<Coffe>("SELECT * FROM Coffe;").ToList());
            }
        }
    }
}
