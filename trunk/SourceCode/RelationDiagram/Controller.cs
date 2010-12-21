using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Net;
using NCRVisual.RelationDiagram.Algo;
using System.Xml;

namespace NCRVisual.RelationDiagram
{
    public class DiagramController
    {
        #region constant
        string INPUT_FILE_NAME = "..\\Output\\";
        string ID_TAG = "UserId";
        string NAME_TAG = "Name";
        string EMAIL_TAG = "Email";
        string VERTEX_TAG = "Vertex";
        string EDGE_TAG = "Edge";
        string START_EDGE_TAG = "Start";
        string END_EDGE_TAG = "End";
        string EDGE_VALUE_TAG = "Value";
        string EDGE_CONTENT_TAG = "Content";
        string EDGE_DATE_TAG = "Date";
        string EDGE_SUBJECT_TAG = "Subject";

        #endregion

        #region private
        private int[][] _input = new int[100][];
        private Collection<Email> EmailList = new Collection<Email>();
        private Collection<Email> unreadEmailList = new Collection<Email>();
        #endregion

        #region properties
        /// <summary>
        /// Event after reading input from data provider
        /// </summary>
        public event EventHandler InputReadingComplete;

        /// <summary>
        /// Get or set the Entity Collection
        /// </summary>
        public Collection<MailListEntity> entityCollection { get; set; }

        /// <summary>
        /// Number of vertex
        /// </summary>
        public int VertexNumber = 0;

        /// <summary>
        /// The Maximum Number of a connection
        /// </summary>
        public int MaxSingleConnection { get; set; }

        /// <summary>
        /// The Minimum Number of a connection
        /// </summary>
        public int MinSingleConnection { get; set; }

        #endregion

        public Point TopLeft;
        public Point LowRight;

        /// <summary>
        /// Create new instance of Diagram controller
        /// </summary>
        public DiagramController(string inputFileName)
        {
            entityCollection = new Collection<MailListEntity>();
            MaxSingleConnection = 0;

            this.INPUT_FILE_NAME += inputFileName;

            //getfile from output.txt
            WebClient client = new WebClient();
            client.OpenReadAsync(new Uri(INPUT_FILE_NAME, UriKind.Relative));
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            using (XmlReader reader = XmlReader.Create(e.Result))
            {
                ReadRelation(reader);
            }

            if (this.InputReadingComplete != null)
            {
                this.InputReadingComplete(null, null);
            };
        }

        private void populateConnection(int[][] matrixInput)
        {
            MaxSingleConnection = 0;
            MinSingleConnection = 10000;
            for (int i = 0; i < VertexNumber; i++)
            {
                int reply=0;
                int receive=0;
                for (int j = 0; j < VertexNumber; j++)
                {
                    if (i!=j)
                    {
                        reply=reply+matrixInput[i][j];
                        receive=receive+matrixInput[j][i];
                    }
                    if (matrixInput[i][j] > 0)
                    {
                        entityCollection[i].Connections.Add(new Connection(entityCollection[i], entityCollection[j], matrixInput[i][j]));
                    }

                    if (matrixInput[i][j] + matrixInput[j][i] > MaxSingleConnection)
                    {
                        MaxSingleConnection = matrixInput[i][j] + matrixInput[j][i];
                    }

                    if (matrixInput[i][j] + matrixInput[j][i] < MinSingleConnection)
                    {
                        MinSingleConnection = matrixInput[i][j] + matrixInput[j][i];
                    }
                }
                entityCollection[i].NumMessagesSent = matrixInput[i][i];
                entityCollection[i].NumMessagesRecv = receive;
                entityCollection[i].NumMessagesRepl = reply;

            }

            foreach (Email mail in this.EmailList)
            {
                foreach (IConnection con in entityCollection[int.Parse(mail.UserId) - 1].Connections)
                {
                    if (con.Destination != null)
                    {
                        if (con.Destination.Id == mail.SendTo)
                        {
                            ((Connection)con).SendDate.Add(mail.sendDate);
                        }
                    }
                }
            }

            foreach (Email m in this.unreadEmailList)
            {
                foreach (IConnection con in entityCollection[int.Parse(m.SendTo) - 1].Connections)
                {
                    if (con.Destination == null)
                    {
                        ((Connection)con).SendDate.Add(m.sendDate);
                    }
                }
            }
        }

