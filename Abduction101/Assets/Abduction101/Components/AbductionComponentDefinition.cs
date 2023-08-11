using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct AbductionComponent : IEntityComponent
    {
        public int abductedTimeout;
        public bool isBeingAbducted => abductedTimeout > 0;
        public bool wasBeingAbducted;
        
        public float abductionSpeed;
        
        public float abductionForce;
        public float abductionCenterForce;

        public Entity source;
        public Vector3 center;
        public Vector3 horizontal;
        public Vector3 vertical;

        public float targetAngle;
        public float currentAngle;
    }
    
    public class AbductionComponentDefinition : ComponentDefinitionBase
    {
        public float angle = 60;
        
        public override string GetComponentName()
        {
            return nameof(AbductionComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new AbductionComponent()
            {
                source = Entity.NullEntity,
                targetAngle = angle
            });
        }
    }
}