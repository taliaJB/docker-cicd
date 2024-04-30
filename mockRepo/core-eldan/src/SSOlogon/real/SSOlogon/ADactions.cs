using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Forms;
using System.Security.Principal;

namespace Eldan.SSOlogon
{
    public class ADactions : IDisposable
    {
        private PrincipalContext pc;

        #region Define Private Fields
        private string _domain = "";            // get the Full Domain Name (eldan.co.il)
        private string _sRootDomain = "";       // Get the Full Distinguished Domain Name (DC=eldan,DC=co,DC=il)k
        private string _domainSvr = "";         // get the Domain Server Name (dcsrv2k3)
        private string _currentUser = "";
        private string _usrLogonName = "";
        private string _usrSamAccountName = "";
        private string _strUsrPwd = "";
        private string _usrFirstName = "";
        private string _usrLastName = "";
        private string _usrFullName = "";
        private string _strUsrFDN = "";
        private string _strUsrOU = "";
        private string _toLog = "";
        private string _strMsg = "";
        private DateTime _pwdLastSetDate;
        private bool disposed = false; // to detect redundant calls
        #endregion


        #region Define Public Properties
        public string DomainName        // get the Full Domain Name (eldan.co.il)
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string sRootDomain       // Get the Full Distinguished Domain Name (DC=eldan,DC=co,DC=il)
        {
            get { return _sRootDomain; }
            set { _sRootDomain = value; }
        }

        public string DomainSvr         // get the Domain Server Name (dcsrv2k3)
        {
            get { return _domainSvr; }
            set { _domainSvr = value; }
        }

        public string CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        public string userLogonName
        {
            get { return _usrLogonName; }
            set { _usrLogonName = value; }
        }

        public string UserSamAccountName
        {
            get { return _usrSamAccountName; }
            set { _usrSamAccountName = value; }
        }

        public string userPwd
        {
            get { return _strUsrPwd; }
            set { _strUsrPwd = value; }
        }

        public string usrFirstName
        {
            get { return _usrFirstName; }
            set { _usrFirstName = value; }
        }

        public string usrLastName
        {
            get { return _usrLastName; }
            set { _usrLastName = value; }
        }

        public string FullName
        {
            get { return _usrFullName; }
            set { _usrFullName = value; }
        }

        public string usrOU
        {
            get { return _strUsrOU; }
            set { _strUsrOU = value; }
        }

        public string ToLog
        {
            get { return _toLog; }
            set { _toLog = value; }
        }

        public string StrMsg
        {
            get { return _strMsg; }
            set { _strMsg = value; }
        }

        public DateTime PwdLastSetDate
        {
            get { return _pwdLastSetDate; }
            set { _pwdLastSetDate = value; }
        }
        #endregion

        // Constructor
        // Find current Domain name
        public ADactions(): this(false)
        {
        }

        private bool m_Silent;

        public ADactions(bool Silent)
        {
            m_Silent = Silent;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                _currentUser = identity.Name;
            }

            try
            {
                _domain = Domain.GetCurrentDomain().Name.ToString();
                _domainSvr = Domain.GetCurrentDomain().FindDomainController().Name.ToString();
                _domainSvr = _domainSvr.Substring(0, _domainSvr.IndexOf("."));
            }
            catch (Exception ex)
            {
                _strMsg = "תקלה בחיבור לדומיין \r\n";
                _toLog = _strMsg + ex.Message;
                if (!m_Silent)
                    MessageBox.Show(_strMsg);

            }

            // Find Root Domain naming context (Exapmle: DC=sasson, DC=co, DC=il)
            using (DirectoryEntry deRootDSE = new DirectoryEntry("GC://RootDSE"))
            {
                _sRootDomain = deRootDSE.Properties["rootDomainNamingContext"].Value.ToString();
            }
        }

