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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.statusButton = new System.Windows.Forms.Button();
            this.addRoomBox = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.addRoomButton = new System.Windows.Forms.Button();
            this.roomName = new System.Windows.Forms.TextBox();
            this.roomCapacity = new System.Windows.Forms.TextBox();
            this.roomLocation = new System.Windows.Forms.TextBox();
            this.createClientBox = new System.Windows.Forms.GroupBox();
            this.createClientButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.clientURL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.clientUsername = new System.Windows.Forms.TextBox();
            this.clientServerURL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.clientScript = new System.Windows.Forms.TextBox();
            this.createServerBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.createServerButton = new System.Windows.Forms.Button();
            this.maxDelays = new System.Windows.Forms.TextBox();
            this.serverURL = new System.Windows.Forms.TextBox();
            this.minDelays = new System.Windows.Forms.TextBox();
            this.maxFaults = new System.Windows.Forms.TextBox();
            this.serverID = new System.Windows.Forms.TextBox();
            this.outputBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.addRoomBox.SuspendLayout();
            this.createClientBox.SuspendLayout();
            this.createServerBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PCSUrlTextBox
            // 
            this.PCSUrlTextBox.Location = new System.Drawing.Point(12, 37);
            this.PCSUrlTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PCSUrlTextBox.Name = "PCSUrlTextBox";
            this.PCSUrlTextBox.Size = new System.Drawing.Size(324, 31);
            this.PCSUrlTextBox.TabIndex = 0;
            this.PCSUrlTextBox.Text = "tcp://localhost:10000/pcs";
            // 
            // PCSConnectButton
            // 
            this.PCSConnectButton.Location = new System.Drawing.Point(352, 31);
            this.PCSConnectButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.PCSConnectButton.Name = "PCSConnectButton";
            this.PCSConnectButton.Size = new System.Drawing.Size(150, 44);
            this.PCSConnectButton.TabIndex = 1;
            this.PCSConnectButton.Text = "Connect";
            this.PCSConnectButton.UseVisualStyleBackColor = true;
            this.PCSConnectButton.Click += new System.EventHandler(this.PCSConnectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PCSUrlTextBox);
            this.groupBox1.Controls.Add(this.PCSConnectButton);
            this.groupBox1.Location = new System.Drawing.Point(24, 23);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(516, 92);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PCS URL";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(26, 127);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(514, 260);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PCS List";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(14, 38);
            this.listBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(488, 204);
            this.listBox1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.statusButton);
            this.groupBox3.Controls.Add(this.addRoomBox);
            this.groupBox3.Controls.Add(this.createClientBox);
            this.groupBox3.Controls.Add(this.createServerBox);
            this.groupBox3.Location = new System.Drawing.Point(552, 23);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Size = new System.Drawing.Size(652, 676);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PCS Actions";
            // 
            // statusButton
            // 
            this.statusButton.Location = new System.Drawing.Point(408, 471);
            this.statusButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(232, 194);
            this.statusButton.TabIndex = 18;
            this.statusButton.Text = "STATUS";
            this.statusButton.UseVisualStyleBackColor = true;
            this.statusButton.Click += new System.EventHandler(this.StatusButton_Click);
            // 
            // addRoomBox
            // 
            this.addRoomBox.Controls.Add(this.label12);
            this.addRoomBox.Controls.Add(this.label11);
            this.addRoomBox.Controls.Add(this.label10);
            this.addRoomBox.Controls.Add(this.addRoomButton);
            this.addRoomBox.Controls.Add(this.roomName);
            this.addRoomBox.Controls.Add(this.roomCapacity);
            this.addRoomBox.Controls.Add(this.roomLocation);
            this.addRoomBox.Location = new System.Drawing.Point(8, 463);
            this.addRoomBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.addRoomBox.Name = "addRoomBox";
            this.addRoomBox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.addRoomBox.Size = new System.Drawing.Size(364, 202);
            this.addRoomBox.TabIndex = 17;
            this.addRoomBox.TabStop = false;
            this.addRoomBox.Text = "Room Creation";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 100);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(130, 25);
            this.label12.TabIndex = 6;
            this.label12.Text = "Room Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(256, 31);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 25);
            this.label11.TabIndex = 5;
            this.label11.Text = "Capacity";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 31);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 25);
            this.label10.TabIndex = 4;
            this.label10.Text = "Location";
            // 
            // addRoomButton
            // 
            this.addRoomButton.Location = new System.Drawing.Point(220, 127);
            this.addRoomButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.addRoomButton.Name = "addRoomButton";
            this.addRoomButton.Size = new System.Drawing.Size(132, 38);
            this.addRoomButton.TabIndex = 3;
            this.addRoomButton.Text = "Add Room";
            this.addRoomButton.UseVisualStyleBackColor = true;
            this.addRoomButton.Click += new System.EventHandler(this.AddRoomButton_Click);
            // 
            // roomName
            // 
            this.roomName.Location = new System.Drawing.Point(8, 127);
            this.roomName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.roomName.Name = "roomName";
            this.roomName.Size = new System.Drawing.Size(194, 31);
            this.roomName.TabIndex = 2;
            // 
            // roomCapacity
            // 
            this.roomCapacity.Location = new System.Drawing.Point(254, 56);
            this.roomCapacity.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.roomCapacity.Name = "roomCapacity";
            this.roomCapacity.Size = new System.Drawing.Size(94, 31);
            this.roomCapacity.TabIndex = 1;
            // 
            // roomLocation
            // 
            this.roomLocation.Location = new System.Drawing.Point(8, 56);
            this.roomLocation.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.roomLocation.Name = "roomLocation";
            this.roomLocation.Size = new System.Drawing.Size(230, 31);
            this.roomLocation.TabIndex = 0;
            // 
            // createClientBox
            // 
            this.createClientBox.Controls.Add(this.createClientButton);
            this.createClientBox.Controls.Add(this.label7);
            this.createClientBox.Controls.Add(this.label9);
            this.createClientBox.Controls.Add(this.clientURL);
            this.createClientBox.Controls.Add(this.label8);
            this.createClientBox.Controls.Add(this.clientUsername);
            this.createClientBox.Controls.Add(this.clientServerURL);
            this.createClientBox.Controls.Add(this.label6);
            this.createClientBox.Controls.Add(this.clientScript);
            this.createClientBox.Location = new System.Drawing.Point(6, 225);
            this.createClientBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createClientBox.Name = "createClientBox";
            this.createClientBox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createClientBox.Size = new System.Drawing.Size(634, 235);
            this.createClientBox.TabIndex = 16;
            this.createClientBox.TabStop = false;
            this.createClientBox.Text = "Client Creation";
            // 
            // createClientButton
            // 
            this.createClientButton.Location = new System.Drawing.Point(422, 179);
            this.createClientButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createClientButton.Name = "createClientButton";
            this.createClientButton.Size = new System.Drawing.Size(198, 44);
            this.createClientButton.TabIndex = 21;
            this.createClientButton.Text = "Create Client";
            this.createClientButton.UseVisualStyleBackColor = true;
            this.createClientButton.Click += new System.EventHandler(this.CreateClientButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(214, 27);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "Client URL";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(416, 102);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 25);
            this.label9.TabIndex = 20;
            this.label9.Text = "Script File";
            // 
            // clientURL
            // 
            this.clientURL.Location = new System.Drawing.Point(220, 58);
            this.clientURL.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.clientURL.Name = "clientURL";
            this.clientURL.Size = new System.Drawing.Size(396, 31);
            this.clientURL.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 102);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(123, 25);
            this.label8.TabIndex = 19;
            this.label8.Text = "Server URL";
            // 
            // clientUsername
            // 
            this.clientUsername.Location = new System.Drawing.Point(10, 58);
            this.clientUsername.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.clientUsername.Name = "clientUsername";
            this.clientUsername.Size = new System.Drawing.Size(194, 31);
            this.clientUsername.TabIndex = 11;
            // 
            // clientServerURL
            // 
            this.clientServerURL.Location = new System.Drawing.Point(10, 133);
            this.clientServerURL.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.clientServerURL.Name = "clientServerURL";
            this.clientServerURL.Size = new System.Drawing.Size(396, 31);
            this.clientServerURL.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 27);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 25);
            this.label6.TabIndex = 17;
            this.label6.Text = "Username";
            // 
            // clientScript
            // 
            this.clientScript.Location = new System.Drawing.Point(422, 133);
            this.clientScript.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.clientScript.Name = "clientScript";
            this.clientScript.Size = new System.Drawing.Size(194, 31);
            this.clientScript.TabIndex = 12;
            // 
            // createServerBox
            // 
            this.createServerBox.Controls.Add(this.label5);
            this.createServerBox.Controls.Add(this.label4);
            this.createServerBox.Controls.Add(this.label3);
            this.createServerBox.Controls.Add(this.label2);
            this.createServerBox.Controls.Add(this.label1);
            this.createServerBox.Controls.Add(this.createServerButton);
            this.createServerBox.Controls.Add(this.maxDelays);
            this.createServerBox.Controls.Add(this.serverURL);
            this.createServerBox.Controls.Add(this.minDelays);
            this.createServerBox.Controls.Add(this.maxFaults);
            this.createServerBox.Controls.Add(this.serverID);
            this.createServerBox.Location = new System.Drawing.Point(6, 31);
            this.createServerBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createServerBox.Name = "createServerBox";
            this.createServerBox.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createServerBox.Size = new System.Drawing.Size(634, 183);
            this.createServerBox.TabIndex = 15;
            this.createServerBox.TabStop = false;
            this.createServerBox.Text = "Server Creation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(250, 102);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 25);
            this.label5.TabIndex = 9;
            this.label5.Text = "max delays";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 102);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "min delays";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 102);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "max faults";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "URL";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server ID";
            // 
            // createServerButton
            // 
            this.createServerButton.Location = new System.Drawing.Point(460, 110);
            this.createServerButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.createServerButton.Name = "createServerButton";
            this.createServerButton.Size = new System.Drawing.Size(162, 56);
            this.createServerButton.TabIndex = 10;
            this.createServerButton.Text = "Create Server";
            this.createServerButton.UseVisualStyleBackColor = true;
            this.createServerButton.Click += new System.EventHandler(this.CreateServerButton_Click);
            // 
            // maxDelays
            // 
            this.maxDelays.Location = new System.Drawing.Point(256, 127);
            this.maxDelays.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.maxDelays.Name = "maxDelays";
            this.maxDelays.Size = new System.Drawing.Size(84, 31);
            this.maxDelays.TabIndex = 4;
            // 
            // serverURL
            // 
            this.serverURL.Location = new System.Drawing.Point(238, 58);
            this.serverURL.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.serverURL.Name = "serverURL";
            this.serverURL.Size = new System.Drawing.Size(380, 31);
            this.serverURL.TabIndex = 1;
            // 
            // minDelays
            // 
            this.minDelays.Location = new System.Drawing.Point(138, 127);
            this.minDelays.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.minDelays.Name = "minDelays";
            this.minDelays.Size = new System.Drawing.Size(84, 31);
            this.minDelays.TabIndex = 3;
            // 
            // maxFaults
            // 
            this.maxFaults.Location = new System.Drawing.Point(22, 127);
            this.maxFaults.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.maxFaults.Name = "maxFaults";
            this.maxFaults.Size = new System.Drawing.Size(84, 31);
            this.maxFaults.TabIndex = 2;
            // 
            // serverID
            // 
            this.serverID.Location = new System.Drawing.Point(22, 58);
            this.serverID.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.serverID.Name = "serverID";
            this.serverID.Size = new System.Drawing.Size(200, 31);
            this.serverID.TabIndex = 0;
            // 
            // outputBox
            // 
            this.outputBox.Location = new System.Drawing.Point(24, 417);
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.Size = new System.Drawing.Size(516, 282);
            this.outputBox.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 389);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(98, 25);
            this.label13.TabIndex = 6;
            this.label13.Text = "OUTPUT";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 710);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.addRoomBox.ResumeLayout(false);
            this.addRoomBox.PerformLayout();
            this.createClientBox.ResumeLayout(false);
            this.createClientBox.PerformLayout();
            this.createServerBox.ResumeLayout(false);
            this.createServerBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PCSUrlTextBox;
        private System.Windows.Forms.Button PCSConnectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox maxDelays;
        private System.Windows.Forms.TextBox minDelays;
        private System.Windows.Forms.TextBox maxFaults;
        private System.Windows.Forms.TextBox serverURL;
        private System.Windows.Forms.TextBox serverID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox createClientBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox clientURL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox clientUsername;
        private System.Windows.Forms.TextBox clientServerURL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox clientScript;
        private System.Windows.Forms.GroupBox createServerBox;
        private System.Windows.Forms.Button createServerButton;
        private System.Windows.Forms.GroupBox addRoomBox;
        private System.Windows.Forms.Button addRoomButton;
        private System.Windows.Forms.TextBox roomName;
        private System.Windows.Forms.TextBox roomCapacity;
        private System.Windows.Forms.TextBox roomLocation;
        private System.Windows.Forms.Button createClientButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button statusButton;
        private System.Windows.Forms.TextBox outputBox;
        private System.Windows.Forms.Label label13;
    }
}

