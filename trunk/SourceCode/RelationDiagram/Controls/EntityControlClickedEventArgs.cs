using System;

namespace NCRVisual.RelationDiagram
{
    public class EntityControlClickedEventArgs : EventArgs
    {
        public EntityControlClickedEventArgs(IEntity entity)
        {
            this.Entity = entity;
        }

        public IEntity Entity { get; set; }
    }
}
