﻿using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using WarehouseManagement.Controller;
using WarehouseManagement.Database;
using WarehouseManagement.Helpers;
using WarehouseManagement.Models;
using WarehouseManagement.Views.Main;
using WWarehouseManagement.Database;
using static WarehouseManagement.Models.Address;

namespace WarehouseManagement.Views.Onboarding
{
    /// <summary>
    /// Interaction logic for OnboardingSetup.xaml
    /// </summary>
    public partial class OnboardingSetup : Window
    {
        BackgroundWorker workerImportAddress;

        sql_control sql = new sql_control();

        public DataTable JNTAddress { get; set; }
        public DataTable FlashAddress { get; set; }
        void CheckedRadio(string text)
        {
            if (text == "J&T")
            {
                ContainerFlash.Visibility = Visibility.Collapsed;
                ContainerJnt.Visibility = Visibility.Visible;
                cmbProvinceFlash.Text = "";

            }
            if (text == "FLASH")
            {
                ContainerFlash.Visibility = Visibility.Visible;
                ContainerJnt.Visibility = Visibility.Collapsed;
                cmbProvinceJnt.Text = "";
            }
        }
        private void radioCustomer(string text)
        {
            if (text == "J&T")
            {
                txtCustomerID.Visibility = Visibility.Visible;

                tbAccountName.Visibility = Visibility.Collapsed;
                tbName.Visibility = Visibility.Collapsed;
                tbMobile.Visibility = Visibility.Collapsed;
                tbEmail.Visibility = Visibility.Collapsed;

            }
            if (text == "FLASH")
            {
                txtCustomerID.Visibility = Visibility.Collapsed;

                tbAccountName.Visibility = Visibility.Visible;
                tbName.Visibility = Visibility.Visible;
                tbMobile.Visibility = Visibility.Visible;
                tbEmail.Visibility = Visibility.Visible;
            }
        }
        public OnboardingSetup()
        {
            InitializeComponent();
            load_couriers();
            txtFileNameProduct.Text = "Addressing_guide_with_can_do_delivery.csv";  //JNT ADDRESS
            txtAddressFlash.Text = "Flash_Addressing_Guide.csv";  //Flash ADDRESS

            JNTAddress = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
            FlashAddress = Csv_Controller.GetDataTableFromCSVFile(txtAddressFlash.Text);

            int numberofitems = JNTAddress.Rows.Count + FlashAddress.Rows.Count;
            pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
            lblTotalNumberOfItems.Text = numberofitems.ToString();

            Csv_Controller.dataTableJntAddress = Csv_Controller.GetDataTableFromCSVFile(txtFileNameProduct.Text);
            Csv_Controller.dataTableFlashAddress = Csv_Controller.GetDataTableFromCSVFile(txtAddressFlash.Text);

            rdbJandT.IsChecked = true;
            rdbJandTCustomer.IsChecked = true;

        }
        db_queries queries = new db_queries();
        private void load_couriers()
        {
            List<String> couriers = new List<String>();
            couriers.Add("JNT");
            couriers.Add("Flash");
            //cmbCourier.ItemsSource = couriers;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnImportAddress.IsEnabled = false;
            workerImportAddress = new BackgroundWorker();
            workerImportAddress.WorkerReportsProgress = true;

            workerImportAddress.DoWork += WorkerImportRegion_DoWork;
            workerImportAddress.RunWorkerCompleted += WorkerImportRegion_RunWorkerCompleted;

            workerImportAddress.RunWorkerAsync();
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
        
        }
        private void cmbProvince_DropDownClosed(object sender, EventArgs e)
        {
            cmbCityJnt.SelectedIndex = -1;
            cmbBarangayJnt.ItemsSource = null;

            queries.city(cmbCityJnt, cmbProvinceJnt.Text);
        }

