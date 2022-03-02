using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using S7.Net;

namespace SiemensTest
{

    public partial class MainWindow : Window
    {
        #region Fields

        Plc _S71200 = null;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataTypeComboBox.ItemsSource = Enum.GetValues(typeof(DataType)).Cast<DataType>();
            this.dataTypeComboBox.SelectedIndex = 0;
            this.varTypeComboBox.ItemsSource = Enum.GetValues(typeof(VarType)).Cast<VarType>();
            this.varTypeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// When the user decide to write to the PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_S71200 != null && _S71200.IsConnected)
                {
                    string nodo = this.nodeTextBox.Text;
                    string content = this.valueTextBlock.Text;

                    _S71200.Write(nodo, content);
                    this.outputTexBlock.Text = string.Concat("All'indirizzo: ", _S71200.IP, " al nodo: ", nodo, " è stato inviato: ", content);
                    this.outputTexBlock.Foreground = Brushes.White;
                }
                else
                {
                    this.outputTexBlock.Text = "Not connected to PLC";
                    this.outputTexBlock.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                this.outputTexBlock.Text = ex.Message;
                this.outputTexBlock.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// When the user decide to read datas from the PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_S71200 != null && _S71200.IsConnected)
                {
                    DataType dataType = (DataType)this.dataTypeComboBox.SelectedItem;
                    VarType varType = (VarType)this.varTypeComboBox.SelectedItem;

                    if (int.TryParse(this.dbTextBox.Text, out int db) && int.TryParse(startByteTextBox.Text, out int startByte) && int.TryParse(this.varCountTextBox.Text, out int varCount))
                    {
                        var valore = _S71200.Read(dataType, db, startByte, varType, varCount);
                        this.outputTexBlock.Text = string.Concat("All'indirizzo: ", _S71200.IP, " è stato letto: ", valore.ToString());
                        this.outputTexBlock.Foreground = Brushes.White;
                    }
                    else
                    {
                        this.outputTexBlock.Text = "Invalid cast";
                        this.outputTexBlock.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    this.outputTexBlock.Text = "Not connected to PLC";
                    this.outputTexBlock.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                this.outputTexBlock.Text = ex.Message;
                this.outputTexBlock.Foreground = Brushes.Red;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.dataTypeTextBlock.Visibility = Visibility.Hidden;
            this.dataTypeComboBox.Visibility = Visibility.Hidden;
            this.dbeTextBlock.Visibility = Visibility.Hidden;
            this.dbTextBox.Visibility = Visibility.Hidden;
            this.startByteTextBlock.Visibility = Visibility.Hidden;
            this.startByteTextBox.Visibility = Visibility.Hidden;
            this.varTypeTextBlock.Visibility = Visibility.Hidden;
            this.varTypeComboBox.Visibility = Visibility.Hidden;
            this.varCountTextBlock.Visibility = Visibility.Hidden;
            this.varCountTextBox.Visibility = Visibility.Hidden;
            this.readButton.Visibility = Visibility.Hidden;

            this.nodeTextBlock.Visibility = Visibility.Visible;
            this.nodeTextBox.Visibility = Visibility.Visible;
            this.valueTextBlock.Visibility = Visibility.Visible;
            this.valueTextBox.Visibility = Visibility.Visible;
            this.writeButton.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.dataTypeTextBlock.Visibility = Visibility.Visible;
            this.dataTypeComboBox.Visibility = Visibility.Visible;
            this.dbeTextBlock.Visibility = Visibility.Visible;
            this.dbTextBox.Visibility = Visibility.Visible;
            this.startByteTextBlock.Visibility = Visibility.Visible;
            this.startByteTextBox.Visibility = Visibility.Visible;
            this.varTypeTextBlock.Visibility = Visibility.Visible;
            this.varTypeComboBox.Visibility = Visibility.Visible;
            this.varCountTextBlock.Visibility = Visibility.Visible;
            this.varCountTextBox.Visibility = Visibility.Visible;
            this.readButton.Visibility = Visibility.Visible;

            this.nodeTextBlock.Visibility = Visibility.Hidden;
            this.nodeTextBox.Visibility = Visibility.Hidden;
            this.valueTextBlock.Visibility = Visibility.Hidden;
            this.valueTextBox.Visibility = Visibility.Hidden;
            this.writeButton.Visibility = Visibility.Hidden;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _S71200 = this.PreparePlc();

                if (_S71200.IsConnected)
                {
                    _S71200.Close();
                    this.statusButton.Fill = Brushes.Red;
                    this.connectButton.Content = "Connect";
                }
                else
                {
                    _S71200.Open();
                    this.statusButton.Fill = Brushes.Green;
                    this.connectButton.Content = "Disconnect";
                }
            }
            catch (Exception ex)
            {
                this.outputTexBlock.Text = ex.Message;
                this.outputTexBlock.Foreground = Brushes.Red;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create the PLC
        /// </summary>
        /// <returns></returns>
        private Plc PreparePlc()
        {
            if (short.TryParse(this.rackTextBox.Text, out short rack) && short.TryParse(this.slotTextBox.Text, out short slot))
            {
                string ip = this.ipTextBox.Text;
                CpuType cpuType;

                if ((bool)this.D.IsChecked)
                {
                    cpuType = CpuType.S7200;
                }
                else if ((bool)this.T.IsChecked)
                {
                    cpuType = CpuType.S7300;
                }
                else if ((bool)this.Q.IsChecked)
                {
                    cpuType = CpuType.S7400;
                }
                else if ((bool)this.MC.IsChecked)
                {
                    cpuType = CpuType.S71500;
                }
                else
                {
                    cpuType = CpuType.S71200;
                }

                return new Plc(cpuType, ip, rack, slot);
            }
            else
            {
                throw new ArgumentException("Invalid rack or slot, must to be short type.");
            }
        }

        #endregion
    }
}
