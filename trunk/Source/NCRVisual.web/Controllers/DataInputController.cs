using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using NCRVisual.Web.Items;

namespace NCRVisual.Web.Controllers
{
    public class DataInputController
    {

        public Collection<Email> MailList { get; set; }

        public Collection<User> UserList { get; set; }

        public int[][] Relation = new int[1000][];//contain the relationship of vertexes

        public string OutputFileName { get; set; }

        StreamReader reader;

        /// <summary>
        /// the main function of the controller
        /// </summary>
        /// <param name="filePath">file path of input file</param>
        /// <param name="fileName">file name of input file</param>
        public void SolveData(string filePath, string fileName)
        {
            MailList = new Collection<Email>();
            UserList = new Collection<User>();

            for (int i = 0; i < 1000; i++)//init the value of array
            {
                Relation[i] = new int[1000];
            }

            GetDataFromFile(filePath + fileName);//read and solve data , then assign it into relation array
            //WriteDataToFile(filePath + "output.txt");//write data to the ouput file            
            WriteDataToXml(filePath + this.OutputFileName + ".xml");//write data to the  XML file
        }

        /// <summary>
        /// Write the data from array Relation to file; now replace by write data to xml
        /// </summary>
        /// <param name="filePath">the path of output file</param>
        private void WriteDataToFile(String filePath)
        {
            StreamWriter writer = new StreamWriter(filePath);
            for (int i = 1; i <= UserList.Count; i++)
            {
                for (int j = 1; j <= UserList.Count; j++)
                {
                    writer.Write(string.Format("{0,-1:0}", Relation[i][j]) + " ");
                }
                writer.WriteLine();
            }
            writer.Close();
        }

