namespace Eldan.SSOlogon
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.lblLoginName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUserAuth = new System.Windows.Forms.Label();
            this.grpBoxAuthMethod = new System.Windows.Forms.GroupBox();
            this.txtDomainName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBoxAppPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxAppUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rdBtnAppAuth = new System.Windows.Forms.RadioButton();
            this.rdBtnWinAuth = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblAuthType = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDomain = new System.Windows.Forms.Label();
            this.grpBoxAuthMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login Name:";
            // 
            // lblLoginName
            // 
            this.lblLoginName.AutoSize = true;
            this.lblLoginName.Location = new System.Drawing.Point(138, 27);
            this.lblLoginName.Name = "lblLoginName";
            this.lblLoginName.Size = new System.Drawing.Size(39, 15);
            this.lblLoginName.TabIndex = 1;
            this.lblLoginName.Text = "name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Authenticated :";
            // 
            // lblUserAuth
            // 
            this.lblUserAuth.AutoSize = true;
            this.lblUserAuth.Location = new System.Drawing.Point(138, 46);
            this.lblUserAuth.Name = "lblUserAuth";
            this.lblUserAuth.Size = new System.Drawing.Size(42, 15);
            this.lblUserAuth.TabIndex = 3;
            this.lblUserAuth.Text = "yes/no";
            // 
            // grpBoxAuthMethod
            // 
            this.grpBoxAuthMethod.Controls.Add(this.txtDomainName);
            this.grpBoxAuthMethod.Controls.Add(this.label7);
            this.grpBoxAuthMethod.Controls.Add(this.txtBoxAppPass);
            this.grpBoxAuthMethod.Controls.Add(this.label4);
            this.grpBoxAuthMethod.Controls.Add(this.txtBoxAppUserName);
            this.grpBoxAuthMethod.Controls.Add(this.label3);
            this.grpBoxAuthMethod.Controls.Add(this.rdBtnAppAuth);
            this.grpBoxAuthMethod.Controls.Add(this.rdBtnWinAuth);
            this.grpBoxAuthMethod.Location = new System.Drawing.Point(22, 96);
            this.grpBoxAuthMethod.Name = "grpBoxAuthMethod";
            this.grpBoxAuthMethod.Size = new System.Drawing.Size(296, 183);
            this.grpBoxAuthMethod.TabIndex = 4;
            this.grpBoxAuthMethod.TabStop = false;
            this.grpBoxAuthMethod.Text = "Authentication Methodes";
            // 
            // txtDomainName
            // 
            this.txtDomainName.Enabled = false;
            this.txtDomainName.Location = new System.Drawing.Point(111, 142);
            this.txtDomainName.MaxLength = 20;
            this.txtDomainName.Name = "txtDomainName";
            this.txtDomainName.Size = new System.Drawing.Size(168, 21);
            this.txtDomainName.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "Domain:";
            // 
            // txtBoxAppPass
            // 
            this.txtBoxAppPass.Enabled = false;
            this.txtBoxAppPass.Location = new System.Drawing.Point(111, 112);
            this.txtBoxAppPass.MaxLength = 20;
            this.txtBoxAppPass.Name = "txtBoxAppPass";
            this.txtBoxAppPass.PasswordChar = '*';
            this.txtBoxAppPass.Size = new System.Drawing.Size(168, 21);
            this.txtBoxAppPass.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Password:";
            // 
            // txtBoxAppUserName
            // 
            this.txtBoxAppUserName.Enabled = false;
            this.txtBoxAppUserName.Location = new System.Drawing.Point(111, 82);
            this.txtBoxAppUserName.MaxLength = 20;
            this.txtBoxAppUserName.Name = "txtBoxAppUserName";
            this.txtBoxAppUserName.Size = new System.Drawing.Size(168, 21);
            this.txtBoxAppUserName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "User Name:";
            // 
            // rdBtnAppAuth
            // 
            this.rdBtnAppAuth.AutoSize = true;
            this.rdBtnAppAuth.Location = new System.Drawing.Point(19, 49);
            this.rdBtnAppAuth.Name = "rdBtnAppAuth";
            this.rdBtnAppAuth.Size = new System.Drawing.Size(188, 17);
            this.rdBtnAppAuth.TabIndex = 1;
            this.rdBtnAppAuth.TabStop = true;
            this.rdBtnAppAuth.Text = "Use other Windows authentication";
            this.rdBtnAppAuth.UseVisualStyleBackColor = true;
            this.rdBtnAppAuth.CheckedChanged += new System.EventHandler(this.rdBtnAppAuth_CheckedChanged);
            // 
            // rdBtnWinAuth
            // 
            this.rdBtnWinAuth.AutoSize = true;
            this.rdBtnWinAuth.Location = new System.Drawing.Point(19, 27);
            this.rdBtnWinAuth.Name = "rdBtnWinAuth";
            this.rdBtnWinAuth.Size = new System.Drawing.Size(171, 17);
            this.rdBtnWinAuth.TabIndex = 0;
            this.rdBtnWinAuth.TabStop = true;
            this.rdBtnWinAuth.Text = "Use current authenticated user";
            this.rdBtnWinAuth.UseVisualStyleBackColor = true;
            this.rdBtnWinAuth.CheckedChanged += new System.EventHandler(this.rdBtnWinAuth_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(88, 301);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(178, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblAuthType
            // 
            this.lblAuthType.AutoSize = true;
            this.lblAuthType.Location = new System.Drawing.Point(138, 65);
            this.lblAuthType.Name = "lblAuthType";
            this.lblAuthType.Size = new System.Drawing.Size(33, 15);
            this.lblAuthType.TabIndex = 8;
            this.lblAuthType.Text = "Kerb";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "Authentication Type:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Domain:";
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(138, 8);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(51, 15);
            this.lblDomain.TabIndex = 10;
            this.lblDomain.Text = "Domain";
            // 
            // Login
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 341);
            this.Controls.Add(this.lblDomain);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblAuthType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpBoxAuthMethod);
            this.Controls.Add(this.lblUserAuth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLoginName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Login";
            this.Text = "Login Form";
            this.Load += new System.EventHandler(this.Login_Load);
            this.grpBoxAuthMethod.ResumeLayout(false);
            this.grpBoxAuthMethod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLoginName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblUserAuth;
        private System.Windows.Forms.GroupBox grpBoxAuthMethod;
        private System.Windows.Forms.TextBox txtBoxAppPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxAppUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdBtnAppAuth;
        private System.Windows.Forms.RadioButton rdBtnWinAuth;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblAuthType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.TextBox txtDomainName;
        private System.Windows.Forms.Label label7;
    }
}

