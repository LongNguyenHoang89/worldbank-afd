using System.Collections.Generic;

namespace NCRVisual.RelationDiagram
{
    public interface IEntity
    {
        //Id of the Entity
        string Id { get; set; }

        //Name of the entity
        string Name { get; set; }

        //Set of connected entities with this entity
        List<IConnection> Connections { get; set; }
    }
}
