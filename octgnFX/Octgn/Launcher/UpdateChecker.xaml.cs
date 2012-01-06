﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Octgn.Launcher
{
    /// <summary>
    /// Interaction logic for UpdateChecker.xaml
    /// </summary>
    public partial class UpdateChecker : Window
    {
        public bool IsClosingDown { get; set; }
        public UpdateChecker()
        {
            IsClosingDown = false;
            InitializeComponent();
            Thread t = new Thread(CheckForUpdates);
            t.Start();
        }
        private void CheckForUpdates()
        {
            bool isupdate = false;
            string ustring = "";
            if (FileExists("http://www.skylabsonline.com/downloads/octgn/update.xml"))
            {
                string[] update = new string[2];
                update = ReadUpdateXML("http://www.skylabsonline.com/downloads/octgn/update.xml");

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Version local = assembly.GetName().Version;
                Version online = new Version(update[0]);
                isupdate = online > local;
                ustring = update[1];
            }
            Dispatcher.BeginInvoke(new Action<bool,string>(UpdateCheckDone), isupdate, ustring);
        }
        private void UpdateCheckDone(bool result,string url)
        {
            if (result)
            {
                IsClosingDown = true;
                switch (
                    MessageBox.Show("An update is available. Would you like to download now?", "Update Available",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        System.Diagnostics.Process.Start(url);
                        break;
                }
            }
            this.Close();
        }
        private bool FileExists(string URL)
        {
            bool result = false;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    System.IO.Stream str = client.OpenRead(URL);
                    if (str != null) result = true; else result = false;
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }
        private string[] ReadUpdateXML(string URL)
        {
            string[] values = new string[2];

            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(System.Net.WebRequest.Create(URL).GetResponse().GetResponseStream()))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (!reader.IsEmptyElement)
                        {
                            switch (reader.Name)
                            {
                                case "Version":
                                    values = new string[2];
                                    if (reader.Read()) { values[0] = reader.Value; }
                                    break;
                                case "Location":
                                    if (reader.Read()) { values[1] = reader.Value; }
                                    break;
                            }
                        }
                    }
                }
            }
            return values;
        }
    }
}