        private void cmbCity_DropDownClosed(object sender, EventArgs e)
        {
            cmbBarangayJnt.SelectedIndex = -1;
            cmbBarangayJnt.ItemsSource = null;

            queries.baranggay(cmbBarangayJnt, cmbCityJnt.Text);
        }
        #region FLASH ADDRESSES
        private void cmbProvinceFlash_DropDownClosed(object sender, EventArgs e)
        {
            cmbCityFlash.SelectedIndex = -1;
            queries.FlashCity(cmbCityFlash, cmbProvinceFlash.Text);
        }
        private void cmbProvinceFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCityFlash.SelectedIndex = -1;
            cmbBarangayFlash.ItemsSource = null;
        }
        private void cmbCityFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashBaranggay(cmbBarangayFlash, cmbCityFlash.Text);
        }
        private void cmbCityFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbBarangayFlash.SelectedIndex = -1;
        }

        private void cmbBarangayFlash_DropDownClosed(object sender, EventArgs e)
        {
            queries.FlashPostalCode(cmbPostalCodeFlash, cmbBarangayFlash.Text);
        }

        private void cmbBarangayFlash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbPostalCodeFlash.SelectedIndex = -1;
        }
        #endregion
        private void WorkerImportRegion_DoWork(object sender, DoWorkEventArgs e)
        {
            //Csv_Controller.ImportAddress(lblImportedProducts, pbBarProduct);
        }
        private void WorkerImportRegion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnImportAddress.IsEnabled = true;
            Csv_Controller.ConfirmedToImport = true;
            queries.province(cmbProvinceJnt);
            queries.FlashProvince(cmbProvinceFlash);
        }
        private void btnBrowseAddress_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFileNameProduct.Text = openFileDialog.FileName;
                Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
                int numberofitems = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName).Rows.Count;
                pbBarProduct.Maximum = numberofitems > 0 ? numberofitems : 100;
                lblTotalNumberOfItems.Text = numberofitems.ToString();
                Csv_Controller.dataTablebulkOrder = Csv_Controller.GetDataTableFromCSVFile(openFileDialog.FileName);
            }
        }

        private void btnImportAddress_Click(object sender, RoutedEventArgs e)
        {
            btnImportAddress.IsEnabled = false;
            workerImportAddress = new BackgroundWorker();
            workerImportAddress.WorkerReportsProgress = true;

            workerImportAddress.DoWork += WorkerImportRegion_DoWork;
            workerImportAddress.RunWorkerCompleted += WorkerImportRegion_RunWorkerCompleted;

            workerImportAddress.RunWorkerAsync();
        }

        private void txtAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtAddress_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            // Check if the entered text contains symbols
            if (HasSymbols(e.Text))
            {
                e.Handled = true; // Ignore the input
            }
        }
        private bool HasSymbols(string text)
        {
            foreach (char c in text)
            {
                // Check if the character is not alphanumeric, space, ",", ".", or "-"
                if (!char.IsLetterOrDigit(c) && c != ' ' && c != ',' && c != '.' && c != '-')
                {
                    return true;
                }
            }
            return false;
        }
        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
        private void rdbJandT_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbJandT.Content.ToString());
            HintAssist.SetHint(txtCustomerID, "J&T VIP CODE");
        }

        private void rdbFlash_Checked(object sender, RoutedEventArgs e)
        {
            CheckedRadio(rdbFlash.Content.ToString());
            HintAssist.SetHint(txtCustomerID, "FLASH KA CODE");
        }

        private void btnSaveShop_Click(object sender, RoutedEventArgs e)
        {
            int shopChecker = int.Parse(sql.ReturnResult($"SELECT COUNT(*) FROM tbl_sender WHERE sender_name = '{txtPagename.Text}'"));
            if(shopChecker > 0)
            {
                MessageBox.Show("Shop name already exists.");
            }
            else
            {
                if (rdbJandT.IsChecked == true)
                {
                    queries.insert_sender("0", txtPagename, txtPhone, cmbProvinceJnt, cmbCityJnt, cmbBarangayJnt, txtAddress, "", 1);
                    MessageBox.Show("Shop Added");
                    txtPagename.Text = "";
                    txtPhone.Text = "";
                    txtAddress.Text = "";
                    cmbProvinceJnt.Text = "";
                    cmbCityJnt.Text = "";
                    cmbBarangayJnt.Text = "";

                }
                else
                {
                    //FLASH
                    queries.insert_sender("0", txtPagename, txtPhone, cmbProvinceFlash, cmbCityFlash, cmbBarangayFlash, txtAddress, cmbPostalCodeFlash.Text, 2);
                    MessageBox.Show("Shop Added");
                    txtPagename.Text = "";
                    txtPhone.Text = "";
                    txtAddress.Text = "";
                    cmbProvinceFlash.Text = "";
                }
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Util.IsAnyTextBoxEmpty(txtCustomerID))
            {
                MessageBox.Show("Please complete all required fields on customer information.");
                return;
            }
            else if(queries.checkExistingVIPCode(txtCustomerID.Text))
            {
                MessageBox.Show("VIP code Exist.");
                return;
            }
            else if(queries.VIPLimit())
            {
                MessageBox.Show("VIPs limit reached.");
                return;
            }
            else
            {
                if (rdbFlashCustomer.IsChecked == true)
                {
                    //queries.api_credentials(rdbFlashCustomer, "fc8250f522c23b8d93a286519494c764a828afdd0c464797ecbb9276aa275629", "", txtCustomerID);
                    //MessageBox.Show("FLASH vip added"); 
                    FlashAccountDetails details = new FlashAccountDetails()
                    {
                        AccountName = tbAccountName.Text,
                        Fullname = tbAccountName.Text,
                        Mobile = tbMobile.Text,
                        Email = tbEmail.Text
                    };

                    GlobalModel.customer_id = "BA0074";
                    GlobalModel.key = "PwxNQHBeBUdLQhdbXAxzAUBqDkdKc1tSS01JQApYWH8EQWtXFBQhClMRTUZAXlZZLgYbO1ZHRXMPAkBORUJbVg==";
                    await FLASH_api.FlashCreateSubaccount(details, rdbFlashCustomer.Content.ToString());

                    tbAccountName.Clear();
                    tbMobile.Clear();
                    tbEmail.Clear();
                    tbName.Clear();
                }
                else
                {
                    //J&T
                    queries.api_credentials(rdbJandTCustomer.Content.ToString(), "03bf07bf1b172b13efb6259f44190ff3", "THIRDYNAL", txtCustomerID.Text);
                    MessageBox.Show("J&T vip added");
                    txtCustomerID.Text = "";
                }
            }
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            if(queries.checkCredentials())
            {
                MessageBox.Show("Information Setup completed");
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("SHOP or VIPs are detected empty. Kindly set-up your SHOP or VIP");
                return;
            }
        }

        private void txtId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemQuotes)
            {
                // Suppress the key event to prevent the character from being entered
                e.Handled = true;
            }
        }

        private void txtPagename_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemQuotes)
            {
                // Suppress the key event to prevent the character from being entered
                e.Handled = true;
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemQuotes)
            {
                // Suppress the key event to prevent the character from being entered
                e.Handled = true;
            }
        }

        private void rdbJandTCustomer_Checked(object sender, RoutedEventArgs e)
        {
            radioCustomer(rdbJandTCustomer.Content.ToString());
        }

        private void rdbFlashCustomer_Checked(object sender, RoutedEventArgs e)
        {
            txtCustomerID.Text = "1";
            radioCustomer(rdbFlashCustomer.Content.ToString());
        }

        private void tbMobile_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputValidation.Integer(sender, e);
        }
    }
}
