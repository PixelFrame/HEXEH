using HEXEH.Core;
using HEXEH.Core.DataType;
using HEXEH.Core.Utility;
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

namespace HEXEH.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Converter _converter = new();
        private static Guid _selectedType = Guid.Empty;
        private static Dictionary<string, object>? _settingMap = null;

        public MainWindow()
        {
            InitializeComponent();
            cbbDataType.ItemsSource = _converter.DataTypeNames;
            cbbDataType.DisplayMemberPath = "Value";
            cbbDataType.SelectedValuePath = "Key";
            cbbDataType.SelectedIndex = 0;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var rawInput = tbInput.Text;
            var bytesInput = Common.StringToBytes(rawInput);
            if(bytesInput.Length == 0)
            {
                MessageBox.Show("Invalid HEX input!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            var hdInput = HexDump.ConvertFromBytes(bytesInput, new Dictionary<string, object>() { { "LineLength", (uint)16 } });
            tbkHexDumpLeft.Text = hdInput.LineNum;
            tbHexDumpMid.Text = hdInput.FormattedHex;
            tbHexDumpRight.Text = hdInput.FormattedChars;
            DataConversion(bytesInput);
        }

        private void cbbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stkPanelSubSettings.Children.Clear();
            if (cbbDataType.SelectedValue == null || cbbDataType.SelectedValuePath != "Key") return;
            _selectedType = (Guid)cbbDataType.SelectedValue;

            var settingMap = _converter.GetSettingMap(_selectedType);
            if (settingMap == null) return;

            foreach (var setting in settingMap)
            {
                var settingName = setting.Key.Split('#');
                var tbkSubSettingName = new Label();
                tbkSubSettingName.Content = settingName[0];
                stkPanelSubSettings.Children.Add(tbkSubSettingName);
                switch (settingName[1])
                {
                    case ("single"):
                        {
                            foreach (var option in setting.Value)
                            {
                                var rbSubsettingOption = new RadioButton();
                                rbSubsettingOption.Content = option;
                                rbSubsettingOption.GroupName = settingName[0];
                                stkPanelSubSettings.Children.Add(rbSubsettingOption);
                            }
                            break;
                        }
                    case ("multi"):
                        {
                            foreach (var option in setting.Value)
                            {
                                var cbxSubsettingOption = new CheckBox();
                                cbxSubsettingOption.Content = option;
                                stkPanelSubSettings.Children.Add(cbxSubsettingOption);
                            }
                            break;
                        }
                    case ("num"): 
                        {
                            var tbSubSettingNumInput = new TextBox();
                            tbSubSettingNumInput.AcceptsReturn = false;
                            stkPanelSubSettings.Children.Add(tbSubSettingNumInput);
                            break;
                        }
                    case ("string"):
                        {
                            var tbSubSettingNumInput = new TextBox();
                            tbSubSettingNumInput.AcceptsReturn = false;
                            stkPanelSubSettings.Children.Add(tbSubSettingNumInput);
                            break;
                        }
                }
            }
        }

        private void DataConversion(byte[] blob)
        {
            tvDataTree.Items.Clear();
            if (_selectedType == new Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")) return;
            tvDataTree.Items.Add(_converter.DoConversion(_selectedType, blob, _settingMap).Head);
        }
    }
}
