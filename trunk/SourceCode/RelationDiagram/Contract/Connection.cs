using System.Collections.Generic;
using System;
namespace NCRVisual.RelationDiagram
{
    /// <summary>
    /// The connection between 2 entities
    /// </summary>
    public class Connection : IConnection
    {
        #region IConnection Members
        public IEntity Source { get; set; }

        public IEntity Destination { get; set; }

        public int Value { get; set; }
        #endregion

        /// <summary>
        /// Get or set the Message Subject collection
        /// </summary>
        public List<string> MessageSubject { get; set; }

        /// <summary>
        /// Get or set the SendDate collection
        /// </summary>
        public List<DateTime> SendDate { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e1">The source entity</param>
        /// <param name="e2">The destination entity</param>
        public Connection(IEntity e1, IEntity e2, int val)
        {
            this.Source = e1;
            this.Destination = e2;
            this.Value = val;
            this.MessageSubject = new List<string>();
            this.SendDate = new List<DateTime>();
        }
    }
}
