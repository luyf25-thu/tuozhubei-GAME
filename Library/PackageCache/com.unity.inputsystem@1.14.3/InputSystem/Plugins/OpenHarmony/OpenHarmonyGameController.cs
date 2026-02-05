#if UNITY_EDITOR || UNITY_OPENHARMONY || PACKAGE_DOCS_GENERATION
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.OpenHarmony.LowLevel;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.OpenHarmony.LowLevel
{
    /// <summary>
    /// Default state layout for OpenHarmony game controller.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct OpenHarmonyGameControllerState : IInputStateTypeInfo
    {
        public const int MaxAxes = 8;
        public const int MaxButtons = 2316;

        public class Variants
        {
            public const string Gamepad = "Gamepad";
            public const string Joystick = "Joystick";
            public const string DPadAxes = "DpadAxes";
            public const string DPadButtons = "DpadButtons";
        }

        internal const uint kAxisOffset = sizeof(uint) * (uint)((MaxButtons + 31) / 32);

        public static FourCC kFormat = new FourCC('O', 'H', 'G', 'C');

        [InputControl(name = "dpad", layout = "Dpad", bit = (uint)OpenHarmonyKeyCode.DpadUp, sizeInBits = 4, variants = Variants.DPadButtons)]
        [InputControl(name = "dpad/up", bit = (uint)OpenHarmonyKeyCode.DpadUp, variants = Variants.DPadButtons)]
        [InputControl(name = "dpad/down", bit = (uint)OpenHarmonyKeyCode.DpadDown, variants = Variants.DPadButtons)]
        [InputControl(name = "dpad/left", bit = (uint)OpenHarmonyKeyCode.DpadLeft, variants = Variants.DPadButtons)]
        [InputControl(name = "dpad/right", bit = (uint)OpenHarmonyKeyCode.DpadRight, variants = Variants.DPadButtons)]
        [InputControl(name = "buttonSouth", bit = (uint)OpenHarmonyKeyCode.ButtonA, variants = Variants.Gamepad)]
        [InputControl(name = "buttonWest", bit = (uint)OpenHarmonyKeyCode.ButtonX, variants = Variants.Gamepad)]
        [InputControl(name = "buttonNorth", bit = (uint)OpenHarmonyKeyCode.ButtonY, variants = Variants.Gamepad)]
        [InputControl(name = "buttonEast", bit = (uint)OpenHarmonyKeyCode.ButtonB, variants = Variants.Gamepad)]
        [InputControl(name = "leftStickPress", bit = (uint)OpenHarmonyKeyCode.ButtonThumbl, variants = Variants.Gamepad)]
        [InputControl(name = "rightStickPress", bit = (uint)OpenHarmonyKeyCode.ButtonThumbr, variants = Variants.Gamepad)]
        [InputControl(name = "leftShoulder", bit = (uint)OpenHarmonyKeyCode.ButtonL1, variants = Variants.Gamepad)]
        [InputControl(name = "rightShoulder", bit = (uint)OpenHarmonyKeyCode.ButtonR1, variants = Variants.Gamepad)]
        [InputControl(name = "start", bit = (uint)OpenHarmonyKeyCode.ButtonStart, variants = Variants.Gamepad)]
        [InputControl(name = "select", bit = (uint)OpenHarmonyKeyCode.ButtonSelect, variants = Variants.Gamepad)]
        public fixed uint buttons[(MaxButtons + 31) / 32];

        [InputControl(name = "dpad", layout = "Dpad", offset = (uint)OpenHarmonyAxis.HatX * sizeof(float) + kAxisOffset, format = "VEC2", sizeInBits = 64, variants = Variants.DPadAxes)]
        [InputControl(name = "dpad/right", offset = 0, bit = 0, sizeInBits = 32, format = "FLT", parameters = "clamp=3,clampConstant=0,clampMin=0,clampMax=1", variants = Variants.DPadAxes)]
        [InputControl(name = "dpad/left", offset = 0, bit = 0, sizeInBits = 32, format = "FLT", parameters = "clamp=3,clampConstant=0,clampMin=-1,clampMax=0,invert", variants = Variants.DPadAxes)]
        [InputControl(name = "dpad/down", offset = ((uint)OpenHarmonyAxis.HatY - (uint)OpenHarmonyAxis.HatX) * sizeof(float), bit = 0, sizeInBits = 32, format = "FLT", parameters = "clamp=3,clampConstant=0,clampMin=0,clampMax=1", variants = Variants.DPadAxes)]
        [InputControl(name = "dpad/up", offset = ((uint)OpenHarmonyAxis.HatY - (uint)OpenHarmonyAxis.HatX) * sizeof(float), bit = 0, sizeInBits = 32, format = "FLT", parameters = "clamp=3,clampConstant=0,clampMin=-1,clampMax=0,invert", variants = Variants.DPadAxes)]
        [InputControl(name = "leftTrigger", offset = (uint)OpenHarmonyAxis.Brake * sizeof(float) + kAxisOffset, parameters = "clamp=1,clampMin=0,clampMax=1.0", variants = Variants.Gamepad)]
        [InputControl(name = "rightTrigger", offset = (uint)OpenHarmonyAxis.Gas * sizeof(float) + kAxisOffset, parameters = "clamp=1,clampMin=0,clampMax=1.0", variants = Variants.Gamepad)]
        [InputControl(name = "leftStick", variants = Variants.Gamepad)]
        [InputControl(name = "leftStick/y", variants = Variants.Gamepad, parameters = "invert")]
        [InputControl(name = "leftStick/up", variants = Variants.Gamepad, parameters = "invert,clamp=1,clampMin=-1.0,clampMax=0.0")]
        [InputControl(name = "leftStick/down", variants = Variants.Gamepad, parameters = "invert=false,clamp=1,clampMin=0,clampMax=1.0")]
        ////FIXME: state for this control is not contiguous
        [InputControl(name = "rightStick", offset = (uint)OpenHarmonyAxis.Z * sizeof(float) + kAxisOffset, sizeInBits = ((uint)OpenHarmonyAxis.Rz - (uint)OpenHarmonyAxis.Z + 1) * sizeof(float) * 8, variants = Variants.Gamepad)]
        [InputControl(name = "rightStick/x", variants = Variants.Gamepad)]
        [InputControl(name = "rightStick/y", offset = ((uint)OpenHarmonyAxis.Rz - (uint)OpenHarmonyAxis.Z) * sizeof(float), variants = Variants.Gamepad, parameters = "invert")]
        [InputControl(name = "rightStick/up", offset = ((uint)OpenHarmonyAxis.Rz - (uint)OpenHarmonyAxis.Z) * sizeof(float), variants = Variants.Gamepad, parameters = "invert,clamp=1,clampMin=-1.0,clampMax=0.0")]
        [InputControl(name = "rightStick/down", offset = ((uint)OpenHarmonyAxis.Rz - (uint)OpenHarmonyAxis.Z) * sizeof(float), variants = Variants.Gamepad, parameters = "invert=false,clamp=1,clampMin=0,clampMax=1.0")]
        public fixed float axis[MaxAxes];

        public FourCC format
        {
            get { return kFormat; }
        }

        public OpenHarmonyGameControllerState WithButton(OpenHarmonyKeyCode code, bool value = true)
        {
            fixed(uint* buttonsPtr = buttons)
            {
                if (value)
                    buttonsPtr[(int)code / 32] |= 1U << ((int)code % 32);
                else
                    buttonsPtr[(int)code / 32] &= ~(1U << ((int)code % 32));
            }
            return this;
        }

        public OpenHarmonyGameControllerState WithAxis(OpenHarmonyAxis axis, float value)
        {
            fixed(float* axisPtr = this.axis)
            {
                axisPtr[(int)axis] = value;
            }
            return this;
        }
    }

    internal enum OpenHarmonyInputSource
    {
        Keyboard = 5,
        Gamepad = 4,
        Touchscreen = 2,
        Mouse = 1,
    }

    [Serializable]
    internal struct OpenHarmonyDeviceCapabilities
    {
        public string deviceDescriptor;
        public int productId;
        public int vendorId;
        public bool isVirtual;
        public OpenHarmonyAxis[] motionAxes;
        public OpenHarmonyInputSource inputSources;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static OpenHarmonyDeviceCapabilities FromJson(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            return JsonUtility.FromJson<OpenHarmonyDeviceCapabilities>(json);
        }

        public override string ToString()
        {
            return
                $"deviceDescriptor = {deviceDescriptor}, productId = {productId}, vendorId = {vendorId}, isVirtual = {isVirtual}, motionAxes = {(motionAxes == null ? "<null>" : String.Join(",", motionAxes.Select(i => i.ToString()).ToArray()))}, inputSources = {inputSources}";
        }
    }
}

