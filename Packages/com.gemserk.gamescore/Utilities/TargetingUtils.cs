using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Components;
using Game.Components.Abilities;
using Game.Models;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using UnityEngine;
using Vertx.Debugging;

namespace Game.Utilities
{
    public static class TargetingUtils
    {
        public static bool HasAliveFlag(this HealthComponent.AliveType self, HealthComponent.AliveType flag)
        {
            return (self & flag) == flag;
        }

        // fixed targeting parameters (like what I am interested in targeting)

        // runtime values (like current player, position, etc)

        public class DistanceComparer : Comparer<Target>
        {
            private Vector3 position;

            public DistanceComparer(Vector3 position)
            {
                this.position = position;
            }
            
            public override int Compare(Target x, Target y)
            {
                var aDifference = x.position - position;
                var bDifference = y.position - position;

                if (aDifference.sqrMagnitude < bDifference.sqrMagnitude)
                {
                    return -1;
                }

                if (aDifference.sqrMagnitude > bDifference.sqrMagnitude)
                {
                    return 1;
                }

                return 0;
            }
        }
        
        public class DistanceLineComparer : Comparer<Target>
        {
            private Vector3 position;

            public DistanceLineComparer(Vector3 position)
            {
                this.position = position;
            }
            
            public override int Compare(Target x, Target y)
            {
                var aDifference = x.position - position;
                var bDifference = y.position - position;

                if (Mathf.Abs(aDifference.z) < Mathf.Abs(bDifference.z))
                {
                    return -1;
                }

                if (Mathf.Abs(aDifference.z) > Mathf.Abs(bDifference.z))
                {
                    return 1;
                }

                return 0;
            }
        }
        
        public class DirectionAlignedComparer : Comparer<Target>
        {
            private Vector3 position;
            private Vector2 direction;

            public DirectionAlignedComparer(Vector3 position, Vector2 direction)
            {
                this.position = position;
                this.direction = direction;
            }
            
            public override int Compare(Target x, Target y)
            {
                var aDifference = x.position - position;
                var bDifference = y.position - position;

                var xAngle = Vector2.Angle(direction, aDifference.XZ());
                var yAngle = Vector2.Angle(direction, bDifference.XZ());

                if (xAngle < yAngle)
                {
                    return -1;
                }

                if (yAngle < xAngle)
                {
                    return 1;
                }

                return 0;
            }
        }

        private static readonly ContactFilter2D HurtBoxContactFilter = new()
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("HurtBox")
        };
        
        private static readonly Collider[] colliders = new Collider[20];

        public static List<Target> GetTargets(this World world, Entity source)
        {
            var hitBox = world.GetComponent<HitBoxComponent>(source);
            var player = world.GetComponent<PlayerComponent>(source);
            
            return world.GetTargets(new RuntimeTargetingParameters
            {
                player = player.player,
                filter = new TargetingFilter
                {
                    area = hitBox.hit,
                    playerAllianceType = PlayerAllianceType.Enemies,
                    areaType = TargetingFilter.AreaType.HitBox,
                    aliveType = HealthComponent.AliveType.Alive
                }
            });
        }

        public static Target GetFirstTarget(this World world, RuntimeTargetingParameters runtimeTargetingParameters)
        {
            var targets = GetTargets(world, runtimeTargetingParameters);
            if (targets.Count > 0)
            {
                return targets[0];
            }
            return null;
        }
        

        public static List<Target> GetTargets(this World world, RuntimeTargetingParameters runtimeTargetingParameters)
        {
            var results = new List<Target>();
            GetTargets(world, runtimeTargetingParameters, results);
            return results;
        }
        
