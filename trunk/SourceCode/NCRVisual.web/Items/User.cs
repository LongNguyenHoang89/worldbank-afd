using System;
using System.Collections.Generic;
using System.Web;

namespace NCRVisual.Web.Items
{
    public class User : IEquatable<User>
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public bool Equals(User other)
        {
            if (this.UserId == other.UserId)
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