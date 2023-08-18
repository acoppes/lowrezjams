﻿using System;
using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Game.Components
{
    public struct MovementComponent : IEntityComponent
    {
        [Flags]
        public enum Constraints
        {
            None = 0,
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2
        }

        public enum InitialSpeedType
        {
            None = 0,
            BaseSpeed = 1
        }
        
        public float speedMultiplier;

        public float baseSpeed;
        public float speed;
        
        public Vector3 currentVelocity;
        public Vector3 movingDirection;

        public Vector3 previousPosition;
        public float stationaryTime;

        public Constraints constraints;

        public bool isMoving => movingDirection.sqrMagnitude > Mathf.Epsilon 
                                && speed > GameConstants.MinSpeedMovement;

        public InitialSpeedType initialSpeedType;
    }
    
    public class MovementComponentDefinition : ComponentDefinitionBase
    {
        public float baseSpeed;

        public MovementComponent.Constraints constraints = MovementComponent.Constraints.None;
        public MovementComponent.InitialSpeedType initialSpeedType = MovementComponent.InitialSpeedType.None;
        
        public override string GetComponentName()
        {
            return nameof(MovementComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new MovementComponent
            {
                baseSpeed = baseSpeed,
                speedMultiplier = 1.0f,
                constraints = constraints,
                initialSpeedType = initialSpeedType
            });
        }
    }
}