
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
            this.randomizeButton = new System.Windows.Forms.Button();
            this.hideShowButton = new System.Windows.Forms.Button();
            this.winnerLabel = new System.Windows.Forms.Label();
            this.playerShipsLabel = new System.Windows.Forms.Label();
            this.computerShipsLabel = new System.Windows.Forms.Label();
            this.playerShotsLabel = new System.Windows.Forms.Label();
            this.computerShotsLabel = new System.Windows.Forms.Label();
            this.clearBoardsButton = new System.Windows.Forms.Button();
            this.randomizeComputerButton = new System.Windows.Forms.Button();
            this.shipSunkLabel = new System.Windows.Forms.Label();
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
            this.shotsBox2.Location = new System.Drawing.Point(707, 53);
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
            this.shipsBox2.Location = new System.Drawing.Point(707, 459);
            this.shipsBox2.Name = "shipsBox2";
            this.shipsBox2.Size = new System.Drawing.Size(400, 400);
            this.shipsBox2.TabIndex = 3;
            this.shipsBox2.TabStop = false;
            this.shipsBox2.Visible = false;
            this.shipsBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.shipsBox2_Paint);
            this.shipsBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.shipsBox2_MouseClick);
            this.shipsBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shipsBox2_MouseMove);
            // 
            // randomizeButton
            // 
            this.randomizeButton.Location = new System.Drawing.Point(12, 865);
            this.randomizeButton.Name = "randomizeButton";
            this.randomizeButton.Size = new System.Drawing.Size(117, 20);
            this.randomizeButton.TabIndex = 4;
            this.randomizeButton.Text = "Randomize Boards";
            this.randomizeButton.UseVisualStyleBackColor = true;
            this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
            // 
            // hideShowButton
            // 
            this.hideShowButton.Location = new System.Drawing.Point(949, 865);
            this.hideShowButton.Name = "hideShowButton";
            this.hideShowButton.Size = new System.Drawing.Size(99, 20);
            this.hideShowButton.TabIndex = 5;
            this.hideShowButton.Text = "Hide/Show";
            this.hideShowButton.UseVisualStyleBackColor = true;
            this.hideShowButton.Click += new System.EventHandler(this.hideShowButton_Click);
            // 
            // winnerLabel
            // 
            this.winnerLabel.BackColor = System.Drawing.Color.Maroon;
            this.winnerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.winnerLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.winnerLabel.Location = new System.Drawing.Point(434, 13);
            this.winnerLabel.Name = "winnerLabel";
            this.winnerLabel.Size = new System.Drawing.Size(305, 35);
            this.winnerLabel.TabIndex = 6;
            this.winnerLabel.Text = "Player 1 Wins!!!";
            this.winnerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.winnerLabel.Visible = false;
            // 
            // playerShipsLabel
            // 
            this.playerShipsLabel.AutoSize = true;
            this.playerShipsLabel.Location = new System.Drawing.Point(215, 35);
            this.playerShipsLabel.Name = "playerShipsLabel";
            this.playerShipsLabel.Size = new System.Drawing.Size(59, 13);
            this.playerShipsLabel.TabIndex = 7;
            this.playerShipsLabel.Text = "Ships (0/0)";
            // 
            // computerShipsLabel
            // 
            this.computerShipsLabel.AutoSize = true;
            this.computerShipsLabel.Location = new System.Drawing.Point(1048, 35);
            this.computerShipsLabel.Name = "computerShipsLabel";
            this.computerShipsLabel.Size = new System.Drawing.Size(59, 13);
            this.computerShipsLabel.TabIndex = 8;
            this.computerShipsLabel.Text = "Ships (0/0)";
            // 
            // playerShotsLabel
            // 
            this.playerShotsLabel.AutoSize = true;
            this.playerShotsLabel.Location = new System.Drawing.Point(63, 35);
            this.playerShotsLabel.Name = "playerShotsLabel";
            this.playerShotsLabel.Size = new System.Drawing.Size(74, 13);
            this.playerShotsLabel.TabIndex = 9;
            this.playerShotsLabel.Text = "Shots Taken: ";
            // 
            // computerShotsLabel
            // 
            this.computerShotsLabel.AutoSize = true;
            this.computerShotsLabel.Location = new System.Drawing.Point(922, 35);
            this.computerShotsLabel.Name = "computerShotsLabel";
            this.computerShotsLabel.Size = new System.Drawing.Size(74, 13);
            this.computerShotsLabel.TabIndex = 10;
            this.computerShotsLabel.Text = "Shots Taken: ";
            // 
            // clearBoardsButton
            // 
            this.clearBoardsButton.Location = new System.Drawing.Point(349, 885);
            this.clearBoardsButton.Name = "clearBoardsButton";
            this.clearBoardsButton.Size = new System.Drawing.Size(117, 20);
            this.clearBoardsButton.TabIndex = 11;
            this.clearBoardsButton.Text = "Clear Boards";
            this.clearBoardsButton.UseVisualStyleBackColor = true;
            this.clearBoardsButton.Click += new System.EventHandler(this.clearBoardsButton_Click);
            // 
            // randomizeComputerButton
            // 
            this.randomizeComputerButton.Location = new System.Drawing.Point(12, 885);
            this.randomizeComputerButton.Name = "randomizeComputerButton";
            this.randomizeComputerButton.Size = new System.Drawing.Size(165, 20);
            this.randomizeComputerButton.TabIndex = 12;
            this.randomizeComputerButton.Text = "Randomize Computer Board";
            this.randomizeComputerButton.UseVisualStyleBackColor = true;
            this.randomizeComputerButton.Click += new System.EventHandler(this.randomizeComputerButton_Click);
            // 
            // shipSunkLabel
            // 
            this.shipSunkLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.shipSunkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shipSunkLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.shipSunkLabel.Location = new System.Drawing.Point(482, 138);
            this.shipSunkLabel.Name = "shipSunkLabel";
            this.shipSunkLabel.Size = new System.Drawing.Size(210, 33);
            this.shipSunkLabel.TabIndex = 13;
            this.shipSunkLabel.Text = "Player 1 Battleship Sunk!";
            this.shipSunkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.shipSunkLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 917);
            this.Controls.Add(this.shipSunkLabel);
            this.Controls.Add(this.randomizeComputerButton);
            this.Controls.Add(this.clearBoardsButton);
            this.Controls.Add(this.computerShotsLabel);
            this.Controls.Add(this.playerShotsLabel);
            this.Controls.Add(this.computerShipsLabel);
            this.Controls.Add(this.playerShipsLabel);
            this.Controls.Add(this.winnerLabel);
            this.Controls.Add(this.hideShowButton);
            this.Controls.Add(this.randomizeButton);
            this.Controls.Add(this.shipsBox2);
            this.Controls.Add(this.shotsBox2);
            this.Controls.Add(this.shipsBox);
            this.Controls.Add(this.shotsBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shotsBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shipsBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox shotsBox;
        private System.Windows.Forms.PictureBox shipsBox;
        private System.Windows.Forms.PictureBox shotsBox2;
        private System.Windows.Forms.PictureBox shipsBox2;
        private System.Windows.Forms.Button randomizeButton;
        private System.Windows.Forms.Button hideShowButton;
        private System.Windows.Forms.Label winnerLabel;
        private System.Windows.Forms.Label playerShipsLabel;
        private System.Windows.Forms.Label computerShipsLabel;
        private System.Windows.Forms.Label playerShotsLabel;
        private System.Windows.Forms.Label computerShotsLabel;
        private System.Windows.Forms.Button clearBoardsButton;
        private System.Windows.Forms.Button randomizeComputerButton;
        private System.Windows.Forms.Label shipSunkLabel;
    }
}

