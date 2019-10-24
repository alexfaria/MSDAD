namespace PuppetMaster
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
            this.PCSUrlTextBox = new System.Windows.Forms.TextBox();
            this.PCSConnectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.createServerButton = new System.Windows.Forms.Button();
            this.createClientButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PCSUrlTextBox
            // 
            this.PCSUrlTextBox.Location = new System.Drawing.Point(6, 19);
            this.PCSUrlTextBox.Name = "PCSUrlTextBox";
            this.PCSUrlTextBox.Size = new System.Drawing.Size(164, 20);
            this.PCSUrlTextBox.TabIndex = 0;
            // 
            // PCSConnectButton
            // 
            this.PCSConnectButton.Location = new System.Drawing.Point(176, 16);
            this.PCSConnectButton.Name = "PCSConnectButton";
            this.PCSConnectButton.Size = new System.Drawing.Size(75, 23);
            this.PCSConnectButton.TabIndex = 1;
            this.PCSConnectButton.Text = "Connect";
            this.PCSConnectButton.UseVisualStyleBackColor = true;
            this.PCSConnectButton.Click += new System.EventHandler(this.PCSConnectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PCSUrlTextBox);
            this.groupBox1.Controls.Add(this.PCSConnectButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 48);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.createClientButton);
            this.groupBox2.Controls.Add(this.createServerButton);
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(13, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 161);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 20);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 134);
            this.listBox1.TabIndex = 0;
            // 
            // createServerButton
            // 
            this.createServerButton.Location = new System.Drawing.Point(134, 20);
            this.createServerButton.Name = "createServerButton";
            this.createServerButton.Size = new System.Drawing.Size(116, 23);
            this.createServerButton.TabIndex = 1;
            this.createServerButton.Text = "create server";
            this.createServerButton.UseVisualStyleBackColor = true;
            this.createServerButton.Click += new System.EventHandler(this.CreateServerButton_Click);
            // 
            // createClientButton
            // 
            this.createClientButton.Location = new System.Drawing.Point(134, 50);
            this.createClientButton.Name = "createClientButton";
            this.createClientButton.Size = new System.Drawing.Size(116, 23);
            this.createClientButton.TabIndex = 2;
            this.createClientButton.Text = "create client";
            this.createClientButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox PCSUrlTextBox;
        private System.Windows.Forms.Button PCSConnectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button createClientButton;
        private System.Windows.Forms.Button createServerButton;
        private System.Windows.Forms.ListBox listBox1;
    }
}