        /// <summary>
        /// Write data in relation to the xml file
        /// The xml file has format
        /// <Relation>
        ///     <Vertex>
        ///         <UserId></UserId>
        ///         <Email></Email>
        ///         <Name></Name>
        ///         <Edge count="1">
        ///             <start></start>
        ///             <end></end>
        ///             <value></value>
        ///             <content count="1">
        ///                 <date></date>
        ///                 <subject></subject>
        ///             </content>
        ///             ..........
        ///             
        ///         </Edge>
        ///         .............
        ///     </Vertex>
        ///     ..............
        /// </relation>
        /// </summary>
        /// <param name="filePath"></param>
        private void WriteDataToXml(String filePath)
        {
            XmlTextWriter writer = new XmlTextWriter(filePath, null);
            writer.WriteStartElement("Relation");

            //write mail that haven't reply //id=0
            {
                writer.WriteWhitespace("\n\t");
                writer.WriteStartElement("Vertex");
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("UserId", "0");
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("Email", "");
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("Name", "");
                int count = 0;
                for (int j = 1; j <= UserList.Count; j++)
                {
                    //List<Email> emailCollection = (from oneemail in MailList where (oneemail.UserTo == 0 && oneemail.UserId == UserList[j - 1].UserId) select oneemail).ToList<Email>();//get email that has the messageid = s[i]
                    List<Email> emailCollection = new List<Email>();
                    foreach (Email oneemail in MailList)
                    {
                        if (oneemail.UserTo == 0 && oneemail.UserId == UserList[j - 1].UserId)
                        {
                            emailCollection.Add(oneemail);
                        }
                    }
                    if (emailCollection != null && emailCollection.Count > 0)
                    {
                        count++;
                        writer.WriteWhitespace("\n\t\t");
                        writer.WriteStartElement("Edge");
                        writer.WriteAttributeString("count", count.ToString());
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("Start", "0");
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("End", UserList[j - 1].UserId.ToString());
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("Value", emailCollection.Count.ToString());
                        for (int k = 0; k < emailCollection.Count; k++)
                        {
                            writer.WriteWhitespace("\n\t\t\t");
                            writer.WriteStartElement("Content");
                            writer.WriteAttributeString("count", k.ToString());
                            writer.WriteWhitespace("\n\t\t\t\t");
                            writer.WriteElementString("Date", emailCollection[k].SendDate);
                            writer.WriteWhitespace("\n\t\t\t\t");
                            writer.WriteElementString("Subject", emailCollection[k].MessageSubject);
                            writer.WriteWhitespace("\n\t\t\t");
                            writer.WriteEndElement();
                        }
                        writer.WriteWhitespace("\n\t\t");
                        writer.WriteEndElement();
                    }
                }
                writer.WriteWhitespace("\n\t");
                writer.WriteEndElement();
            }


            for (int i = 0; i < UserList.Count; i++)
            {
                writer.WriteWhitespace("\n\t");
                writer.WriteStartElement("Vertex");//start element of each vertex
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("UserId", UserList[i].UserId.ToString());
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("Email", UserList[i].Email);
                writer.WriteWhitespace("\n\t\t");
                writer.WriteElementString("Name", UserList[i].Name);

                int count = 0;
                for (int j = 1; j <= UserList.Count; j++)//write relation data beetween vertex i and the others
                {
                    if (Relation[i + 1][j] > 0)//because the relation array begin with index 1
                    {
                        count++;
                        writer.WriteWhitespace("\n\t\t");
                        writer.WriteStartElement("Edge");
                        writer.WriteAttributeString("count", count.ToString());
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("Start", UserList[i].UserId.ToString());
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("End", UserList[j - 1].UserId.ToString());
                        writer.WriteWhitespace("\n\t\t\t");
                        writer.WriteElementString("Value", Relation[i + 1][j].ToString());

                        //email date and email subject of the email which user has sent
                        //List<Email> emailCollection = (from oneemail in MailList where (oneemail.UserId == UserList[i].UserId && oneemail.UserTo == UserList[j - 1].UserId) select oneemail).ToList<Email>();//get email that has the messageid = s[i]
                        List<Email> emailCollection = new List<Email>();
                        foreach (Email oneemail in MailList)
                        {
                            if (oneemail.UserId == UserList[i].UserId && oneemail.UserTo == UserList[j - 1].UserId)
                            {
                                emailCollection.Add(oneemail);
                            }
                        }
                        try
                        {
                            //Collection<Email> emailCollection = (Collection<Email>)emails;
                            for (int k = 0; k < emailCollection.Count; k++)
                            {
                                writer.WriteWhitespace("\n\t\t\t");
                                writer.WriteStartElement("Content");
                                writer.WriteAttributeString("count", k.ToString());
                                writer.WriteWhitespace("\n\t\t\t\t");
                                writer.WriteElementString("Date", emailCollection[k].SendDate);
                                writer.WriteWhitespace("\n\t\t\t\t");
                                writer.WriteElementString("Subject", emailCollection[k].MessageSubject);
                                writer.WriteWhitespace("\n\t\t\t");
                                writer.WriteEndElement();
                            }
                        }
                        catch (Exception e)
                        {
                            e.ToString();
                        }
                        writer.WriteWhitespace("\n\t\t");
                        writer.WriteEndElement();
                    }
                }
                writer.WriteWhitespace("\n\t");
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
        }

        /// <summary>
        /// Just split a string
        /// </summary>
        /// <param name="line">input string</param>
        /// <returns>an array contains words</returns>
        private string[] GetWords(string line)
        {
            line = Regex.Replace(line, "  ", " ");
            return Regex.Split(line, " ");
        }

        /// <summary>
        /// An email has the format *@* 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetEmail(string line)
        {
            string[] s = GetWords(line);
            return s[1] + "@" + s[3];
        }

        /// <summary>
        /// name of user if from the word 4 to the end of string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetName(string[] s)
        {
            string name = "";
            if (s.Length > 4)
            {
                for (int i = 3; i < s.Length; i++)
                {
                    name = name + s[i] + " ";
                }
            }

            //string result = "";
            //int flag = 0;
            //for (int i = 0; i < name.Length; i++)
            //    if (name[i] == '(')
            //    {
            //        flag = 1;
            //    }
            //    else if (name[i] == ')')
            //    {
            //        flag = 0;
            //    }
            //    else if (flag == 1)
            //    {
            //        result = result + name[i];
            //    }

            //return result;
            return name;
        }

        private string GetStringContent(string[] s)
        {
            string content = "";
            if (s.Length > 0)
            {
                for (int i = 1; i < s.Length; i++)
                {
                    content = content + s[i] + " ";
                }
            }
            return content;
        }

        /// <summary>
        /// check if the string is a message
        /// a message mus have format <somewords>
        /// </summary>
        /// <param name="s">the input string</param>
        /// <returns>the bool value if the strign is message</returns>
        private bool IsMessage(string s)
        {
            return (s != null) && (s[0] == '<') && (s[s.Length - 1] == '>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="userid">the identity of the sender</param>
        /// <returns></returns>
        private bool SolveReply(string[] s, int userid, ref Email mail)
        {
            for (int i = 1; i < s.Length; i++)//the reply may contains many message-id of the previous emails
            {
                //var emails = from oneemail in MailList where oneemail.MessageId.Equals(s[i]) select oneemail;//get email that has the messageid = s[i]
                List<Email> emails = new List<Email>();
                foreach (Email oneemail in MailList)
                {
                    if (oneemail.MessageId.Equals(s[i]))
                    {
                        emails.Add(oneemail);
                    }
                }
                try
                {
                    if (emails.Count > 0)//if the  emails are found
                    {
                        Email email = emails[0];//get the first one
                        if (email != null)
                        {
                            if (userid != email.UserId)
                                Relation[userid][email.UserId]++;//update the array
                            mail.UserTo = email.UserId;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            //the case : there are more than one line contains parent email
            bool IsEnd = false;//flag that check it is the end of reply-to sector
            while (IsEnd)
            {
                string line = reader.ReadLine();//get a line
                string[] words = GetWords(line);//split the line
                if (words.Length == 1)
                {
                    if (IsMessage(words[0]))//check if the line is a message
                    {
                        //redo the work obove, check and update the data
                        //var emails = from oneemail in MailList where oneemail.MessageId.Equals(words[0]) select oneemail;
                        List<Email> emails = new List<Email>();
                        foreach (Email oneemail in MailList)
                        {
                            if (oneemail.MessageId.Equals(words[0]))
                            {
                                emails.Add(oneemail);
                            }
                        }
                        if (emails.Count > 0)
                        {
                            Email email = emails[0];
                            if (email != null)
                            {
                                Relation[userid][email.UserId]++;
                            }
                        }
                    }
                    else
                    {
                        IsEnd = true;
                    }
                }
                else
                {
                    IsEnd = true;
                }

                //check if this line contain messageid
                if (words.Length > 0)
                {
                    if (words[0].Equals("Message-ID"))
                    {
                        string id = SolveId(words);
                        Email email = new Email();
                        email.MessageId = id;
                        email.UserId = userid;
                        MailList.Add(email);//add new mail
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Return the message id
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string SolveId(string[] s)
        {
            if (s.Length == 2)
            {
                return s[1];
            }
            else
                return "";
        }

        /// <summary>
        /// selec the userid where email=mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        private int GetUserId(string mail)
        {
            //var users = from oneuser in UserList where oneuser.Email.Equals(mail) select oneuser;
            List<User> users = new List<User>();
            foreach (User oneuser in UserList)
            {
                if (oneuser.Email.Equals(mail))
                {
                    users.Add(oneuser);
                }
            }
            if (users.Count == 0)
            {
                return -1;
            }

            User user = users[0];
            if (user != null)
            {
                return user.UserId;
            }
            else return -1;
        }

        /// <summary>
        /// Read data from input file into a two dimesion array
        /// </summary>
        /// <param name="filePath">string</param>
        /// <returns>an 2 dimesion array</returns>
        private int[][] GetDataFromFile(string filePath)
        {
            reader = new StreamReader(filePath);
            while (!reader.EndOfStream)//read file
            {
                string firstLine = reader.ReadLine();//read the first line

                if (IsNewMail(firstLine)) // check if the first line is the beginning of the email
                {
                    User user = new User();
                    Email email = new Email();
                    string mail = GetEmail(firstLine);//get value of email adress
                    user.Email = mail;//assign email to the user object
                    user.UserId = GetUserId(mail);//select userid where email = mail,if can not found return -1
                    bool inHeader = true;//a flag to check if the line in header

                    //Get data from header of file
                    while (inHeader)
                    {
                        firstLine = reader.ReadLine();//read a line
                        string[] s = GetWords(firstLine);//split the string 

                        if (s.Length > 0)
                        {
                            switch (s[0])//check the first word of the string 
                            {
                                case "Date:":
                                    email.SendDate = GetStringContent(s);
                                    break;
                                case "Subject:":
                                    email.MessageSubject = GetStringContent(s);
                                    break;
                                case "From:"://if the first word == from, then add 1 into the number of email that the user has sent
                                    user.Name = GetName(s);//get user name
                                    if (user.UserId == -1)//if userid=-1 then this is new user, add to userlist
                                    {
                                        user.UserId = UserList.Count + 1;
                                        UserList.Add(user);
                                    }
                                    Relation[user.UserId][user.UserId]++;//add 1 into the number of email
                                    break;
                                case "Message-ID:"://this is the identity of header and the end line of the header also

                                    email.MessageId = SolveId(s);//get the value of message id 
                                    email.UserId = user.UserId;//assign userid to email
                                    MailList.Add(email);//add new mail
                                    inHeader = false;//end of a header
                                    break;
                                case "In-Reply-To:"://this is help to know the previous email,                                    
                                    inHeader = SolveReply(s, user.UserId, ref email);//may be this process has go through the case messageid
                                    break;
                            }
                        }

                    }
                }

            }

            reader.Close();
            return Relation;
        }

        /// <summary>
        /// check if the string has format of the date time
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool IsDatetime(string line)
        {
            DateTime dt;
            if (DateTime.TryParseExact(line, "ddd MMM d HH:mm:ss yyyy ", new CultureInfo("en-US"), DateTimeStyles.AllowLeadingWhite, out dt))
                return true;
            else
                return false;

        }

        /// <summary>
        ///  check the format of string if it is the beginning of email
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool IsNewMail(string line)
        {

            line = Regex.Replace(line, "  ", " ");
            String[] s = Regex.Split(line, " ");
            if (s.Length == 9)
            {
                if (!s[0].Equals("From"))
                {
                    return false;
                }
                if (!s[2].Equals("at"))
                {
                    return false;
                }
                string dt = "";
                for (int i = 4; i < 9; i++)
                {
                    dt = dt + s[i] + ' ';
                }
                if (!IsDatetime(dt))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// this function is just to test, don't care
        /// </summary>
        public void SetPositionForData()
        {
            Collection<Point> V = new Collection<Point>();
            int[] dy = new int[UserList.Count + 1];
            for (int i = 1; i <= UserList.Count; i++)
                for (int j = 1; j <= UserList.Count; j++)
                {
                    if (Relation[i][j] > 0)
                        dy[i]++;
                }

            //set position
            int[] dx = new int[UserList.Count + 1];
            for (int i = 1; i <= UserList.Count; i++)
            {
                Point temp = new Point(dy[i], dx[i]);
                V.Add(temp);
                dx[i]++;
            }

        }      

    }
}