        public static void GetTargets(this World world, RuntimeTargetingParameters runtimeTargetingParameters, List<Target> results)
        {
            var targets = new List<Target>();

            var filter = runtimeTargetingParameters.filter;
            
            if (filter.areaType == TargetingFilter.AreaType.HitBox)
            {
                // collect targets using physics collider
                var area = filter.area;

                var colliderCount = DrawPhysics.OverlapBoxNonAlloc(area.position3d, area.size * 0.5f, colliders,
                    Quaternion.identity, HurtBoxContactFilter.layerMask,
                    QueryTriggerInteraction.Collide);

                if (colliderCount > 0)
                {
                    for (var i = 0; i < colliderCount; i++)
                    {
                        var collider = colliders[i];
                        var targetEntityReference = collider.GetComponent<TargetReference>();
                        var target = targetEntityReference.target;
                        targets.Add(target);
                    }
                }
            } else
            {
                var targetComponents = world.GetComponents<TargetComponent>();
                
                foreach (var entity in world.GetFilter<TargetComponent>().Exc<DisabledComponent>().End())
                {
                    var targetComponent = targetComponents.Get(entity);
                    targets.Add(targetComponent.target);
                }
            }
            
            // filter valid targets
            foreach (var target in targets)
            {
                if (ValidateTarget(target, runtimeTargetingParameters))
                {
                    results.Add(target);
                }
            }

            var sorter = runtimeTargetingParameters.filter.sorter as ITargetSorter;
            if (sorter != null)
            {
                sorter.Sort(results, runtimeTargetingParameters);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidateTarget(Target target, RuntimeTargetingParameters runtimeTargetingParameters)
        {
            if (target == null)
            {
                return false;
            }
            
            var targetingFilter = runtimeTargetingParameters.filter;
            
            if (!targetingFilter.playerAllianceType.CheckPlayerAlliance(runtimeTargetingParameters.player, target.player))
            {
                return false;
            }
            
            if (targetingFilter.aliveType != HealthComponent.AliveType.None)
            {
                if (target.aliveType == HealthComponent.AliveType.None)
                {
                    return false;
                }
                    
                if (!targetingFilter.aliveType.HasFlag(target.aliveType))
                {
                    return false;
                }
            }

            var difference = target.position - runtimeTargetingParameters.position;

            if (targetingFilter.angle.Min > 0 || targetingFilter.angle.Max < 180)
            {
                var targetAngle = Vector2.Angle(runtimeTargetingParameters.direction.XZ(), difference.XZ());
                if (targetAngle > targetingFilter.angle.Max)
                {
                    return false;
                }
                
                if (targetAngle < targetingFilter.angle.Min)
                {
                    return false;
                }
            }

            if (targetingFilter.distanceType == TargetingFilter.CheckDistanceType.InsideDistance)
            {
                var differenceSqrMagnitude = difference.sqrMagnitude;
                
                if (differenceSqrMagnitude > targetingFilter.maxRangeSqr)
                {
                    return false;
                }
                
                if (differenceSqrMagnitude < targetingFilter.minRangeSqr)
                {
                    return false;
                }
            }
            
            if (targetingFilter.distanceType == TargetingFilter.CheckDistanceType.InsideDistanceXZ)
            {
                var differenceSqrMagnitude = difference.XZ().sqrMagnitude;
                
                if (differenceSqrMagnitude > targetingFilter.maxRangeSqr)
                {
                    return false;
                }
                
                if (differenceSqrMagnitude < targetingFilter.minRangeSqr)
                {
                    return false;
                }
            }

            return true;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RuntimeTargetingParameters GetRuntimeTargetingParameters(this Ability ability)
        {
            return new RuntimeTargetingParameters()
            {
                filter = ability.targeting.targetingFilter,
                position = ability.center,
                direction = ability.direction,
                player = ability.player,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidTarget(this Ability ability, Target target)
        {
            return ValidateTarget(target, ability.GetRuntimeTargetingParameters());
        }

        public static Vector3 AdjustDirectionToNearestEnemyInLine(World world, int player, Vector3 position, Vector3 lookingDirection, 
            float lineAngle, float lineHeight)
        {
            // Search for enemies in line or angle and adjust direction
            var targets = TargetingUtils.GetTargets(world, new RuntimeTargetingParameters()
            {
                player = player,
                filter = new TargetingFilter()
                {
                    area = HitBox.AllTheWorld,
                    aliveType = HealthComponent.AliveType.Alive,
                    areaType = TargetingFilter.AreaType.Nothing,
                    playerAllianceType = PlayerAllianceType.Enemies
                }
            });

            targets.Sort(new DistanceLineComparer(position));

            foreach (var target in targets)
            {
                var difference = target.position - position;

                if (Vector2.Angle(lookingDirection.XZ(), difference.XZ()) > lineAngle)
                {
                    continue;
                }

                if (Mathf.Abs(difference.z) > lineHeight)
                {
                    continue;
                }

                var direction = target.position - position;
                direction.Normalize();

                return direction;
            }

            return lookingDirection;
        }
    }
}