        #region Change user's password
        public bool ChangePassword(string domainName, string userName)
        {
            bool bSuccess = false;

            using ( pc = CreatePrincipalContext(domainName))
            {
                //// Check if user object already exists in the store
                // userName == SamAccountName == Logon name
                using (UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName))
                {
                    try
                    {
                        if (usrPrincipal == null)
                        {
                            _strMsg = "תקלה: המשתמש  " + userName + " == אינו קיים. \r\n";
                            _toLog = _strMsg;
                            if (!m_Silent)
                                MessageBox.Show(_strMsg);
                            return false;
                        }
                        else
                        {
                            //// Get user's full distinguish name
                            _strUsrFDN = usrPrincipal.DistinguishedName;

                            if (_strUsrPwd != "" && _strUsrPwd.Length > 0)
                            {
                                usrPrincipal.SetPassword(_strUsrPwd);
                                usrPrincipal.Save();
                                usrPrincipal.ExpirePasswordNow();
                            }
                            bSuccess = true;
                        }
                    }
                    catch (Exception e)
                    {
                        _strMsg = "תקלה בהחלפת הסיסמא. ";
                        _toLog = _strMsg + "\r\n" + e.Message;
                        if (!m_Silent)
                            MessageBox.Show(_strMsg);

                        bSuccess = false;
                    }
                }
            }

