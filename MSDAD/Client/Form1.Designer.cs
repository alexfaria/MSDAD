namespace Client
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
            this.showBtn = new System.Windows.Forms.Button();
            this.topicBox = new System.Windows.Forms.TextBox();
            this.createBtn = new System.Windows.Forms.Button();
            this.userBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.participantsBox = new System.Windows.Forms.TextBox();
            this.numInviteesBox = new System.Windows.Forms.TextBox();
            this.numSlotsBox = new System.Windows.Forms.TextBox();
            this.inviteesBox = new System.Windows.Forms.TextBox();
            this.slotsBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.joinBtn = new System.Windows.Forms.Button();
            this.meetingsBox = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // showBtn
            // 
            this.showBtn.Location = new System.Drawing.Point(1149, 619);
            this.showBtn.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.showBtn.Name = "showBtn";
            this.showBtn.Size = new System.Drawing.Size(200, 187);
            this.showBtn.TabIndex = 1;
            this.showBtn.Text = "Show";
            this.showBtn.UseVisualStyleBackColor = true;
            this.showBtn.Click += new System.EventHandler(this.showBtn_Click);
            // 
            // topicBox
            // 
            this.topicBox.Location = new System.Drawing.Point(32, 129);
            this.topicBox.Name = "topicBox";
            this.topicBox.Size = new System.Drawing.Size(509, 38);
            this.topicBox.TabIndex = 2;
            // 
            // createBtn
            // 
            this.createBtn.Location = new System.Drawing.Point(1149, 44);
            this.createBtn.Name = "createBtn";
            this.createBtn.Size = new System.Drawing.Size(200, 509);
            this.createBtn.TabIndex = 3;
            this.createBtn.Text = "Create";
            this.createBtn.UseVisualStyleBackColor = true;
            this.createBtn.Click += new System.EventHandler(this.createBtn_Click);
            // 
            // userBox
            // 
            this.userBox.Location = new System.Drawing.Point(32, 44);
            this.userBox.Name = "userBox";
            this.userBox.Size = new System.Drawing.Size(1095, 38);
            this.userBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 581);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 31);
            this.label1.TabIndex = 5;
            this.label1.Text = "Meetings";
            // 
            // participantsBox
            // 
            this.participantsBox.Location = new System.Drawing.Point(611, 129);
            this.participantsBox.Name = "participantsBox";
            this.participantsBox.Size = new System.Drawing.Size(516, 38);
            this.participantsBox.TabIndex = 6;
            // 
            // numInviteesBox
            // 
            this.numInviteesBox.Location = new System.Drawing.Point(611, 515);
            this.numInviteesBox.Name = "numInviteesBox";
            this.numInviteesBox.Size = new System.Drawing.Size(516, 38);
            this.numInviteesBox.TabIndex = 7;
            // 
            // numSlotsBox
            // 
            this.numSlotsBox.Location = new System.Drawing.Point(32, 515);
            this.numSlotsBox.Name = "numSlotsBox";
            this.numSlotsBox.Size = new System.Drawing.Size(509, 38);
            this.numSlotsBox.TabIndex = 8;
            // 
            // inviteesBox
            // 
            this.inviteesBox.Location = new System.Drawing.Point(611, 222);
            this.inviteesBox.Multiline = true;
            this.inviteesBox.Name = "inviteesBox";
            this.inviteesBox.Size = new System.Drawing.Size(516, 239);
            this.inviteesBox.TabIndex = 9;
            // 
            // slotsBox
            // 
            this.slotsBox.Location = new System.Drawing.Point(32, 222);
            this.slotsBox.Multiline = true;
            this.slotsBox.Name = "slotsBox";
            this.slotsBox.Size = new System.Drawing.Size(509, 239);
            this.slotsBox.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 31);
            this.label2.TabIndex = 11;
            this.label2.Text = "Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(605, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(405, 31);
            this.label3.TabIndex = 12;
            this.label3.Text = "Minimum Number of Participants";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(605, 481);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(362, 31);
            this.label4.TabIndex = 13;
            this.label4.Text = "Number of Invitees (optional)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 31);
            this.label5.TabIndex = 14;
            this.label5.Text = "Topic";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(605, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(229, 31);
            this.label6.TabIndex = 15;
            this.label6.Text = "Invitees (optional)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 188);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 31);
            this.label7.TabIndex = 16;
            this.label7.Text = "Slots";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 481);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(208, 31);
            this.label8.TabIndex = 17;
            this.label8.Text = "Number of Slots";
            // 
            // joinBtn
            // 
            this.joinBtn.Location = new System.Drawing.Point(1149, 844);
            this.joinBtn.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.joinBtn.Name = "joinBtn";
            this.joinBtn.Size = new System.Drawing.Size(200, 182);
            this.joinBtn.TabIndex = 19;
            this.joinBtn.Text = "Join (Selected)";
            this.joinBtn.UseVisualStyleBackColor = true;
            this.joinBtn.Click += new System.EventHandler(this.joinBtn_Click_1);
            // 
            // meetingsBox
            // 
            this.meetingsBox.Location = new System.Drawing.Point(32, 619);
            this.meetingsBox.Name = "meetingsBox";
            this.meetingsBox.Size = new System.Drawing.Size(1095, 407);
            this.meetingsBox.TabIndex = 20;
            this.meetingsBox.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.meetingsBox_AfterSelect);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 1073);
            this.Controls.Add(this.meetingsBox);
            this.Controls.Add(this.joinBtn);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.slotsBox);
            this.Controls.Add(this.inviteesBox);
            this.Controls.Add(this.numSlotsBox);
            this.Controls.Add(this.numInviteesBox);
            this.Controls.Add(this.participantsBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.userBox);
            this.Controls.Add(this.createBtn);
            this.Controls.Add(this.topicBox);
            this.Controls.Add(this.showBtn);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button showBtn;
        private System.Windows.Forms.TextBox topicBox;
        private System.Windows.Forms.Button createBtn;
        private System.Windows.Forms.TextBox userBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox participantsBox;
        private System.Windows.Forms.TextBox numInviteesBox;
        private System.Windows.Forms.TextBox numSlotsBox;
        private System.Windows.Forms.TextBox inviteesBox;
        private System.Windows.Forms.TextBox slotsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button joinBtn;
        private System.Windows.Forms.TreeView meetingsBox;
    }
}

