using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Socket client;
        public string name = "";
        Thread timeUpdate;
        public byte[] buffer = new byte[4096];
        public Form1()
        {
            InitializeComponent();
        }
        //Placeholder
        private void textBox_MouseEnter(object sender, EventArgs e)
        {
           
            var a = (sender as TextBox);
            if (a.Text == "Enter message")
            {
                a.Text = "";
            }

        }

        //Placeholder
        private void textBox_MouseLeave(object sender, EventArgs e)
        {
            var a = (sender as TextBox);
            if(a.Text == "")
            {
                a.Text = "Enter message";
                
            }

        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            timeUpdate = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Invoke((MethodInvoker)delegate ()
                        {
                            this.time.Text = "Time: " +DateTime.Now.ToLongTimeString();
                        });
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });
            timeUpdate.Start();
            
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack),client);
            SendText("!ready", client);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timeUpdate.Abort();

            Application.Exit();
        }

        public void SendText(string text, Socket s)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            if (data.Length > 2048)
            {
                inputBox.Text = "";
                MessageBox.Show("Message is too long");
                return;
            }
            s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), s);

        }
        public void SendCallBack(IAsyncResult result)
        {
            Socket s = (Socket)result.AsyncState;
            s.EndSend(result);

        }
        public  void ReceiveCallBack(IAsyncResult result)
        {
            Socket s = (Socket)result.AsyncState;
            int dataLength = 0;
            try
            {
                dataLength = s.EndReceive(result);
            }
            catch (Exception)
            {
                
                return;

            }
            byte[] temp = new byte[dataLength];
            Array.Copy(buffer, temp, dataLength);
            string text = Encoding.ASCII.GetString(temp);
            if (text.Contains("!users"))
            {
                Invoke((MethodInvoker)delegate { label3.Text = "Active users: " + text.Split(' ')[1]; });
            }
            else
            {
                string[] message = text.Split('&');
                Invoke((MethodInvoker)delegate { this.textBox1.Text += string.Format("[{0}]: {1}\r\n", message[0], message[1]); });
            }

            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);


        }
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && this.inputBox.Text !="")
            {
                
                string message = this.inputBox.Text.Replace('&',' ');
                
                SendText(string.Format("{0}&{1}",name, message  ),client);
                this.inputBox.Text = "";
            }
        }

       
    }
}
