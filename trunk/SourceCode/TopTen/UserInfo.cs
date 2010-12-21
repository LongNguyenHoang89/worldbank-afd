using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace TopTen
{
    public class UserInfo
    {
        #region private variables
        private string emailAddress;
        private int numMessagesSent;
        private int numMessagesRecv;
        private int numMessagesRepl;
        #endregion
        #region getters & setters
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }
        public int NumMessagesRecv
        {
            get { return numMessagesRecv; }
            set { numMessagesRecv = value; }
        }
        public int NumMessagesSent
        {
            get { return numMessagesSent; }
            set { numMessagesSent = value; }
        }
        public int NumMessagesRepl
        {
            get { return numMessagesRepl; }
            set { numMessagesRepl = value; }
        }
        public int NumMessagesSentAndReceived
        {
            get { return numMessagesRecv + numMessagesSent + numMessagesRepl; }
        }
        #endregion

        public UserInfo(string emailAddr, int messagesSentNum, int messagesRecvNum, int messagesReplNum)
        {
            this.emailAddress = emailAddr;
            this.numMessagesSent = messagesSentNum;
            this.numMessagesRecv = messagesRecvNum;
            this.numMessagesRepl = messagesReplNum;
        }
    }
}
