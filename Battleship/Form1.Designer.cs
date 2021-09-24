
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
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox)).BeginInit();
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
            this.shipsBox.Location = new System.Drawing.Point(610, 53);
            this.shipsBox.Name = "shipsBox";
            this.shipsBox.Size = new System.Drawing.Size(400, 400);
            this.shipsBox.TabIndex = 1;
            this.shipsBox.TabStop = false;
            this.shipsBox.Paint += new System.Windows.Forms.PaintEventHandler(this.shipsBox_Paint);
            this.shipsBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shipsBox_MouseClick);
            this.shipsBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shipsBox_MouseMove);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 588);
            this.Controls.Add(this.shipsBox);
            this.Controls.Add(this.shotsBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox shotsBox;
        private System.Windows.Forms.PictureBox shipsBox;
    }
}

