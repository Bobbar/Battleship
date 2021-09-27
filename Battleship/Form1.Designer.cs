
namespace Battleship
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.shotsBox = new System.Windows.Forms.PictureBox();
            this.shipsBox = new System.Windows.Forms.PictureBox();
            this.shotsBox2 = new System.Windows.Forms.PictureBox();
            this.shipsBox2 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // shotsBox
            // 
            this.shotsBox.BackColor = System.Drawing.Color.Silver;
            this.shotsBox.Location = new System.Drawing.Point(66, 53);
            this.shotsBox.Name = "shotsBox";
            this.shotsBox.Size = new System.Drawing.Size(400, 400);
            this.shotsBox.TabIndex = 0;
            this.shotsBox.TabStop = false;
            this.shotsBox.Paint += new System.Windows.Forms.PaintEventHandler(this.shotsBox_Paint);
            this.shotsBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shotsBox_MouseClick);
            // 
            // shipsBox
            // 
            this.shipsBox.BackColor = System.Drawing.Color.Gray;
            this.shipsBox.Location = new System.Drawing.Point(66, 459);
            this.shipsBox.Name = "shipsBox";
            this.shipsBox.Size = new System.Drawing.Size(400, 400);
            this.shipsBox.TabIndex = 1;
            this.shipsBox.TabStop = false;
            this.shipsBox.Paint += new System.Windows.Forms.PaintEventHandler(this.shipsBox_Paint);
            this.shipsBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shipsBox_MouseClick);
            this.shipsBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shipsBox_MouseMove);
            // 
            // shotsBox2
            // 
            this.shotsBox2.BackColor = System.Drawing.Color.Silver;
            this.shotsBox2.Location = new System.Drawing.Point(648, 53);
            this.shotsBox2.Name = "shotsBox2";
            this.shotsBox2.Size = new System.Drawing.Size(400, 400);
            this.shotsBox2.TabIndex = 2;
            this.shotsBox2.TabStop = false;
            this.shotsBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.shotsBox2_Paint);
            this.shotsBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shotsBox2_MouseClick);
            // 
            // shipsBox2
            // 
            this.shipsBox2.BackColor = System.Drawing.Color.Gray;
            this.shipsBox2.Location = new System.Drawing.Point(648, 459);
            this.shipsBox2.Name = "shipsBox2";
            this.shipsBox2.Size = new System.Drawing.Size(400, 400);
            this.shipsBox2.TabIndex = 3;
            this.shipsBox2.TabStop = false;
            this.shipsBox2.Visible = false;
            this.shipsBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.shipsBox2_Paint);
            this.shipsBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shipsBox2_MouseClick);
            this.shipsBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shipsBox2_MouseMove);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(316, 880);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 20);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(823, 880);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(47, 20);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 917);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.shipsBox2);
            this.Controls.Add(this.shotsBox2);
            this.Controls.Add(this.shipsBox);
            this.Controls.Add(this.shotsBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox shotsBox;
        private System.Windows.Forms.PictureBox shipsBox;
        private System.Windows.Forms.PictureBox shotsBox2;
        private System.Windows.Forms.PictureBox shipsBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

