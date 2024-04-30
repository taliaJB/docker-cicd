namespace Eldan.SSOlogon
{
    partial class SelfChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelfChangePassword));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBoxOldPass = new System.Windows.Forms.TextBox();
            this.txtBoxNewPass = new System.Windows.Forms.TextBox();
            this.txtBoxConfirmPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxSamAccountName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtBoxOldPass
            // 
            resources.ApplyResources(this.txtBoxOldPass, "txtBoxOldPass");
            this.txtBoxOldPass.Name = "txtBoxOldPass";
            // 
            // txtBoxNewPass
            // 
            this.txtBoxNewPass.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.txtBoxNewPass, "txtBoxNewPass");
            this.txtBoxNewPass.Name = "txtBoxNewPass";
            // 
            // txtBoxConfirmPass
            // 
            this.txtBoxConfirmPass.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.txtBoxConfirmPass, "txtBoxConfirmPass");
            this.txtBoxConfirmPass.Name = "txtBoxConfirmPass";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtBoxSamAccountName
            // 
            resources.ApplyResources(this.txtBoxSamAccountName, "txtBoxSamAccountName");
            this.txtBoxSamAccountName.Name = "txtBoxSamAccountName";
            this.txtBoxSamAccountName.ReadOnly = true;
            this.txtBoxSamAccountName.TabStop = false;
            // 
            // SelfChangePassword
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.txtBoxSamAccountName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBoxConfirmPass);
            this.Controls.Add(this.txtBoxNewPass);
            this.Controls.Add(this.txtBoxOldPass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "SelfChangePassword";
            this.Load += new System.EventHandler(this.SelfChangePassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBoxOldPass;
        private System.Windows.Forms.TextBox txtBoxNewPass;
        private System.Windows.Forms.TextBox txtBoxConfirmPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxSamAccountName;
    }
}