using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public partial class Form2 : Form
    {
        bool ip = false;
        bool connecting = false;
        public Socket client;
        
        Thread connection;
        
        public Form2()
        {
            
            InitializeComponent();
        }
        //Placeholder
        private void textBox_MouseEnter(object sender, EventArgs e)
        {
            var a = (sender as TextBox);
            if (a.Text == "Username" || a.Text == "Ip address")
            {
                a.Text = "";
            }

        }
        //Placeholder
        private void textBox_MouseLeave(object sender, EventArgs e)
        {
            var a = (sender as TextBox);
            if (a.Text == "")
            {
                if (a.Name == "name")
                {
                    a.Text = "Username";
                }
                else if (a.Name == "ipaddress")
                {
                    a.Text = "Ip address";
                }
            }

            

        }
        
        private void ipaddress_TextChanged(object sender, EventArgs e)
        {
            var a = (sender as TextBox);

            if (Regex.IsMatch(a.Text, @"\d{1,3}[.]\d{1,3}[.]\d{1,3}[.]\d{1,3}[:]\d{1,4}"))
            {
                ip = true;
            }
            unlockBtn(null,EventArgs.Empty);
        }
        
        private void unlockBtn(object sender, EventArgs e)
        {
            if (ip && name.Text != "") { connect.Enabled = true;}
        }

        private void connect_Click(object sender, EventArgs e)
        {
            
            if (connecting || name.Text == "Username")
                return;

            new Thread(() =>
           {
               DateTime start = DateTime.Now;
               TimeSpan ts = DateTime.Now - start;
               while (Math.Round(ts.TotalSeconds) <= 10)
               {
                   ts = DateTime.Now - start;
                   Thread.Sleep(1000);
               }
               if (connection.IsAlive)
               {
                   connection.Abort();
                   Invoke((MethodInvoker)delegate ()
                   {
                       this.status.Text = "No response";
                   });
               }
               connecting = false;
           }).Start();
            
            connection= new Thread(() => {
                connecting= true;
                client = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                string[] addr = ipaddress.Text.Split(':');
                string ip = addr[0];
                int port = int.Parse(addr[1]);
                
                while (!client.Connected)
                {

                    try
                    {
                        Invoke((MethodInvoker)delegate () {
                            this.status.Text = "Connecting...";
                        });
                        client.Connect(ip,port);
                       
                        

                    }
                    catch (Exception)
                    {
                        
                    }
                }
               
                    Invoke((MethodInvoker)delegate () {
                        this.Hide();
                        Form1 f1 = new Form1();
                        f1.client = client;
                        f1.name = name.Text;
                        f1.Show();
                    });
                    
                
                
                
            });

            connection.Start();
            
            
        
        }
        private void disconnect(object sender, EventArgs e)
        {
            if(connection != null)
            {
                connection.Abort();
            }
            Application.Exit();
        }

       
    }
}
