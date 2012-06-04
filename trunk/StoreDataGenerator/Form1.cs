using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO;

namespace StoreDataGenerator
{
    public partial class Form1 : Form
    {
        List<CultureInfo> cultures = new List<CultureInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboLocale.DataSource = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".dat";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(MarketplaceConfig.Store));
                Stream file = saveFile.OpenFile();

                MarketplaceConfig.Store store = new MarketplaceConfig.Store(int.Parse(textVersion.Text), textDisplay.Text,
                    textName.Text, textID.Text, checkEnabled.Checked, new Uri(textImage.Text, UriKind.RelativeOrAbsolute), 
                    (CultureInfo)comboLocale.SelectedItem);

                serializer.WriteObject(file, store);
                file.Close();
            }
        }
    }
}
