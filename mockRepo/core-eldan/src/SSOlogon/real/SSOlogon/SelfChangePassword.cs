using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Eldan.SSOlogon
{
	public partial class SelfChangePassword: Form
    {

        #region Private Fields
        string strOldPass = "";
        string strNewPass = "";
        string strConfPass = "";
        #endregion

        #region Public Fields
        public string userSamAccountName { get; set; }
        #endregion

        #region Overloads
        public SelfChangePassword()
		{
			InitializeComponent();
		}

        public SelfChangePassword(string userSamAccountName)
        {

            InitializeComponent();
        }
        #endregion

        Eldan.SSOlogon.ADactions _adActions = new Eldan.SSOlogon.ADactions();

        private void SelfChangePassword_Load(object sender, EventArgs e)
        {
            txtBoxSamAccountName.Text = userSamAccountName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool bSuccess = false;
            CheckIfPasswardValid();
            if (ChangePassword())
            {
                bSuccess = true;
            }
            else
            {
                bSuccess = false;
            }

            LogonSSO logonSSO = new LogonSSO();
            logonSSO.PassChangeSucc = bSuccess;
        }

        internal void CheckIfPasswardValid()
        {
            strOldPass = txtBoxOldPass.Text.Trim();
            strNewPass = txtBoxNewPass.Text.Trim();
            strConfPass = txtBoxConfirmPass.Text.Trim();

            if (strNewPass == strConfPass)
            {
                if (!IsValidPassword(strNewPass))
                {
                    MessageBox.Show("חובה להכניס סיסמא בת 6 תווים לפחות ");
                    strNewPass = "";
                    strConfPass = "";
                    txtBoxNewPass.Text = "";
                    txtBoxConfirmPass.Text = "";
                    return;
                }
            }
            else
            {
                MessageBox.Show("אימות הסיסמא אינו זהה לסיסמא החדשה");
                strNewPass = "";
                strConfPass = "";
                txtBoxNewPass.Text = "";
                txtBoxConfirmPass.Text = "";
                return;
            }
        }

        // Check if password match the Active Directory standard
        private bool IsValidPassword(string strPassword)
        {
            bool bPassOK = false;

            if (Regex.IsMatch(strPassword, @"^([a-zA-Z0-9?_.!]{6,20})$"))   // can enter upper OR lower case OR digits and these signs [?_.!] as you wish
            {
                bPassOK = true;
            }
            else
            {
                bPassOK = false;
            }

            return bPassOK;
        }

        internal bool ChangePassword()
        {
            bool success = false;

            _adActions.userPwd = strNewPass;
            if (_adActions.ChangePassword(_adActions.DomainName,userSamAccountName))
            {
                MessageBox.Show("סיסמא הוחלפה בהצלחה !");
                success = true;
            }
            else
            {
                MessageBox.Show("הסיסמא לא הוחלפה !!");
                success = false;
            }
            
            this.Dispose();
            return success;
        }
	}
}
