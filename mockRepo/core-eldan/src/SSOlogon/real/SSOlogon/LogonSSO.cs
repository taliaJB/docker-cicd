using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace Eldan.SSOlogon
{
    public class LogonSSO : IDisposable
    {
        #region Define Private Fields
        private string _domain = "";            // get the Full Domain Name (eldan.co.il)
        private string _usrDomain = "";         // get the Domain Name (eldan)
        private string _loggedonUserName = "";
        private string _loggedonUserPass = "";
        private bool _isAuthenticated = false;
        private string _authType = "";
        private string _strMsg = "";
        private string _toLog = "";
        private string _userToCheck = "";
        private string _userPassToCheck = "";
        private bool _passChangeSucc = false;
        private DateTime _pwdLastSetDate;
        #endregion

        #region Define Public Properties
        public string DomainName        // get the Domain Name (sasson.co.il)
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string UsrDomain
        {
            get { return _usrDomain; }
        }

        public string LogonUserName
        {
            get { return _loggedonUserName; }
            set { _loggedonUserName = value; }
        }

        public string LogonUserPass
        {
            get { return _loggedonUserPass; }
            set { _loggedonUserPass = value; }
        }

        public bool UserIsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public string AuthenticationType
        {
            get { return _authType; }
        }

        public string StrMsg
        {
            get { return _strMsg; }
            set { _strMsg = value; }
        }

        public string ToLog
        {
            get { return _toLog; }
            set { _toLog = value; }
        }

        public string UserToCheck
        {
            get { return _userToCheck; }
            set { _userToCheck = value; }
        }

        public string UserPassToCheck
        {
            get { return _userPassToCheck; }
            set { _userPassToCheck = value; }
        }

        public bool PassChangeSucc
        {
            get { return _passChangeSucc; }
            set { _passChangeSucc = value; }
        }

        public DateTime PwdLastSetDate
        {
            get { return _pwdLastSetDate; }
            set { _pwdLastSetDate = value; }
        }
        #endregion

        // Constructor

        private bool m_Silent;

        public LogonSSO() : this(false)
        {
        }

        public LogonSSO(bool Silent)
        {
            m_Silent = Silent;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (identity.IsAuthenticated)
                {
                    _isAuthenticated = true;
                }
                else
                {
                    _isAuthenticated = false;
                }

                _domain = Domain.GetCurrentDomain().Name.ToString();
                _loggedonUserName = identity.Name.Substring(identity.Name.IndexOf("\\") + 1);
                _authType = identity.AuthenticationType;
                _usrDomain = identity.Name.Substring(0, identity.Name.IndexOf("\\"));
            }
        }


        public bool IsUserValid(string usrNameToCheck, string usrPassToCheck, string usrDomain)
        {
            bool isValid;

            // Creating the PrincipalContext
            PrincipalContext pc = null;
            
            try
            {
                pc = new PrincipalContext(ContextType.Domain, DomainName, usrPassToCheck,
                                          ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing);
            }
            catch 
            {
                pc.Dispose();
                //MessageBox.Show("Failed to create PrincipalContext. \r\n Exception: " + e.Message);
                _strMsg = ".הסיסמה אינה נכונה או פג תוקפה " + Environment.NewLine + "בכדי להחליף את הסיסמה, נא להתנתק" + Environment.NewLine + ".מהמחשב ולהיכנס מחדש";
                _toLog = _strMsg;
                return false;
            }

            ADactions _adActions = new ADactions(m_Silent);

            try
            {
                // Check if user account exist
                if (!_adActions.IsUserExist(DomainName, usrNameToCheck))
                {
                    _strMsg = _adActions.StrMsg;
                    _toLog = _adActions.ToLog;
                    return false;
                }

                // validate the credentials
                isValid = pc.ValidateCredentials(_adActions.userLogonName, usrPassToCheck,
                    ContextOptions.Negotiate | ContextOptions.Signing |
                    ContextOptions.Sealing);

                if (!isValid)
                {
                    bool IsAccountLockedOut = false;

                    // Get Password Last Set Date
                    //_adActions.GetPasswordLastSetDate(usrDomain, usrNameToCheck);
                    //_pwdLastSetDate = _adActions.PwdLastSetDate;

                    try
                    {
                        IsAccountLockedOut = _adActions.CheckIsAccountLockedOut(usrDomain, usrNameToCheck);
                    }
                    catch 
                    {
                        isValid = false;
                        //MessageBox.Show("Error New: " + e.Message);
                        return isValid;
                    }
                    

                    if (!IsAccountLockedOut)
                    {
                        //doRefreshPassword(usrDomain, usrNameToCheck);

                        _strMsg = ".הסיסמה אינה נכונה או פג תוקפה " + Environment.NewLine + "בכדי להחליף את הסיסמה, נא להתנתק" + Environment.NewLine + ".מהמחשב ולהיכנס מחדש";
                        _toLog = _strMsg;

                    }
                    else
                    {
                        _strMsg = "חשבון המשתמש נעול.\r\n נא פנה לתמיכה לקבלת עזרה";
                        _toLog = _strMsg;
                    }
                }
            }
            catch 
            {
                isValid = false;
                //MessageBox.Show("Error: " + e.Message);
                return isValid;
            }
            finally
            {
                _adActions.Dispose();
                pc.Dispose();
            }
            return isValid;
        }

        internal bool doRefreshPassword(string strDomain,string strUserToRefresh)
        {
            return impersonateUser(_usrDomain, "xxxxx", "xxxxx",strUserToRefresh);

        }


        private bool impersonateUser(string impersonateUserDomain, string impersonateUserName, string impersonateUserPass, string strUserToRefresh)
        {
            bool IsRefreshPassSucceeded = false;

            // Start with current loggedin user
            //MessageBox.Show("Current user: " + WindowsIdentity.GetCurrent().Name);

            // Do Impersonation to another user
            WrapperImpersonationContext newContext = new WrapperImpersonationContext(impersonateUserDomain, impersonateUserName, impersonateUserPass);
            
            newContext.Enter();

            // Execute code under other uses context
            //MessageBox.Show("Current user: " + WindowsIdentity.GetCurrent().Name);

            using (ADactions _adActions = new ADactions(m_Silent))
            {
                IsRefreshPassSucceeded = _adActions.RefreshExpiredPassword(impersonateUserDomain, strUserToRefresh);

                if (IsRefreshPassSucceeded)
                {
                    using (SelfChangePassword selfChange = new SelfChangePassword(strUserToRefresh))
                    {
                        selfChange.userSamAccountName = strUserToRefresh;
                        if (selfChange.Visible == false)
                        {
                            selfChange.ShowDialog();
                            if (_passChangeSucc)
                            {
                                IsRefreshPassSucceeded = true;
                            }
                            else
                            {
                                IsRefreshPassSucceeded = false;
                            }
                        }
                    }

                }
                else
                {
                    _strMsg = ".הסיסמה אינה נכונה או פג תוקפה " + Environment.NewLine + "בכדי להחליף את הסיסמה, נא להתנתק" + Environment.NewLine + ".מהמחשב ולהיכנס מחדש";
                    _toLog = _strMsg;
                }

                //MessageBox.Show("Error Message: " + newContext.StrMsg);
                // Stop Imersonation and return to loggedin user's credentials
                newContext.Leave();
            }

            // Show current user context
            //MessageBox.Show("Current user: " + WindowsIdentity.GetCurrent().Name);

            return IsRefreshPassSucceeded;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
