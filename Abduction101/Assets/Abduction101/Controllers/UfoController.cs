using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;

namespace Abduction101.Controllers
{
    public class UfoController : ControllerBase, IUpdate
    {
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var movementComponent = ref entity.Get<MovementComponent>();
            movementComponent.speed = movementComponent.baseSpeed;
            
            // TODO: acceleration
            // movementComponent.movingDirection = entity.Get<InputComponent>().direction3d();
            
            // Debug.Log(movementComponent.movingDirection);
        }
    }
}
