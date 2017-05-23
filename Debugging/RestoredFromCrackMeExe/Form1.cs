using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace RestoredFromCrackMeExe
{
    partial class Form1 : Form
    {
        private string[] a;
        private IContainer eval_b;
        private Button eval_c;
        private TextBox eval_d;
        private Label eval_e;
        private Label eval_f;
        [NonSerialized]
        string eval_g;

        public Form1()
        {
            this.eval_a();
        }

        private void eval_a(object A_0, EventArgs A_1)
        {
            if (string.IsNullOrEmpty(this.eval_d.Text))
            {
                this.eval_f.Text = "Key cannot be empty";
            }
            else
            {
                this.a = this.eval_d.Text.Split(new char[]
                {
                    '-'
                });
                this.eval_f.Text = (this.eval_a(this.a) ? "Correct key" : "Wrong key");
                this.eval_f.Visible = true;
            }
        }

        private bool eval_a(string[] A_0)
        {
            NetworkInterface networkInterface;
            EvalClass evalA = new EvalClass();
            networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault<NetworkInterface>();

            if (networkInterface == null)
            {
                return false;
            }

            evalA.a = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());
            evalA.b = A_0.Select(int.Parse).ToArray<int>();

            IEnumerable<byte> arg_11B_0 = networkInterface.GetPhysicalAddress().GetAddressBytes();
            var t = arg_11B_0.Select(evalA.eval_bi)
                            .Select(CGenr.eval_a).ToArray();

            return t.Select(evalA.eval_ii).All(CGenr.eval_b);
        }

        class CGenr
        {
            public static int eval_a(int A_0)
            {
                return A_0 * 10;
            }

            public static bool eval_b(int A_0)
            {
                return A_0 == 0;
            }
        }

        private void eval_a()
        {
            this.eval_c = new Button();
            this.eval_d = new TextBox();
            this.eval_e = new Label();
            this.eval_f = new Label();
            base.SuspendLayout();
            this.eval_c.Location = new Point(268, 51);
            this.eval_c.Name = "bt_check";
            this.eval_c.Size = new Size(75, 23);
            this.eval_c.TabIndex = 0;
            this.eval_c.Text = "Check";
            this.eval_c.UseVisualStyleBackColor = true;
            this.eval_c.Click += new EventHandler(this.eval_a);
            this.eval_d.Location = new Point(35, 25);
            this.eval_d.Name = "tb_key";
            this.eval_d.Size = new Size(308, 20);
            this.eval_d.TabIndex = 1;
            this.eval_e.AutoSize = true;
            this.eval_e.Location = new Point(32, 9);
            this.eval_e.Name = "label1";
            this.eval_e.Size = new Size(107, 13);
            this.eval_e.TabIndex = 2;
            this.eval_e.Text = "Please, enter the key";
            this.eval_f.AutoSize = true;
            this.eval_f.Location = new Point(35, 52);
            this.eval_f.Name = "lb_status";
            this.eval_f.Size = new Size(35, 13);
            this.eval_f.TabIndex = 3;
            this.eval_f.Text = "label2";
            this.eval_f.Visible = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(369, 86);
            base.Controls.Add(this.eval_f);
            base.Controls.Add(this.eval_e);
            base.Controls.Add(this.eval_d);
            base.Controls.Add(this.eval_c);
            base.Name = "Form1";
            this.Text = "Crack me";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        class EvalClass
        {
            public byte[] a;
            public int[] b;

            public int eval_bi(byte A_0, int A_1)
            {
                return (int)(A_0 ^ this.a[A_1]);
            }

            public int eval_ii(int A_0, int A_1)
            {
                return A_0 - this.b[A_1];
            }
        }
    }
}
