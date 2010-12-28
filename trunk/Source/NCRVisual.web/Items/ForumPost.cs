using System;
using System.Collections.Generic;
using System.Web;

namespace NCRVisual.Web.Items
{
    public class ForumPost
    {
        #region private variables
        private int posterId;
        private string posterEmailAddr;
        private string posterName;
        private int postId;
        private int topicId;
        private string postSubject;
        private int postTime;
        private double timeZone;

        #endregion

        #region getters and setters

        public string PostSubject
        {
            get { return postSubject; }
            set { postSubject = value; }
        }

        public double TimeZone
        {
            get { return timeZone; }
            set { timeZone = value; }
        }

        public string PosterEmailAddr
        {
            get { return posterEmailAddr; }
            set { posterEmailAddr = value; }
        }

        public string PosterName
        {
            get { return posterName; }
            set { posterName = value; }
        }

        public int PosterId
        {
            get { return posterId; }
            set { posterId = value; }
        }

        public int PostId
        {
            get { return postId; }
            set { postId = value; }
        }

        public int TopicId
        {
            get { return topicId; }
            set { topicId = value; }
        }

        public int PostTime
        {
            get { return postTime; }
            set { postTime = value; }
        }

        #endregion
    }
}