namespace UnityEngine.InputSystem.OpenHarmony
{
    [InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), variants = OpenHarmonyGameControllerState.Variants.Gamepad)]
    public class OpenHarmonyGamepad : Gamepad
    {
    }

    /// <summary>
    /// Generic controller with Dpad axes
    /// </summary>
    [InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), hideInUI = true,
        variants = OpenHarmonyGameControllerState.Variants.Gamepad + InputControlLayout.VariantSeparator + OpenHarmonyGameControllerState.Variants.DPadAxes)]
    public class OpenHarmonyGamepadWithDpadAxes : OpenHarmonyGamepad
    {
    }

    /// <summary>
    /// Generic controller with Dpad buttons
    /// </summary>
    [InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), hideInUI = true,
        variants = OpenHarmonyGameControllerState.Variants.Gamepad + InputControlLayout.VariantSeparator + OpenHarmonyGameControllerState.Variants.DPadButtons)]
    public class OpenHarmonyGamepadWithDpadButtons : OpenHarmonyGamepad
    {
    }

    /// <summary>
    /// Joystick on OpenHarmony.
    /// </summary>
    [InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), variants = OpenHarmonyGameControllerState.Variants.Joystick)]
    public class OpenHarmonyJoystick : Joystick
    {
    }

    ///// <summary>
    ///// A PlayStation DualShock 4 controller connected to an OpenHarmony device.
    ///// </summary>
    //[InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), displayName = "OpenHarmony DualShock 4 Gamepad",
    //    variants = OpenHarmonyGameControllerState.Variants.Gamepad + InputControlLayout.VariantSeparator + OpenHarmonyGameControllerState.Variants.DPadAxes)]
    //public class DualShock4GamepadOpenHarmony : DualShockGamepad
    //{
    //}

    ///// <summary>
    ///// A PlayStation DualShock 4 controller connected to an OpenHarmony device.
    ///// </summary>
    //[InputControlLayout(stateType = typeof(OpenHarmonyGameControllerState), displayName = "OpenHarmony Xbox One Controller",
    //    variants = OpenHarmonyGameControllerState.Variants.Gamepad + InputControlLayout.VariantSeparator + OpenHarmonyGameControllerState.Variants.DPadAxes)]
    //public class XboxOneGamepadOpenHarmony : XInput.XInputController
    //{
    //}
}
#endif // UNITY_EDITOR || UNITY_OPENHARMONY
