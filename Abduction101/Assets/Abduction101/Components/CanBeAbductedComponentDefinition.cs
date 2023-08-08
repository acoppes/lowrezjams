using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct CanBeAbductedComponent : IEntityComponent
    {
        public int abductedTimeout;
        public bool isBeingAbducted => abductedTimeout > 0;
        public float abductionSpeed;
        public float abductionForce;

        public Entity source;
        public Vector3 center;
        public Vector3 horizontal;
        public Vector3 vertical;

        public float angle;
    }
    
    public class CanBeAbductedComponentDefinition : ComponentDefinitionBase
    {
        public float angle = 60;
        
        public override string GetComponentName()
        {
            return nameof(CanBeAbductedComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new CanBeAbductedComponent()
            {
                source = Entity.NullEntity,
                angle = angle
            });
        }
    }
}