            return bSuccess;
        }
        #endregion

        #region Self Service to Change Own password
        public bool ChangeSelfPassword(string oldPassword, string newPassword)
        {
            bool bSuccess = false;

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, _currentUser))
                    {
                        user.ChangePassword(oldPassword, newPassword);
                        bSuccess = true;
                    }
                }
            }
            catch (Exception e)
            {
                _strMsg = "תקלה בהחלפת הסיסמא. \r\n";
                _toLog = _strMsg + e.Message;
                if (!m_Silent)
                    MessageBox.Show(_strMsg);

                bSuccess = false;
            }

            return bSuccess;
        }
        #endregion

        #region Check if user Account exist
        public bool IsUserExist(string domainName, string userName)
        {
            bool bSuccess = true;

            using ( pc = CreatePrincipalContext(domainName))
            {
                //// Check if user object already exists in the store
                using (UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName))
                {
                    try
                    {
                        if (usrPrincipal == null)
                        {
                            _strMsg = " אינו קיים " + userName + " המשתמש ";
                            _toLog = _strMsg;
                            //MessageBox.Show(_strMsg);
                            bSuccess = false;
                        }
                        else
                        {
                            _usrLogonName = usrPrincipal.SamAccountName;
                        }
                    }
                    catch (Exception e)
                    {
                        _strMsg = "תקלה בניסיון לבדוק אם המשתמש נעול.";
                        _toLog = _strMsg + "\r\n" + e.Message;
                        if (!m_Silent)
                            MessageBox.Show(_strMsg);

                        bSuccess = false;
                    }
                }
            }

            return bSuccess;
        }
        #endregion


        #region Check if user's Account is LockedOut
        public bool CheckIsAccountLockedOut(string domainName, string userName)
        {
            bool bSuccess = false;

            pc = CreatePrincipalContext(domainName);

            try
            {
                //// Check if user object already exists in the store
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName);

                if (usrPrincipal == null)
                {
                    _strMsg = "שגיאה: המשתמש " + userName + " == אינו קיים \r\n";
                    _toLog = _strMsg;
                    //MessageBox.Show(_strMsg);
                    return false;
                }
                else
                {
                    if (usrPrincipal.IsAccountLockedOut())
                    {
                        _strMsg = "חשבון המשתמש נעול. נא להתקשר לתמיכה.";
                        _toLog = _strMsg;
                        bSuccess = true;
                    }
                    else
                    {
                        // המשתמש אינו נעול
                        bSuccess = false;
                    }
                }
                usrPrincipal.Dispose();
            }
            catch (Exception )
            {
                //_strMsg = "תקלה בניסיון לבדוק אם המשתמש נעול. ";
                //_toLog = _strMsg + "\r\n" + e.Message;
                //if (!m_Silent)
                //    MessageBox.Show(_strMsg);

                bSuccess = false;
            }
            finally
            {
                pc.Dispose();
            }

            return bSuccess;
        }
        #endregion

        #region Refresh Expired Password
        public bool RefreshExpiredPassword(string domainName, string userName)
        {
            bool bSuccess = false;

            using ( pc = CreatePrincipalContext(domainName))
            {
                //// Check if user object already exists in the store
                using (UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName))
                {
                    try
                    {
                        if (usrPrincipal == null)
                        {
                            _strMsg = "שגיאה: המשתמש " + userName + " == אינו קיים \r\n";
                            _toLog = _strMsg;
                            if (!m_Silent)
                                MessageBox.Show(_strMsg);

                            //return false;
                        }
                        else
                        {
                            try
                            {
                                DateTime datePwdLastSet;
                                datePwdLastSet = usrPrincipal.LastPasswordSet.Value;

                                usrPrincipal.RefreshExpiredPassword();
                                bSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                                _strMsg = "תקלה בניסיון להאריך את תוקף הסיסמא.";
                                _toLog = _strMsg + "\r\n" + ex.Message;
                                if (!m_Silent)
                                    MessageBox.Show(_strMsg);

                                bSuccess = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _strMsg = "תקלה בניסיון להאריך את תוקף הסיסמא.";
                        _toLog = _strMsg + "\r\n" + e.Message;
                        if (!m_Silent)
                            MessageBox.Show(_strMsg);

                        bSuccess = false;
                    }
                }
            }

            return bSuccess;
        }
        #endregion

        #region Get Date of Password last set
        public bool GetPasswordLastSetDate(string domainName, string userName)
        {
            bool bSuccess = false;

            using ( pc = CreatePrincipalContext(domainName))
            {
                //// Check if user object already exists in the store
                using (UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName))
                {
                    try
                    {
                        if (usrPrincipal == null)
                        {
                            if (!m_Silent)
                                MessageBox.Show("שגיאה: המשתמש " + userName + " == אינו קיים \r\n");

                            //return false;
                        }
                        else
                        {
                            try
                            {
                                _pwdLastSetDate = usrPrincipal.LastPasswordSet.Value;
                                _strMsg = "תאריך האחרון בו הוחלפה הסיסמא: " + _pwdLastSetDate;
                                _toLog = _strMsg;
                                //MessageBox.Show(_strMsg);
                                bSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                _strMsg = ex.Message;
                                _toLog = _strMsg;
                                //MessageBox.Show(_strMsg);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _strMsg = "תקלה בניסיון לבדוק את תאריך החלפת הסיסמא.";
                        _toLog = _strMsg + "\r\n" + e.Message;
                        if (!m_Silent)
                            MessageBox.Show(_strMsg);
                        bSuccess = false;
                    }
                }
            }

            return bSuccess;
        }
        #endregion

        #region Create PrincipalContext
        //// Creating the PrincipalContext("sasson.co.il")
        internal PrincipalContext CreatePrincipalContext(string domainName)
        {
            try
            {
                //For Example: pc = new PrincipalContext(ContextType.Domain, "sasson.co.il");
                pc = new PrincipalContext(ContextType.Domain, domainName);
            }
            catch (Exception e)
            {
                _strMsg = "Failed to create PrincipalContext. Exception: " + e.Message;
                _toLog = _strMsg;
                if (!m_Silent)
                    MessageBox.Show(_strMsg);
            }
            //finally
            //{
            //    pc.Dispose();
            //}
            return pc;
        }


        //// Creating the PrincipalContext white Full Path ("sasson.co.il", "OU=TestOU,DC=SASSON,DC=CO,DC=IL")
        internal PrincipalContext CreatePrincipalContext(string domainName, string fullPath)
        {
            try
            {
                //For Example: pc = new PrincipalContext(ContextType.Domain, "sasson.co.il", "OU=TestOU,DC=SASSON,DC=CO,DC=IL");
                pc = new PrincipalContext(ContextType.Domain, domainName, fullPath);
            }
            catch (Exception e)
            {
                _strMsg = "Failed to create PrincipalContext. Exception: " + e.Message;
                _toLog = _strMsg;
                if (!m_Silent)
                    MessageBox.Show(_strMsg);
            }
            return pc;
        }
        //// Creating the PrincipalContext
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (pc != null)
                    {
                        pc.Dispose();
                    }
                }

                disposed = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
