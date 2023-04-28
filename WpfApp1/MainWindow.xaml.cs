using MySql.Data.MySqlClient;
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
using System.IO;

namespace WpfApp1

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string kapcsolatLeiro = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";
        List<Termek> termekek = new List<Termek>();
        MySqlConnection SQLkapcsolat;

        public MainWindow()
        {
            InitializeComponent();

            AdatbazisMegnyitas();
            KategoriaBetoltese();
            GyartoBetoltese();

            TermekekBetolteseListba();

            AdatbazsLezarasa();
        }
        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            termekek.Clear();
            string SQLSzukitettLista = SzukitoLekerdezesEloallitasa();

            MySqlCommand SQLparancs = new MySqlCommand(SQLSzukitettLista, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString("Kategória"),
                                    eredmenyOlvaso.GetString("Gyártó"),
                                    eredmenyOlvaso.GetString("Név"),
                                    eredmenyOlvaso.GetInt32("Ár"),
                                    eredmenyOlvaso.GetInt32("Garidő"));
                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.Items.Refresh();
        }

        private void TermekekBetolteseListba()
        {
            string SQLOsszesTermek = "SELECT * FROM termékek;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLOsszesTermek, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString("Kategória"),
                                    eredmenyOlvaso.GetString("Gyártó"),
                                    eredmenyOlvaso.GetString("Név"),
                                    eredmenyOlvaso.GetInt32("Ár"),
                                    eredmenyOlvaso.GetInt32("Garidő"));
                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();

            dgTermekek.ItemsSource = termekek;
        }

        private void KategoriaBetoltese()
        {
            string SQLKategoriakRendezve = "SELECT DISTINCT kategória FROM termékek ORDER BY kategóriák;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLKategoriakRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvasa = SQLparancs.ExecuteReader();

            cbKategoria.Items.Add(" - Nincs megadva - ");
            while (eredmenyOlvasa.Read())
            {
                cbKategoria.Items.Add(eredmenyOlvasa.GetString("kategoria"));
            }
            eredmenyOlvasa.Close();
            cbKategoria.SelectedIndex = 0;
        }

        private void GyartoBetoltese()
        {
            string SQLGyartokRendezve = "SELECT DISTINCT gyártó FROM termékek ORDER BY gyártó;";

            MySqlCommand SQLparancs = new MySqlCommand(SQLGyartokRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbGyarto.Items.Add(" - Nics megadva - ");
            while (eredmenyOlvaso.Read())
            {
                cbGyarto.Items.Add(eredmenyOlvaso.GetString("Gyártó"));
            }
            eredmenyOlvaso.Close();
            cbGyarto.SelectedIndex = 0;
        }
        private void AdatbazisMegnyitas()
        {
            try
            {
                SQLkapcsolat = new MySqlConnection(kapcsolatLeiro);
                SQLkapcsolat.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Nem tud kapcsolódni az adatbázishoz");
                this.Close();

            }
        }

        private void AdatbazsLezarasa()
        {
            SQLkapcsolat.Close();
            SQLkapcsolat.Dispose();
        }

        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("cucc.csv");
            foreach (var item in termekek)
            {
                sw.WriteLine(item.ToCSVString());

            }
            sw.Close();
        }
        private string SzukitoLekerdezesEloallitasa()
        {
            bool vanFeltetel = false;
            string SQLSzukitettLista = "Select * FROM termékek";

            if (cbGyarto.SelectedIndex > 0 || cbKategoria.SelectedIndex > 0 ||txtTermek.Text !="")
            {
                SQLSzukitettLista += "WHERE ";
            }
            if (cbGyarto.SelectedIndex >0)
            {
                SQLSzukitettLista += $"gyártó='{cbGyarto.SelectedIndex}'";
                vanFeltetel = true;
            }
            if (cbKategoria.SelectedIndex > 0)
            {
                if (vanFeltetel)
                {
                    SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"kategória='{cbKategoria.SelectedIndex}'";
                vanFeltetel = true;
            }
            if (txtTermek.Text != "")
            {
                if (vanFeltetel)
                {
                    SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"kategória'%{txtTermek.Text}%'";
            }
            return SQLSzukitettLista;
        }
    }
}
