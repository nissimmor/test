using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Application.Run(new Form1());
            // Handle the ApplicationExit event to know when the application is exiting.
            //Application.ApplicationExit += new EventHandler(Form1.OnApplicationExit);

        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {

            // When the application is exiting, write the application data to the
            // user file and close it.
            DialogResult dr = MessageBox.Show("OnApplicationExit",
                      "Mood Test", MessageBoxButtons.YesNo);
            WriteCharacters();
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
                WriteCharacters();
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
            throw new NotImplementedException();
        }
        public static async void WriteCharacters()
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            char[] charsToAdd = ue.GetChars(ue.GetBytes("First line and second line"));
            using (StreamWriter writer = File.CreateText("newfile.txt"))
            {
                await writer.WriteLineAsync(charsToAdd, 0, 11);
                await writer.WriteLineAsync(charsToAdd, 11, charsToAdd.Length - 11);
            }
        }
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
          

            // When the application is exiting, write the application data to the
            // user file and close it.
            DialogResult dr = MessageBox.Show("OnApplicationExit",
                      "Mood Test", MessageBoxButtons.YesNo);
            WriteCharacters();
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
                WriteCharacters();
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
            throw new NotImplementedException();
        }

    

    }
}
