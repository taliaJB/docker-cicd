using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eldan.LoggerBase
{
    public class EldanError : Exception
    {
        public EldanError(Exception Execption)
            : base(Execption.Message, Execption)
        {
            Init();
        }

        private void Init()
        {
            m_EldanErrorID = new Random().Next(100000000, 999999999);
        }
        
        private int m_EldanErrorID;

        public int EldanErrorID
        {
            get { return m_EldanErrorID; }
            set { m_EldanErrorID = value; }
        }
        
        public override string ToString()
        {
            return base.Message + " (Eldan Error ID: " + m_EldanErrorID.ToString() + ")";
        }

        public string GetFullMessage()
        {
            return ToString();
        }

        public string GetShortMessage()
        {
            return string.Format("Eldan exception (Eldan error ID: {0}) for more details, address Eldan's IT", m_EldanErrorID.ToString());
        }

        public string GetOriginalMessage()
        {
            return base.Message;
        }

        public Exception GetExtendedException(bool AddEldanIDInErrorMessage = true)
        {
            return new Exception(AddEldanIDInErrorMessage ? GetFullMessage() : GetOriginalMessage(), GetOriginalException()); 
        }

        public Exception GetShortException()
        {
            return new Exception(GetShortMessage());
        }

        public Exception GetOriginalException()
        {
            return base.InnerException;
        }
    }
}
