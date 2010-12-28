
﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Visifire.Charts;

namespace PersonalInfoAndStatistics
{
    public partial class PersonalStatisticsControl : UserControl
    {
        #region constants
        private const int INVALID_NUM = -1; // Used as default values to indicate a variable hasn't been set
        private const string MESS_SENT_CAPTION = "Messages sent";
        private const string MESS_RECV_CAPTION = "Messages received";
        #endregion

        #region variable
        // variable for general information
        // variables used to create the graph
        private List<DateTime> allMessagesSentTime = null;
        private List<DateTime> allMessagesReceivedTime = null;
        // the graphs & their variables
        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private Chart messagesSentChart = null;
        private DataPoint[] messagesSentDataPoints = null;
        private Chart messagesRecvChart = null;
        private DataPoint[] messagesRecvDataPoints = null;
        private bool shouldTrigger = true; // the flag to decide if combo box change event should trigger or not
        #endregion

        private void extractYear(List<DateTime> input)
        {
            List<int> years = new List<int>();
            foreach (object o in comboBox1.Items)
            {
                years.Add(Convert.ToInt32(o));
            }
            foreach (DateTime dateTime in input)
            {
                if (!years.Contains(dateTime.Year))
                {
                    years.Add(dateTime.Year);
                }
            }
            years.Sort();
            comboBox1.Items.Clear();
            foreach (int i in years)
            {
                comboBox1.Items.Add(i);
            }

            shouldTrigger = false; // change flag so the event won't trigger
            if (comboBox1.Items[0] != null) // select the first choice
            {
                comboBox1.SelectedIndex = 0;
            }
            shouldTrigger = true; // restore the flag
        }

        #region getters & setters, but with more functionality (refresh data on update)

        private int Year
        {
            get {
                try
                {
                    return Convert.ToInt32(comboBox1.SelectedValue.ToString());
                }
                catch (FormatException)
                {
                    return -1;
                }
                catch (NullReferenceException)
                {
                    return -1;
                }

            }
            set {
                // clear the graph current info
                clearRecvChart();
                clearSentChart();
                // refresh the graph
                processMessageTime(allMessagesSentTime, messagesSentDataPoints);
                //processMessageTime(allMessagesReceivedTime, messagesRecvDataPoints);
            }
        }

        public string EmailAddr
        {
            get { return labelEmailAddrVal.Content.ToString(); }
            set {
                labelEmailAddrVal.Content = value;
            }
        }
        public string MemberName
        {
            get { return labelNameVal.Content.ToString(); }
            set {
                labelNameVal.Content = value;
            }
        }
        public int MessagesSent
        {
            get
            {
                try
                {
                    return Convert.ToInt32(labelTotalMessSentVal.Content.ToString());
                }
                catch (Exception) // Can only be number format exception
                {
                    return 0;
                }
            }
            set {
                labelTotalMessSentVal.Content = value;
            }
        }
        
        public List<DateTime> AllMessagesSentTime
        {
            get { return allMessagesSentTime; }
            set {
                allMessagesSentTime = value; // do the normal task of a setter
                extractYear(allMessagesSentTime); // get the years availables in the messages
                // then refresh the graph
                if (messagesSentChart == null) // make sure it's not null
                {
                    messagesSentChart = new Chart();
                    // set the caption of the YAxis
                    Axis YAxis = new Axis();
                    YAxis.Title = MESS_SENT_CAPTION;
                    messagesSentChart.AxesY.Add(YAxis);
                    // init the data
                    DataSeries dataSeries = new DataSeries();
                    messagesSentChart.Series.Add(dataSeries);
                    messagesSentDataPoints = new DataPoint[12];
                    DataPoint tmpDP;
                    for (int i = 0; i < 12; i++)
                    {
                        messagesSentDataPoints[i] = new DataPoint();
                        tmpDP = messagesSentDataPoints[i];
                        tmpDP.YValue = 0;
                        tmpDP.AxisXLabel = months[i];
                        tmpDP.XValue = i + 1;
                        dataSeries.DataPoints.Add(tmpDP);
                    }
                    // Add the chart to the control
                    chart1.Children.Add(messagesSentChart);
                }
                else
                {
                    clearSentChart();
                }
                // refresh values of the chart
                processMessageTime(allMessagesSentTime, messagesSentDataPoints);
            }
        }