        void ReadRelation(XmlReader reader)
        {
            reader.MoveToContent();
            int count = 0;

            //Start reading xml
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == VERTEX_TAG)
                {
                    MailListEntity entity = new MailListEntity();

                    //Read Userid
                    reader.ReadToFollowing(ID_TAG);
                    string id = reader.ReadElementContentAsString();

                    //Read Email
                    reader.ReadToFollowing(EMAIL_TAG);
                    string email = reader.ReadElementContentAsString();

                    //Read Name
                    reader.ReadToFollowing(NAME_TAG);
                    string name = reader.ReadElementContentAsString();

                    entity.Id = id;
                    entity.Email = email;
                    entity.Name = email;

                    if (id.Equals("0"))
                    {
                        while (reader.ReadToNextSibling(EDGE_TAG))
                        {
                            reader.ReadToFollowing(START_EDGE_TAG);
                            int start = reader.ReadElementContentAsInt();
                            reader.ReadToFollowing(END_EDGE_TAG);
                            int end = reader.ReadElementContentAsInt();
                            reader.ReadToFollowing(EDGE_VALUE_TAG);
                            int value = reader.ReadElementContentAsInt();
                            //_input[start - 1][end - 1] = value;

                            while (reader.ReadToNextSibling(EDGE_CONTENT_TAG))
                            {
                                reader.ReadToFollowing(EDGE_DATE_TAG);
                                string time = reader.ReadElementContentAsString();
                                reader.ReadToFollowing(EDGE_SUBJECT_TAG);
                                string subject = reader.ReadElementContentAsString();
                                this.unreadEmailList.Add(
                                    new Email
                                    {
                                        MessageSubject = subject,
                                        UserId = start.ToString(),
                                        sendDate = toDateTime(time),
                                        SendTo = end.ToString()
                                    }
                                    );

                                reader.Read();
                            }
                            reader.Read();
                        }
                        //count++;
                    }
                    else
                    {
                        _input[count] = new int[100];
                        //Build matrix for connection                        
                        while (reader.ReadToNextSibling(EDGE_TAG))
                        {
                            reader.ReadToFollowing(START_EDGE_TAG);
                            int start = reader.ReadElementContentAsInt();
                            reader.ReadToFollowing(END_EDGE_TAG);
                            int end = reader.ReadElementContentAsInt();
                            reader.ReadToFollowing(EDGE_VALUE_TAG);
                            int value = reader.ReadElementContentAsInt();                            
                            _input[start - 1][end - 1] = value;

                            while (reader.ReadToNextSibling(EDGE_CONTENT_TAG))
                            {
                                reader.ReadToFollowing(EDGE_DATE_TAG);
                                string time = reader.ReadElementContentAsString();
                                reader.ReadToFollowing(EDGE_SUBJECT_TAG);
                                string subject = reader.ReadElementContentAsString();
                                this.EmailList.Add(
                                    new Email
                                    {
                                        MessageSubject = subject,
                                        UserId = start.ToString(),
                                        sendDate = toDateTime(time),                                                                                
                                        SendTo = end.ToString(),
                                    }
                                    );

                                reader.Read();
                            }
                            reader.Read();
                        }

                        this.entityCollection.Add(entity);
                        count++;
                    }
                }
            }

            VertexNumber = count;
            populateConnection(_input);            
        }

        public Collection<Point> RunAlgo(IAlgorithm algorithm)
        {
            double maxX = 0;
            double minX = 0;
            double maxY = 0;
            double minY = 0;

            Collection<Point> tempPoints = algorithm.RunAlgo(_input, VertexNumber);

            foreach (Point p in tempPoints)
            {
                if (p.X > maxX)
                {
                    maxX = p.X;
                }

                if (p.X < minX)
                {
                    minX = p.X;
                }

                if (p.Y > maxY)
                {
                    maxY = p.Y;
                }

                if (p.X < minY)
                {
                    minY = p.Y;
                }
            }

            this.TopLeft = new Point(minX, minY);
            this.LowRight = new Point(maxX, maxY);

            return tempPoints;
        }

        public DateTime toDateTime(string input)
        {
            string[] split = input.Split(' ');
            int year = int.Parse(split[3]);
            int Date = int.Parse(split[1]);
            int Month = 1;

            switch (split[2])
            {
                case "Jan": Month = 1; break;
                case "Feb": Month = 2; break;
                case "Mar": Month = 3; break;
                case "Apr": Month = 4; break;
                case "May": Month = 5; break;
                case "Jun": Month = 6; break;
                case "Jul": Month = 7; break;
                case "Aug": Month = 8; break;
                case "Sep": Month = 9; break;
                case "Oct": Month = 10; break;
                case "Nov": Month = 11; break;
                case "Dec": Month = 12; break;
            }

            string[] splitTime = split[4].Split(':');
            int hour = int.Parse(splitTime[0]);
            int min = int.Parse(splitTime[1]);
            int sec = int.Parse(splitTime[2]);

            DateTime dt = new DateTime(year, Month, Date, hour, min, sec, 0);
            return dt;
        }
    }

    /// <summary>
    /// Anonymous class for saving Email
    /// </summary>
    public class Email
    {
        public string UserId { get; set; }
        public DateTime sendDate { get; set; }
        public string MessageSubject { get; set; }
        public string SendTo { get; set; }
    }
}
