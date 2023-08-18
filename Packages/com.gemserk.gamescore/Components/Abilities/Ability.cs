﻿using System.Collections.Generic;
using Game.Utilities;
using Gemserk.Utilities;
using UnityEngine;

namespace Game.Components.Abilities
{
    public struct AbilityTarget
    {
        public Vector3 position;
        public Target target;
        public bool valid;

        public Vector3 targetPosition => (target != null && valid) ? target.position : position;
        
        public override string ToString()
        {
            return $"[{target},{position},{valid}]";
        }
    }
    
    public class Ability
    {
        public enum ExecutionType
        {
            None = 0, 
            AutoCast = 1
        }
        
        public enum ReloadCooldownType
        {
            IfNoExecuting = 0, // reloads when no executing
            IfNoActionsExecuting = 1, // reloads when no action executing
        }

        public enum ResetCooldownType
        {
            None = 0,
            ResetsIfAnyActionExecuting = 1
        }

        public enum StopType
        {
            Completed,
            Interrupted,
            Cancelled
        }
        
        public string name;

        public bool isAutomatic => executionType == ExecutionType.AutoCast;

        public ExecutionType executionType;
        public ReloadCooldownType cooldownType = ReloadCooldownType.IfNoExecuting;
        public ResetCooldownType resetCooldownType = ResetCooldownType.None;
        
        public bool isReady => cooldown.IsReady && (totalCharges == 0 || currentCharges > 0);
        public bool hasTargets => abilityTargets.Count > 0;

        public bool isExecuting;
        public bool isCharged;

        public bool autoTarget;
        public ITargeting targeting;

        public bool pendingExecution;
        // public int completedTimes;

        public Cooldown cooldown;
        public Cooldown startTime;

        public Vector3 center;
        public Vector3 direction;
        public int player;

        public List<AbilityTarget> abilityTargets = new List<AbilityTarget>();
        public bool targetsLocked;

        public int currentCharges;
        public int totalCharges;

        public int currentCharge => totalCharges - currentCharges;

        public int executedTimes;

        public float executionTime;

        public void Start()
        {
            isExecuting = true;
            pendingExecution = false;
            
            if (totalCharges > 0)
            {
                currentCharges--;
            }

            executedTimes++;

            executionTime = 0;
        }

        public void Stop(StopType stopType)
        {
            // if (stopType == StopType.Completed)
            // {
            //     completedTimes++;
            // }
            
            cooldown.Reset();
            isExecuting = false;
            
            // abilityTargets.Clear();
            // targetsLocked = false;
        }
        
        public void CopyTarget(Target target)
        {
            abilityTargets.Clear();
            abilityTargets.Add(new AbilityTarget
            {
                position = target.position,
                target = target,
                valid = true
            });
        }
        
        public void CopyTarget(AbilityTarget abilityTarget)
        {
            abilityTargets.Clear();
            abilityTargets.Add(abilityTarget);
        }

        public void CopyTargets(List<Target> targets)
        {
            abilityTargets.Clear();
            foreach (var target in targets)
            {
                abilityTargets.Add(new AbilityTarget
                {
                    position = target.position,
                    target = target,
                    valid = true
                });
            }
        }

        public void ConsumeCharge(int charges = 1)
        {
            this.currentCharges -= charges;
        }
        
        public void Fill()
        {
            cooldown.Fill();
            currentCharges = totalCharges;
        }
    }
}