        //public List<DateTime> AllMessagesReceivedTime
        //{
        //    get { return allMessagesReceivedTime; }
        //    set {
        //        allMessagesReceivedTime = value; // do the normal task of a setter
        //        extractYear(allMessagesReceivedTime); // get the years availables in the messages
        //        // then refresh the graph
        //        if (messagesRecvChart == null) // make sure it's not null
        //        {
        //            messagesRecvChart = new Chart();
        //            // set the caption of the YAxis
        //            Axis YAxis = new Axis();
        //            YAxis.Title = MESS_RECV_CAPTION;
        //            messagesRecvChart.AxesY.Add(YAxis);
        //            // init the data
        //            DataSeries dataSeries = new DataSeries();
        //            messagesRecvChart.Series.Add(dataSeries);
        //            messagesRecvDataPoints = new DataPoint[12];
        //            messagesRecvDataPoints = new DataPoint[12];
        //            DataPoint tmpDP;
        //            for (int i = 0; i < 12; i++)
        //            {
        //                messagesRecvDataPoints[i] = new DataPoint();
        //                tmpDP = messagesRecvDataPoints[i];
        //                tmpDP.YValue = 0;
        //                tmpDP.AxisXLabel = months[i];
        //                tmpDP.XValue = i + 1;
        //                dataSeries.DataPoints.Add(tmpDP);
        //            }
        //            // Add the chart to the control
        //            chart2.Children.Add(messagesRecvChart);
        //        }
        //        else
        //        {
        //            clearRecvChart();
        //        }
        //        // refresh values of the chart
        //        processMessageTime(allMessagesReceivedTime, messagesRecvDataPoints);
        //    }
        //}
        #endregion

        #region private functions
        private void processMessageTime(List<DateTime> dateTimeList, DataPoint[] dataPoints)
        {
            foreach (DateTime tmpDateTime in dateTimeList)
            {
                if (tmpDateTime.Year == Year)
                {
                    dataPoints[tmpDateTime.Month - 1].YValue++;
                }
            }
        }

            #region functions to clear data
            private void clearSentChart()
            {
                if (messagesSentChart != null)
                {
                    foreach (DataPoint tmpDP in messagesSentChart.Series[0].DataPoints)
                    {
                        tmpDP.YValue = 0;
                    }
                }
            }
            private void clearRecvChart()
            {
                if (messagesRecvChart != null)
                {
                    foreach (DataPoint tmpDP in messagesRecvChart.Series[0].DataPoints)
                    {
                        tmpDP.YValue = 0;
                    }
                }
            }
            #endregion
        #endregion

        public PersonalStatisticsControl()
        {
            InitializeComponent();

            #region populate years to the combo box

            #endregion

            #region debug code to create dummy data
            //List<DateTime> tmp1 = new List<DateTime>();
            //List<DateTime> tmp2 = new List<DateTime>();

            //Random random = new Random();
            //for (int i = 0; i < 50; i++)
            //{
            //    int randomMonth = random.Next(12);
            //    int randomDay = random.Next(28);
            //    tmp1.Add(new DateTime(2010, randomMonth + 1, randomDay + 1));
            //}
            //for (int i = 0; i < 50; i++)
            //{
            //    int randomMonth = random.Next(12);
            //    int randomDay = random.Next(28);
            //    tmp2.Add(new DateTime(2010, randomMonth + 1, randomDay + 1));
            //}
            //AllMessagesReceivedTime = tmp1;
            //AllMessagesSentTime = tmp2;
            #endregion

        }

        #region event handler

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedItem != null && shouldTrigger)
            {
                this.Year = Convert.ToInt32(comboBox1.SelectedItem);
            }
        }

        #endregion
    }
}