using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AspnetProfilerHelper.WindowsApp
{
    public partial class Form1 : Form
    {
        public BindingList<ProfileData> profileData = new BindingList<ProfileData>();
        private bool blockParse = false;

        public Form1()
        {
            InitializeComponent();
            dataGridViewData.DataSource = profileData;
        }

        private void textBoxPropertyNames_TextChanged(object sender, EventArgs e)
        {
            ParseEncodedData();
        }

        private void textBoxPropertyValues_TextChanged(object sender, EventArgs e)
        {
            ParseEncodedData();
        }

        private void ParseEncodedData()
        {
            if (blockParse)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(textBoxPropertyNames.Text))
                    return;

                if (string.IsNullOrWhiteSpace(textBoxPropertyValues.Text))
                    return;

                profileData.Clear();
                var splitNames = textBoxPropertyNames.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splitNames.Count(); i += 4)
                {
                    ProfileData data = new ProfileData();
                    data.Name = splitNames[i];
                    var position = int.Parse(splitNames[i + 2]);
                    var length = int.Parse(splitNames[i + 3]);

                    //A value of -1 signifies no value. 
                    if (length < 0)
                    {
                        data.Value = string.Empty;
                        data.StringValue = false;
                    }
                    else
                    {
                        data.Value = textBoxPropertyValues.Text.Substring(position, length);
                        data.StringValue = true;
                    }
                    profileData.Add(data);
                }

                labelStatus.Text = "Everything's fine";
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error parsing encoded data";
            }
       }

        private void dataGridViewData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            EncodeDataGridData();
        }

        private void EncodeDataGridData()
        {
            try
            {
                blockParse = true;
                string names = string.Empty;
                string values = string.Empty;

                var currentPosition = 0;

                foreach (var item in profileData)
                {
                    if (item.StringValue)
                    {
                        names += string.Format("{0}:S:{1}:{2}:", item.Name, currentPosition, item.Value.Length);
                    }
                    else
                    {
                        names += string.Format("{0}:B:{1}:{2}:", item.Name, 0, -1);
                    }
                    values += item.Value;
                    currentPosition += item.Value.Length;
                }

                textBoxPropertyNames.Text = names;
                textBoxPropertyValues.Text = values;
            }
            finally
            {
                blockParse = false;
            }
        }

    }

    public class ProfileData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool StringValue { get; set; }
    }
}
