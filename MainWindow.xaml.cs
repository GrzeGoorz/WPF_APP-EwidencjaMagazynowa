using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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

namespace APLIKACJA_SQL
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();


            //Połączenie do bazy danych SQL o nazwie : "MagazynDBConnectionString"
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_MAGAZYN.Properties.Settings.MagazynDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowAllProdukt();
            ShowMagazyn();
            

        }
        //### OKNO TRZECIE
        private void ShowAllProdukt()
        {
            try
            {
                string query = "select * from Produkt";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable produktTable = new DataTable();
                    sqlDataAdapter.Fill(produktTable);

                    listaAllProdukt.DisplayMemberPath = "Nazwa";
                    listaAllProdukt.SelectedValuePath = "Id";
                    listaAllProdukt.ItemsSource = produktTable.DefaultView;
                }
            }
            catch (Exception e) 
            {
               // MessageBox.Show(e.ToString());
            }
            
        }

        //Tworzenie zapytania SQL
        private void ShowMagazyn()
        {
            try 
            {
                string query = "select * from Magazyn";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    //Tworzenie Tabeli
                    DataTable magazynTable = new DataTable();

                    //Fill(DataSet) - Dodaje lub odświeża wiersze w obiekcie DataSet , aby dopasować je do tych w źródle danych.
                    sqlDataAdapter.Fill(magazynTable);

                    //dodatnie do ListBox "listaLokalizacji" informacji z tabeli, które mają być wyświetlone - w tym przypadku :Lokalizacja
                    listaLokalizacji.DisplayMemberPath = "Lokalizacja";
                    //dodatnie do ListBox "listaLokalizacji" przekazania zaznaczonej wartości - w tym przypadku: Id
                    listaLokalizacji.SelectedValuePath = "Id";
                    //dodatnie do ListBox "listaLokalizacji" domyslnego widoku magazynTable (query = "select * from Magazyn")
                    listaLokalizacji.ItemsSource = magazynTable.DefaultView;
                }
            }
            catch (Exception e) 
            { 
                 MessageBox.Show(e.ToString());
            }
            
        }

        //############# OKNO DRUGIE - POJAWIANIE SIĘ PRODUKTÓW W "listaProduktLokalizacji" PO WYBRANIU MAGAZYNIU 

        private void ShowProduktLokalizacji()
        {
            try
            {   
                // utworzenie zaptania i połączenie do bazy
                string query = "select * from Produkt p inner join MagazynProdukt mp on p.Id = mp.ProduktId where mp.MagazynId = @MagazynId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                    //Tworzenie Tabeli
                    DataTable produktTable = new DataTable();

                    //Fill(DataSet) - Dodaje lub odświeża wiersze w obiekcie DataSet , aby dopasować je do tych w źródle danych.
                    sqlDataAdapter.Fill(produktTable);

                    //dodatnie do ListBox "listaLokalizacji" informacji z tabeli, które mają być wyświetlone - w tym przypadku :Lokalizacja
                    listaProduktLokalizacji.DisplayMemberPath = "Nazwa";
                    //dodatnie do ListBox "listaLokalizacji" przekazania zaznaczonej wartości - w tym przypadku: Id
                    listaProduktLokalizacji.SelectedValuePath = "Id";
                    //dodatnie do ListBox "listaLokalizacji" domyslnego widoku magazynTable (query = "select * from Magazyn")
                    listaProduktLokalizacji.ItemsSource = produktTable.DefaultView;
                }
            }
            catch (Exception e)
            {
               // MessageBox.Show(e.ToString());
            }

        }

        private void listaLokalizacji_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Wyświetla produkt
            ShowProduktLokalizacji();
            // AKTUALIZACJA TEXTU W TEXT_BOXIE
            ShowSelectedinTextBox1();
        }
        // PRZYCISK
        private void Button_Click_UsunLokalizacje(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "delete from Magazyn where id = @MagazynId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                sqlCommand.ExecuteScalar();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowMagazyn();
            }

        }
        // PRZYCISK DODAJ LOKALIZACJĘ
        private void Button_Click_DodajLokalizacjedoBazy(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Magazyn values (@Lokalizacja)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Lokalizacja", textBox1.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowMagazyn();
            }
        }
        // PRZYCISK DODAJ PRODUKT DO LOKALIZACJI
        private void Button_Click_DodajProduktdoLokalizacji(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into MagazynProdukt values (@MagazynId, @ProduktId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ProduktId", listaAllProdukt.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wybierz produkt z bazy oraz lokalizację");
            }
            finally
            {
                sqlConnection.Close();
                ShowProduktLokalizacji();
            }
        }
        // PRZYCISK DODAJ PRODUKT DO BAZY 
        private void Button_Click_DodajProduktdoBazy(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Produkt values (@Nazwa)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Nazwa", textBox2.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllProdukt();
            }
        }
        // USUŃ PRODUKT Z BAZY DANYCH
        private void Button_Click_UsunProduktzBazy(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Produkt where id = @ProduktId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProduktId", listaAllProdukt.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllProdukt();
                


            }
        }

        // // USUŃ PRODUKT Z lokalizacji
        private void Button_Click_UsunProduktzLokalizacji(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete TOP(1) from MagazynProdukt where MagazynId = @MagazynId AND ProduktId = @ProduktId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ProduktId", listaProduktLokalizacji.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wybierz produkt z lokalizacji aby usunąć");
            }
            finally
            {
                sqlConnection.Close();
                ShowProduktLokalizacji();



            }

        }

        // AKTUALIZACJA TEXTU W TEXT_BOXIE 1
        private void ShowSelectedinTextBox1()
        {
            try
            {
                // utworzenie zaptania i połączenie do bazy
                string query = "select Lokalizacja from Magazyn where Id = @MagazynId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                    //Tworzenie Tabeli
                    DataTable MagazynDataTable = new DataTable();

                    //Fill(DataSet) - Dodaje lub odświeża wiersze w obiekcie DataSet , aby dopasować je do tych w źródle danych.
                    sqlDataAdapter.Fill(MagazynDataTable);
                    //Zmiana tekstu w textbox
                    textBox1.Text = MagazynDataTable.Rows[0]["Lokalizacja"].ToString();
                   //Zmiana tekstu w LABEL
                    LabelLokalizacja.Content = "PRODUKTY W LOKALIZACJI " + MagazynDataTable.Rows[0]["Lokalizacja"].ToString();
                  
                    



                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        // AKTUALIZACJA TEXTU W TEXT_BOXIE 2
        private void ShowSelectedinTextBox2()
        {
            try
            {
                // utworzenie zaptania i połączenie do bazy
                string query = "select Nazwa from Produkt where Id = @ProduktId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ProduktId", listaAllProdukt.SelectedValue);
                    //Tworzenie Tabeli
                    DataTable ProduktDataTable = new DataTable();

                    //Fill(DataSet) - Dodaje lub odświeża wiersze w obiekcie DataSet , aby dopasować je do tych w źródle danych.
                    sqlDataAdapter.Fill(ProduktDataTable);

                    textBox2.Text = ProduktDataTable.Rows[0]["Nazwa"].ToString();


                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        

        private void listaAllProdukt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // AKTUALIZACJA TEXTU W TEXT_BOXIE 2
            ShowSelectedinTextBox2();
        }

        // Przycick_AktualizacjaLokalizacji
        private void Button_Click_AktualizacjaLokalizacji(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Magazyn Set Lokalizacja = @Lokalizacja where Id = @MagazynId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@MagazynId", listaLokalizacji.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Lokalizacja", textBox1.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Zaznacz LOKALIZACJĘ aby dokonać AKTUALIZACJI");
            }
            finally
            {
                sqlConnection.Close();
                ShowMagazyn();

            }
        }


        // Przycick_Aktualizacja PRODUKTÓW
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Produkt Set Nazwa = @Nazwa where Id = @ProduktId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProduktId", listaAllProdukt.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Nazwa", textBox2.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Zaznacz PRODUKT aby dokonać AKTUALIZACJI");
            }
            finally
            {
                sqlConnection.Close();
                ShowAllProdukt();

            }
        }
    }
}
