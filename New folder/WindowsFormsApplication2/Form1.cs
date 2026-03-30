using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        // Define the dictionary with key-value pairs
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        string agtcomp = "Rdy,8tmoma-pituh2,68306,mor_nisim,0,Rdy,YF-TCCM-Dev,68311,mor_nisim_Women,0";
        string agentsAll = "mor_nisim,0024,mor_nisim_Kids,0324,mor_nisim_Nurse,0124,mor_nisim_Women,0224";
        string comp = "Rdy,8tmoma-pituh2,68306,mor_nisim,0";
        int sw = 0;

        public Form1()
        {
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            InitializeComponent();
            
            string[] agents = agentsAll.Split(',');
            for ( int i=0; i<agents.Length; i+=2)
                keyValuePairs.Add(agents[i], agents[i+1]);
            // Bind the dictionary to the ComboBox
            comboBox1.DataSource = new BindingSource(keyValuePairs, null);
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";
            string[] comps = comp.Split(',');
            textBox2.Text = comps[1];
            textBox3.Text = comps[2];


        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected key and value and display them in a label
            
            string selectedKey = comboBox1.SelectedIndex.ToString();
            string selectedValue = comboBox1.SelectedValue.ToString();
            string[] tValue = { "1", "1" };
            if (sw == 0)
            {
                selectedValue = selectedValue.Substring(1, selectedValue.Length - 2);
                tValue = selectedValue.Split(',');   //.ToString();
                textBox1.Text = tValue[1];
                sw = 1;
            }
            else
            {
                selectedValue = comboBox1.SelectedValue.ToString();
                //selectedValue.Substring(1, selectedValue.Length - 2);
                tValue = selectedValue.Split(',');   //.ToString();
                textBox1.Text = selectedValue;
            }

            label1.Text = "Selected Key: " + selectedKey + ", Selected Value: " + selectedValue;
           

            string[] agtcomps = agtcomp.Split(',');
            string agtcomp1 = "";
            for (int i = 3; i < agtcomps.Length; i += 4)
            {
                if (tValue[0].Equals(agtcomps[i]))
                {
                    agtcomp1 = agtcomps[i];
                    break;
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            DialogResult dr = MessageBox.Show("Form1_FormClosed",
                      "Mood Test", MessageBoxButtons.YesNo);
            Program.WriteCharacters();
            Thread.Sleep(2000);
            switch (dr)
            {
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.WriteCharacters();
            MessageBox.Show("Unhandled exception handler fired. Object: " +
                     e.CloseReason);
            Program.WriteCharacters();
            // Prepare cooperative async shutdown from another thread
            Thread t = new Thread(delegate ()
            {
                MessageBox.Show("Asynchronous shutdown started");
                Environment.Exit(1);
            });

            t.Start();
            t.Join(); // wait until we have exited
            e.Cancel = true;
            DialogResult dr = MessageBox.Show("Form1_FormClosing",
                      "Mood Test", MessageBoxButtons.YesNo);
            Thread.Sleep(2000);
            switch (dr)
            {
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
            }
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            // When the application is exiting, write the application data to the
            // user file and close it.
            DialogResult dr = MessageBox.Show("OnApplicationExit",
                      "Mood Test", MessageBoxButtons.YesNo);
            Program.WriteCharacters();
            Thread.Sleep(2000);
            switch (dr)
            {
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
            }

            try
            {
                // Ignore any errors that might occur while closing the file handle.
                DialogResult dr1 = MessageBox.Show("OnApplicationExit1",
                      "Mood Test1", MessageBoxButtons.YesNo);
                Thread.Sleep(2000);
                switch (dr)
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            catch { }
        }
    }
}




   

 
