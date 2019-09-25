using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core3Study.GRpc.WinFormClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new FirstTest.FirstTestClient(channel);

            var reply = client.GetCacheValue(new Request() { });

            MessageBox.Show($"调用自己的first服务:{reply.Value}");


        }

    }
}
