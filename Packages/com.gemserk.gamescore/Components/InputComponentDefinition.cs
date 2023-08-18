using System;
using System.Collections.Generic;
using Gemserk.Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Components
{
    public static class InputComponentExtensions
    {
        public static InputComponent.InputAction right(this InputComponent inputComponent) => inputComponent.actions["right"];
        public static InputComponent.InputAction left(this InputComponent inputComponent) => inputComponent.actions["left"];
        public static InputComponent.InputAction up(this InputComponent inputComponent) => inputComponent.actions["up"];
        public static InputComponent.InputAction down(this InputComponent inputComponent) => inputComponent.actions["down"];
        
        public static InputComponent.InputAction forward(this InputComponent inputComponent) => inputComponent.actions["forward"];
        public static InputComponent.InputAction backward(this InputComponent inputComponent) => inputComponent.actions["backward"];
        
        public static InputComponent.InputAction button1(this InputComponent inputComponent) => inputComponent.actions["button1"];
        public static InputComponent.InputAction button2(this InputComponent inputComponent) => inputComponent.actions["button2"];
        public static InputComponent.InputAction button3(this InputComponent inputComponent) => inputComponent.actions["button3"];
        
        public static InputComponent.InputAction direction(this InputComponent inputComponent) => inputComponent.actions["movement"];

        public static Vector3 direction3d(this InputComponent inputComponent)
        {
            var direction = inputComponent.direction().vector2;
            return new Vector3(direction.x, 0, direction.y);
        }
    }
    
    public struct InputComponent : IEntityComponent
    {
        public class InputAction
        {
            public const int InputTypeValue = (int) InputActionType.Value;
            public const int InputTypeButton = (int) InputActionType.Button;

            public string name;

            // by default are button
            public int type = InputTypeButton;
        
            public bool isPressed;
            public bool wasPressed;

            public Vector2 vector2;

            //    public bool wasPressedThisFrame => isPressed && wasPressed;

            public InputAction(string name)
            {
                this.name = name;
                isPressed = false;
                wasPressed = false;
            }

            public void UpdatePressed(bool pressed)
            { 
                wasPressed = !isPressed && pressed;
                isPressed = pressed;
            }

            public override string ToString()
            {
                if (type == 0)
                {
                    return $"{name}:{vector2}";
                }
            
                return $"{name}:{isPressed}";
            }
        }
        
        public Dictionary<string, InputAction> actions;

        public static InputComponent Default()
        {
            return new InputComponent()
            {
                actions = new Dictionary<string, InputAction>()
                {
                    { "right", new InputAction("right") },
                    { "left", new InputAction("left") },
                    { "up", new InputAction("up") },
                    { "down", new InputAction("down") },
                    { "forward", new InputAction("forward") },
                    { "backward", new InputAction("backward") },
                    { "button1", new InputAction("button1") },
                    { "button2", new InputAction("button2") },
                    { "button3", new InputAction("button3") },
                    { "movement", new InputAction("movement") },
                }
            };
        }

        public InputAction GetButton(string name)
        {
            return actions[name];
        }
        
        public void ClearPressedChanged()
        {
            foreach (var buttonName in actions.Keys)
            {
                actions[buttonName].wasPressed = false;
            }
        }

        public bool IsActionDefined(string buttonName)
        {
            return actions.ContainsKey(buttonName);
        }
    }
    
    public class InputComponentDefinition : ComponentDefinitionBase
    {
        public List<string> customButtonMap;
        public float bufferTime = BufferedInputComponent.DefaultBufferTime;

        public override string GetComponentName()
        {
            return nameof(InputComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            var controlComponent = InputComponent.Default();
            if (customButtonMap.Count > 0)
            {
                controlComponent.actions = new Dictionary<string, InputComponent.InputAction>(StringComparer.OrdinalIgnoreCase);
                foreach (var button in customButtonMap)
                {
                    controlComponent.actions.Add(button, new InputComponent.InputAction(button));
                }
            }
            world.AddComponent(entity, controlComponent);
            
            if (bufferTime > 0)
            {
                var bufferInputComponent = BufferedInputComponent.Default();
                bufferInputComponent.totalBufferTime = bufferTime;
                world.AddComponent(entity, bufferInputComponent);
            }
        }
    }
}