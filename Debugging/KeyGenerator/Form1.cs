using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyGenerator
{
    partial class Form1 : Form
    {
        private Button eval_c;
        private TextBox eval_d;

        public Form1()
        {
            this.eval_a();
        }

        private void eval_a(object A_0, EventArgs A_1)
        {
          this.eval_d.Text = generateKey();
        }

        private string generateKey()
        {
            EvalClass evalA = new EvalClass();
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault<NetworkInterface>();

            if (networkInterface == null)
            {
                return "You need a network interace";
            }

            evalA.a = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            var PABytes = networkInterface.GetPhysicalAddress().GetAddressBytes();

            return String.Join("-", PABytes.Select(evalA.eval_bi).Select(CGenr.eval_a));
        }

        class CGenr
        {
            public static int eval_a(int A_0)
            {
                return A_0 * 10;
            }
        }

        private void eval_a()
        {
            this.eval_c = new Button();
            this.eval_d = new TextBox();
            base.SuspendLayout();
            this.eval_c.Location = new Point(268, 51);
            this.eval_c.Name = "bt_generate";
            this.eval_c.Size = new Size(75, 23);
            this.eval_c.TabIndex = 0;
            this.eval_c.Text = "Generate";
            this.eval_c.UseVisualStyleBackColor = true;
            this.eval_c.Click += new EventHandler(this.eval_a);
            this.eval_d.Location = new Point(35, 25);
            this.eval_d.Name = "tb_key";
            this.eval_d.Size = new Size(308, 20);
            this.eval_d.TabIndex = 1;
            this.eval_d.Text = $"Press \"{this.eval_c.Text}\" button to generate key...";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(369, 86);
            base.Controls.Add(this.eval_d);
            base.Controls.Add(this.eval_c);
            base.Name = "Form1";
            this.Text = "Key for \"Crack me\"";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        class EvalClass
        {
            public byte[] a;

            public int eval_bi(byte A_0, int A_1)
            {
                return (int)(A_0 ^ this.a[A_1]);
            }
        }
    }
}
