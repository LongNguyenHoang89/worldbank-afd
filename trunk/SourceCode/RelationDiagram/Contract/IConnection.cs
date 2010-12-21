using System.Collections.Generic;

namespace NCRVisual.RelationDiagram
{
    public interface IConnection
    {
        /// <summary>
        /// Get or set the source entity
        /// </summary>
        IEntity Source { get; set; }

        /// <summary>
        /// Get or set the destination entity
        /// </summary>
        IEntity Destination { get; set; }

        /// <summary>
        /// The value of the edge
        /// </summary>
        int Value { get; set; }
    }
}
