using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;

namespace Game.Systems
{
    public class TargetSystem : BaseSystem, IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var targetComponents = world.GetComponents<TargetComponent>();
            var playerComponents = world.GetComponents<PlayerComponent>();
            var positions = world.GetComponents<PositionComponent>();
            
            foreach (var entity in world.GetFilter<TargetComponent>().End())
            {
                ref var targetComponent = ref targetComponents.Get(entity);
                targetComponent.target.targeted = false;
            }

            foreach (var entity in world.GetFilter<TargetComponent>().Inc<PositionComponent>().End())
            {
                ref var targetComponent = ref targetComponents.Get(entity);
                var positionComponent = positions.Get(entity);

                ref var target = ref targetComponent.target;
                target.entity = world.GetEntity(entity);
                target.position = positionComponent.value;
            }
            
            foreach (var entity in world.GetFilter<TargetComponent>().Inc<PlayerComponent>().End())
            {
                ref var targetComponent = ref targetComponents.Get(entity);
                var playerComponent = playerComponents.Get(entity);

                targetComponent.target.player = playerComponent.player;
            }

            // var nameComponents = world.GetComponents<NameComponent>();
            // foreach (var entity in world.GetFilter<TargetComponent>().Inc<NameComponent>().End())
            // {
            //     ref var targetComponent = ref targetComponents.Get(entity);
            //     var nameComponent = nameComponents.Get(entity);
            //     targetComponent.target.name = nameComponent.name;
            // }
            
            var healthComponents = world.GetComponents<HealthComponent>();
            
            foreach (var entity in world.GetFilter<TargetComponent>().Inc<HealthComponent>().End())
            {
                ref var targetComponent = ref targetComponents.Get(entity);
                var healthComponent = healthComponents.Get(entity);

                targetComponent.target.aliveType = healthComponent.aliveType;
                targetComponent.target.healthFactor = healthComponent.factor;
            }

            var movementComponents = world.GetComponents<MovementComponent>();
            foreach (var entity in world.GetFilter<TargetComponent>().Inc<MovementComponent>().End())
            {
                ref var targetComponent = ref targetComponents.Get(entity);
                var movementComponent = movementComponents.Get(entity);

                targetComponent.target.velocity = movementComponent.currentVelocity;
            }
        }
    }
}