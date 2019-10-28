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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.addRoomButton = new System.Windows.Forms.Button();
            this.roomName = new System.Windows.Forms.TextBox();
            this.roomCapacity = new System.Windows.Forms.TextBox();
            this.roomLocation = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.createClientButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.clientURL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.clientUsername = new System.Windows.Forms.TextBox();
            this.clientServerURL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.clientScript = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // PCSUrlTextBox
            // 
            this.PCSUrlTextBox.Location = new System.Drawing.Point(6, 19);
            this.PCSUrlTextBox.Name = "PCSUrlTextBox";
            this.PCSUrlTextBox.Size = new System.Drawing.Size(164, 20);
            this.PCSUrlTextBox.TabIndex = 0;
            this.PCSUrlTextBox.Text = "tcp://localhost:10000/pcs";
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
            this.groupBox1.Text = "PCS URL";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(13, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(195, 303);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PCS List";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 20);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(162, 277);
            this.listBox1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.statusButton);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(276, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(326, 358);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PCS Actions";
            // 
            // statusButton
            // 
            this.statusButton.Location = new System.Drawing.Point(204, 245);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(116, 101);
            this.statusButton.TabIndex = 18;
            this.statusButton.Text = "STATUS";
            this.statusButton.UseVisualStyleBackColor = true;
            this.statusButton.Click += new System.EventHandler(this.StatusButton_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.addRoomButton);
            this.groupBox6.Controls.Add(this.roomName);
            this.groupBox6.Controls.Add(this.roomCapacity);
            this.groupBox6.Controls.Add(this.roomLocation);
            this.groupBox6.Location = new System.Drawing.Point(4, 241);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(182, 105);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Room Creation";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 52);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "Room Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(128, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Capacity";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Location";
            // 
            // addRoomButton
            // 
            this.addRoomButton.Location = new System.Drawing.Point(110, 66);
            this.addRoomButton.Name = "addRoomButton";
            this.addRoomButton.Size = new System.Drawing.Size(66, 20);
            this.addRoomButton.TabIndex = 3;
            this.addRoomButton.Text = "Add Room";
            this.addRoomButton.UseVisualStyleBackColor = true;
            this.addRoomButton.Click += new System.EventHandler(this.AddRoomButton_Click);
            // 
            // roomName
            // 
            this.roomName.Location = new System.Drawing.Point(4, 66);
            this.roomName.Name = "roomName";
            this.roomName.Size = new System.Drawing.Size(99, 20);
            this.roomName.TabIndex = 2;
            // 
            // roomCapacity
            // 
            this.roomCapacity.Location = new System.Drawing.Point(127, 29);
            this.roomCapacity.Name = "roomCapacity";
            this.roomCapacity.Size = new System.Drawing.Size(49, 20);
            this.roomCapacity.TabIndex = 1;
            // 
            // roomLocation
            // 
            this.roomLocation.Location = new System.Drawing.Point(4, 29);
            this.roomLocation.Name = "roomLocation";
            this.roomLocation.Size = new System.Drawing.Size(117, 20);
            this.roomLocation.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.createClientButton);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.clientURL);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.clientUsername);
            this.groupBox5.Controls.Add(this.clientServerURL);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.clientScript);
            this.groupBox5.Location = new System.Drawing.Point(3, 117);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(317, 122);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Client Creation";
            // 
            // createClientButton
            // 
            this.createClientButton.Location = new System.Drawing.Point(211, 93);
            this.createClientButton.Name = "createClientButton";
            this.createClientButton.Size = new System.Drawing.Size(99, 23);
            this.createClientButton.TabIndex = 21;
            this.createClientButton.Text = "Create Client";
            this.createClientButton.UseVisualStyleBackColor = true;
            this.createClientButton.Click += new System.EventHandler(this.CreateClientButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(107, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Client URL";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(208, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Script File";
            // 
            // clientURL
            // 
            this.clientURL.Location = new System.Drawing.Point(110, 30);
            this.clientURL.Name = "clientURL";
            this.clientURL.Size = new System.Drawing.Size(200, 20);
            this.clientURL.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Server URL";
            // 
            // clientUsername
            // 
            this.clientUsername.Location = new System.Drawing.Point(5, 30);
            this.clientUsername.Name = "clientUsername";
            this.clientUsername.Size = new System.Drawing.Size(99, 20);
            this.clientUsername.TabIndex = 11;
            // 
            // clientServerURL
            // 
            this.clientServerURL.Location = new System.Drawing.Point(5, 69);
            this.clientServerURL.Name = "clientServerURL";
            this.clientServerURL.Size = new System.Drawing.Size(200, 20);
            this.clientServerURL.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Username";
            // 
            // clientScript
            // 
            this.clientScript.Location = new System.Drawing.Point(211, 69);
            this.clientScript.Name = "clientScript";
            this.clientScript.Size = new System.Drawing.Size(99, 20);
            this.clientScript.TabIndex = 12;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.createServerButton);
            this.groupBox4.Controls.Add(this.maxDelays);
            this.groupBox4.Controls.Add(this.serverURL);
            this.groupBox4.Controls.Add(this.minDelays);
            this.groupBox4.Controls.Add(this.maxFaults);
            this.groupBox4.Controls.Add(this.serverID);
            this.groupBox4.Location = new System.Drawing.Point(3, 16);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(317, 95);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Server Creation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(125, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "max delays";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "min delays";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "max faults";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "URL";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server ID";
            // 
            // createServerButton
            // 
            this.createServerButton.Location = new System.Drawing.Point(230, 57);
            this.createServerButton.Name = "createServerButton";
            this.createServerButton.Size = new System.Drawing.Size(81, 29);
            this.createServerButton.TabIndex = 10;
            this.createServerButton.Text = "Create Server";
            this.createServerButton.UseVisualStyleBackColor = true;
            this.createServerButton.Click += new System.EventHandler(this.CreateServerButton_Click);
            // 
            // maxDelays
            // 
            this.maxDelays.Location = new System.Drawing.Point(128, 66);
            this.maxDelays.Name = "maxDelays";
            this.maxDelays.Size = new System.Drawing.Size(44, 20);
            this.maxDelays.TabIndex = 4;
            // 
            // serverURL
            // 
            this.serverURL.Location = new System.Drawing.Point(119, 30);
            this.serverURL.Name = "serverURL";
            this.serverURL.Size = new System.Drawing.Size(192, 20);
            this.serverURL.TabIndex = 1;
            // 
            // minDelays
            // 
            this.minDelays.Location = new System.Drawing.Point(69, 66);
            this.minDelays.Name = "minDelays";
            this.minDelays.Size = new System.Drawing.Size(44, 20);
            this.minDelays.TabIndex = 3;
            // 
            // maxFaults
            // 
            this.maxFaults.Location = new System.Drawing.Point(11, 66);
            this.maxFaults.Name = "maxFaults";
            this.maxFaults.Size = new System.Drawing.Size(44, 20);
            this.maxFaults.TabIndex = 2;
            // 
            // serverID
            // 
            this.serverID.Location = new System.Drawing.Point(11, 30);
            this.serverID.Name = "serverID";
            this.serverID.Size = new System.Drawing.Size(102, 20);
            this.serverID.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 450);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox clientURL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox clientUsername;
        private System.Windows.Forms.TextBox clientServerURL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox clientScript;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button createServerButton;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button addRoomButton;
        private System.Windows.Forms.TextBox roomName;
        private System.Windows.Forms.TextBox roomCapacity;
        private System.Windows.Forms.TextBox roomLocation;
        private System.Windows.Forms.Button createClientButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button statusButton;
    }
}

