using System;
using System.Collections.Generic;
using System.Web;

namespace NCRVisual.Web.Items
{
    public class PostInfoMin : IEquatable<PostInfoMin>
    {
        public int TopicId { get; set; }
        public int PosterId { get; set; }

        public PostInfoMin(int topicId, int posterId)
        {
            this.TopicId = topicId;
            this.PosterId = posterId;
        }

        public bool Equals(PostInfoMin other)
        {
            if (other.TopicId == this.TopicId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}