using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Principal;

namespace Eldan.SSOlogon
{
    public partial class Login : Form
    {
        //private int m_attempts = 0;
        //private const int MAX_LOGIN_ATTEMPTS = 5;
        private string usrDomain;

        public Login()
        {
            InitializeComponent();
        }

        LogonSSO usrProperties = new LogonSSO();

        private void Login_Load(object sender, EventArgs e)
        {
            rdBtnWinAuth.Checked = true;

            usrDomain = usrProperties.UsrDomain;
            lblDomain.Text = usrDomain;
            
            if (usrProperties.LogonUserName != null)
            {
                lblLoginName.Text = usrProperties.LogonUserName;
            }
            else
            {
                lblLoginName.Text = "User not authenticated";
            }

            if (usrProperties.UserIsAuthenticated)
            {
                lblUserAuth.Text = "YES";
            }
            else
            {
                lblUserAuth.Text = "NO";
            }

            lblAuthType.Text = usrProperties.AuthenticationType;
             
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdBtnWinAuth.Checked)
            {
                MessageBox.Show("Windows Logged on user is: " + lblLoginName.Text);
            }

            if (rdBtnAppAuth.Checked)
            {
                if (usrProperties.IsUserValid(txtBoxAppUserName.Text, txtBoxAppPass.Text, txtDomainName.Text))
                {
                    MessageBox.Show("Application user is: " + txtBoxAppUserName.Text + " Is Authenticated");
                    lblDomain.Text = usrProperties.UsrDomain;
                    lblLoginName.Text = usrProperties.LogonUserName;
                    if (usrProperties.UserIsAuthenticated)
                    {
                        lblUserAuth.Text = "YES";
                    }
                    else
                    {
                        lblUserAuth.Text = "NO";
                    }

                    lblAuthType.Text = usrProperties.AuthenticationType;
                }
                else
                {
                    MessageBox.Show("Application user is: " + txtBoxAppUserName.Text + " Is NOT Authenticated");
                }
            }
        }

        private void rdBtnAppAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnAppAuth.Checked)
            {
                txtBoxAppUserName.Enabled = true;
                txtBoxAppPass.Enabled =true;
                txtDomainName.Enabled = true;
                txtBoxAppUserName.Focus();

                lblDomain.Visible = false;
                lblLoginName.Visible = false;
                lblUserAuth.Visible = false;
                lblAuthType.Visible = false;

                txtDomainName.Text = usrDomain;
            }
        }

        private void rdBtnWinAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnWinAuth.Checked)
            {
                txtBoxAppUserName.Enabled = false;
                txtBoxAppPass.Enabled = false;
                txtDomainName.Enabled = false;

                lblDomain.Visible = true;
                lblLoginName.Visible = true;
                lblUserAuth.Visible = true;
                lblAuthType.Visible = true;
            }
        }
    }
}
