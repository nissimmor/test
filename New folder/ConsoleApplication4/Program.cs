using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyValComboBoxExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Define the dictionary with key-value pairs
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("keya", "valuea");
            keyValuePairs.Add("keyb", "valueb");
            keyValuePairs.Add("keyc", "valuec");
            keyValuePairs.Add("keyd", "valued");

            // Bind the dictionary to the ComboBox
            comboBox1.DataSource = new BindingSource(keyValuePairs, null);
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected key and value and display them in a label
            string selectedKey = comboBox1.SelectedValue.ToString();
            string selectedValue = comboBox1.SelectedItem.ToString();
            label1.Text = "Selected Key: " + selectedKey + ", Selected Value: " + selectedValue;
        }
    }
}
