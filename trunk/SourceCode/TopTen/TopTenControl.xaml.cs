using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace TopTen
{
    public partial class TopTenControl : UserControl
    {
        #region consts & pseudo consts
        private const string FILTER_BY_MES_REPL = "Total messages replied";
        private const string FILTER_BY_MES_SENT = "Total messages sent";
        private const string FILTER_BY_MES_RECV = "Total messages received";
        private static string[] comboBoxSelections = new string[] { FILTER_BY_MES_SENT, FILTER_BY_MES_RECV, FILTER_BY_MES_REPL };
        #endregion

        #region private vars
        private ObservableCollection<UserInfo> userList = null;
        private bool triggerFlag = true;
        #endregion

        #region getters & setters, with additonal functionality
        public ObservableCollection<UserInfo> UserList
        {
            get { return this.userList; }
            set
            {
                this.userList = value;
                if (comboBox1.SelectedIndex > -1 && userList != null)
                {
                    if (comboBox1.SelectedItem.Equals(FILTER_BY_MES_REPL))
                    {
                        sortTopTenByRepl();
                    }
                    else if (comboBox1.SelectedItem.Equals(FILTER_BY_MES_SENT))
                    {
                        sortTopTenBySent();
                    }
                    else if (comboBox1.SelectedItem.Equals(FILTER_BY_MES_RECV))
                    {
                        sortTopTenByRecv();
                    }
                }
                // get the top ten of users to show on data grid
                ObservableCollection<UserInfo> tmpColl = new ObservableCollection<UserInfo>();
                int smallerNum = 10;
                if (smallerNum > userList.Count)
                {
                    smallerNum = userList.Count;
                }
                for (int i = 0; i < smallerNum; i++)
                {
                    tmpColl.Add(userList[i]);
                }
                dataGrid1.ItemsSource = tmpColl;
            }
        }
        #endregion

        #region private functions
        private void sortTopTenByRepl()
        {
            //In this special case, bubble sort will have the best speed and complexity will always be 10*n = n
            int userListSize = userList.Count;
            int outerSize = 10;
            if (outerSize > userListSize)
            {
                outerSize = userListSize;
            }
            for (int i = 0; i < outerSize; i++)
            {
                for (int j = userListSize - 1; j > i; j--)
                {
                    if (userList[j].NumMessagesRepl > userList[j - 1].NumMessagesRepl)
                    {
                        UserInfo tmp = userList[j];
                        userList[j] = userList[j - 1];
                        userList[j - 1] = tmp;
                    }
                }
            }
        }
        private void sortTopTenBySent()
        {
            //In this special case, bubble sort will have the best speed and complexity will always be 10*n = n
            int userListSize = userList.Count;
            int outerSize = 10;
            if (outerSize > userListSize)
            {
                outerSize = userListSize;
            }
            for (int i = 0; i < outerSize; i++)
            {
                for (int j = userListSize - 1; j > i; j--)
                {
                    if (userList[j].NumMessagesSent > userList[j - 1].NumMessagesSent)
                    {
                        UserInfo tmp = userList[j];
                        userList[j] = userList[j - 1];
                        userList[j - 1] = tmp;
                    }
                }
            }
        }
        private void sortTopTenByRecv()
        {
            //In this special case, bubble sort will have the best speed and complexity will always be 10*n = n
            int userListSize = userList.Count;
            int outerSize = 10;
            if (outerSize > userListSize)
            {
                outerSize = userListSize;
            }
            for (int i = 0; i < outerSize; i++)
            {
                for (int j = userListSize - 1; j > i; j--)
                {
                    if (userList[j].NumMessagesRecv > userList[j - 1].NumMessagesRecv)
                    {
                        UserInfo tmp = userList[j];
                        userList[j] = userList[j - 1];
                        userList[j - 1] = tmp;
                    }
                }
            }
        }
        #endregion

        public TopTenControl()
        {
            InitializeComponent();
            foreach (string tmpStr in comboBoxSelections) {
                this.comboBox1.Items.Add(tmpStr);
            }
            this.triggerFlag = false;
            this.comboBox1.SelectedIndex = 0;
            this.triggerFlag = true;
            
            #region debug code
            //ObservableCollection<UserInfo> tmp = new ObservableCollection<UserInfo>();
            //for (int i = 0; i < 6; i++)
            //{
            //    UserInfo tmpUI = new UserInfo(i + "@gmail.com", i % 5, i, i*2);
            //    tmp.Add(tmpUI);
            //}
            //this.UserList = tmp;
            #endregion
        }

        #region event handlers
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (triggerFlag)
            {
                this.UserList = userList; // refresh the datagrid
            }
        }
        #endregion
    }
}
