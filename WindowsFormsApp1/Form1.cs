using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lstLocal.View = View.Details;
            lstLocal.Clear();
            lstLocal.GridLines = true;
            lstLocal.FullRowSelect = true;
            lstLocal.BackColor = System.Drawing.Color.Aquamarine;
            lstLocal.Columns.Add("IP", 100);
            lstLocal.Columns.Add("HostName", 200);
            lstLocal.Columns.Add("MAC Address", 300);

            Ping_all();
        }

        //private string NetworkGateway()
        //{
        //    string ip = null;
        //    foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
        //    {
        //        if (f.OperationalStatus == OperationalStatus.Up)
        //        {
        //            foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
        //            {
        //                ip = d.Address.ToString();
        //            }
        //        }
        //    }
        //    return ip;
        //}

        public void Ping_all()
        {
            //string gate_ip = NetworkGateway();
            //string[] array = gate_ip.Split('.');
            
            for(int i= 2; i<=255; i++)
            {
                string ping_var = "192.168.137." + i;
                Ping(ping_var, 4, 4000);
            }
        }

        public void Ping(string host, int attempts, int timeout)
        {
            for(int i = 0; i < attempts; i++)
            {
                new Thread(delegate ()
                {
                    try
                    {
                        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                        ping.PingCompleted += new PingCompletedEventHandler(PingCompleated);
                        ping.SendAsync(host, timeout, host);
                    }
                    catch
                    {

                    }
                }).Start();
            }
        }

        private void PingCompleated(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if(e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string hostname = GetHostName(ip);
                string macaddress = GetMacAddress(ip);
                string[] arr = new string[3];

                arr[0] = ip;
                arr[1] = hostname;
                arr[2] = macaddress;

                ListViewItem item;
                if(this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                        {
                            item = new ListViewItem(arr);
                            lstLocal.Items.Add(item);
                        }));
                }
                else
                {
                    MessageBox.Show(e.Reply.Status.ToString());
                }
            }
        }

        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if(entry != null)
                {
                    
                    return entry.HostName;
                }
            }catch(SocketException)
            {

            }
            return null;
        }
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process Process = new System.Diagnostics.Process();
            Process.StartInfo.FileName = "arp";
            Process.StartInfo.Arguments = "-a " + ipAddress;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.Start();
            string strOutput = Process.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if(substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2)) + "-" + substrings[4] + "-" + substrings[5] +
                    "-" + substrings[6] + "-" + substrings[7] + "-" + substrings[8].Substring(0, 2);
                return macAddress;
            }
            else
            {
                return "OwnedForms Machin";
            }
        }

        private void lstLocal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
