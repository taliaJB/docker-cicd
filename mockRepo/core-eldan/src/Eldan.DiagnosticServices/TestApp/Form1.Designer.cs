namespace TestApp
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
            this.btnUpdateSuppliersData = new System.Windows.Forms.Button();
            this.btnUpdateEdiData = new System.Windows.Forms.Button();
            this.btnUpdatePointerData = new System.Windows.Forms.Button();
            this.btnUpdateSupplierData = new System.Windows.Forms.Button();
            this.btnGetCarData = new System.Windows.Forms.Button();
            this.btnUpdateIturanData = new System.Windows.Forms.Button();
            this.btnUpdateInetData = new System.Windows.Forms.Button();
            this.btnCalibrateSupplierCar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpdateSuppliersData
            // 
            this.btnUpdateSuppliersData.Location = new System.Drawing.Point(67, 31);
            this.btnUpdateSuppliersData.Name = "btnUpdateSuppliersData";
            this.btnUpdateSuppliersData.Size = new System.Drawing.Size(203, 23);
            this.btnUpdateSuppliersData.TabIndex = 0;
            this.btnUpdateSuppliersData.Text = "Update Suppliers Data (for schedulare)";
            this.btnUpdateSuppliersData.UseVisualStyleBackColor = true;
            this.btnUpdateSuppliersData.Click += new System.EventHandler(this.btnUpdateSuppliersData_Click);
            // 
            // btnUpdateEdiData
            // 
            this.btnUpdateEdiData.Location = new System.Drawing.Point(67, 89);
            this.btnUpdateEdiData.Name = "btnUpdateEdiData";
            this.btnUpdateEdiData.Size = new System.Drawing.Size(112, 23);
            this.btnUpdateEdiData.TabIndex = 1;
            this.btnUpdateEdiData.Text = "Update Edi Data";
            this.btnUpdateEdiData.UseVisualStyleBackColor = true;
            this.btnUpdateEdiData.Click += new System.EventHandler(this.btnUpdateEdiData_Click);
            // 
            // btnUpdatePointerData
            // 
            this.btnUpdatePointerData.Location = new System.Drawing.Point(185, 89);
            this.btnUpdatePointerData.Name = "btnUpdatePointerData";
            this.btnUpdatePointerData.Size = new System.Drawing.Size(141, 23);
            this.btnUpdatePointerData.TabIndex = 2;
            this.btnUpdatePointerData.Text = "Update Pointer Data";
            this.btnUpdatePointerData.UseVisualStyleBackColor = true;
            this.btnUpdatePointerData.Click += new System.EventHandler(this.btnUpdatePointerData_Click);
            // 
            // btnUpdateSupplierData
            // 
            this.btnUpdateSupplierData.Location = new System.Drawing.Point(67, 60);
            this.btnUpdateSupplierData.Name = "btnUpdateSupplierData";
            this.btnUpdateSupplierData.Size = new System.Drawing.Size(203, 23);
            this.btnUpdateSupplierData.TabIndex = 3;
            this.btnUpdateSupplierData.Text = "UpdateSupplier Data (by demand)";
            this.btnUpdateSupplierData.UseVisualStyleBackColor = true;
            this.btnUpdateSupplierData.Click += new System.EventHandler(this.btnUpdateSupplierData_Click);
            // 
            // btnGetCarData
            // 
            this.btnGetCarData.Location = new System.Drawing.Point(67, 118);
            this.btnGetCarData.Name = "btnGetCarData";
            this.btnGetCarData.Size = new System.Drawing.Size(102, 23);
            this.btnGetCarData.TabIndex = 4;
            this.btnGetCarData.Text = "Get Car Data";
            this.btnGetCarData.UseVisualStyleBackColor = true;
            this.btnGetCarData.Click += new System.EventHandler(this.btnGetCarData_Click);
            // 
            // btnUpdateIturanData
            // 
            this.btnUpdateIturanData.Location = new System.Drawing.Point(332, 89);
            this.btnUpdateIturanData.Name = "btnUpdateIturanData";
            this.btnUpdateIturanData.Size = new System.Drawing.Size(127, 23);
            this.btnUpdateIturanData.TabIndex = 5;
            this.btnUpdateIturanData.Text = "Update Ituran Data";
            this.btnUpdateIturanData.UseVisualStyleBackColor = true;
            this.btnUpdateIturanData.Click += new System.EventHandler(this.btnUpdateIturanData_Click);
            // 
            // btnUpdateInetData
            // 
            this.btnUpdateInetData.Location = new System.Drawing.Point(465, 89);
            this.btnUpdateInetData.Name = "btnUpdateInetData";
            this.btnUpdateInetData.Size = new System.Drawing.Size(131, 23);
            this.btnUpdateInetData.TabIndex = 6;
            this.btnUpdateInetData.Text = "Update Inet Data";
            this.btnUpdateInetData.UseVisualStyleBackColor = true;
            this.btnUpdateInetData.Click += new System.EventHandler(this.btnUpdateInetData_Click);
            // 
            // btnCalibrateSupplierCar
            // 
            this.btnCalibrateSupplierCar.Location = new System.Drawing.Point(67, 147);
            this.btnCalibrateSupplierCar.Name = "btnCalibrateSupplierCar";
            this.btnCalibrateSupplierCar.Size = new System.Drawing.Size(138, 23);
            this.btnCalibrateSupplierCar.TabIndex = 8;
            this.btnCalibrateSupplierCar.Text = "Calibrate Supplier Car";
            this.btnCalibrateSupplierCar.UseVisualStyleBackColor = true;
            this.btnCalibrateSupplierCar.Click += new System.EventHandler(this.btnCalibrateSupplierCar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCalibrateSupplierCar);
            this.Controls.Add(this.btnUpdateInetData);
            this.Controls.Add(this.btnUpdateIturanData);
            this.Controls.Add(this.btnGetCarData);
            this.Controls.Add(this.btnUpdateSupplierData);
            this.Controls.Add(this.btnUpdatePointerData);
            this.Controls.Add(this.btnUpdateEdiData);
            this.Controls.Add(this.btnUpdateSuppliersData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpdateSuppliersData;
        private System.Windows.Forms.Button btnUpdateEdiData;
        private System.Windows.Forms.Button btnUpdatePointerData;
        private System.Windows.Forms.Button btnUpdateSupplierData;
        private System.Windows.Forms.Button btnGetCarData;
        private System.Windows.Forms.Button btnUpdateIturanData;
        private System.Windows.Forms.Button btnUpdateInetData;
        private System.Windows.Forms.Button btnCalibrateSupplierCar;
    }
}

