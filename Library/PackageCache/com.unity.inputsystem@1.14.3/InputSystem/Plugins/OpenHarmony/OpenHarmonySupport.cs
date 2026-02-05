#if UNITY_EDITOR || UNITY_OPENHARMONY
using System.Linq;
using UnityEngine.InputSystem.OpenHarmony.LowLevel;
using UnityEngine.InputSystem.OpenHarmony;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.OpenHarmony
{
    /// <summary>
    /// Initializes custom openharmony devices.
    /// You can use 'hdc shell dumpsys input' from terminal to output information about all input devices.
    /// </summary>
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
    class OpenHarmonySupport
    {
        internal const string kOpenHarmonyInterface = "OpenHarmony";
        public static void Initialize()
        {

            InputSystem.RegisterLayout<OpenHarmonyGamepad>(
            matches: new InputDeviceMatcher()
                .WithInterface(kOpenHarmonyInterface)
                .WithDeviceClass("OpenHarmonyGameController"));
            InputSystem.RegisterLayout<OpenHarmonyJoystick>(
            matches: new InputDeviceMatcher()
                .WithInterface(kOpenHarmonyInterface)
                .WithDeviceClass("OpenHarmonyGameController"));
            InputSystem.RegisterLayout<OpenHarmonyGamepadWithDpadAxes>();
            InputSystem.RegisterLayout<OpenHarmonyGamepadWithDpadButtons>();
            //Add Sensors
            InputSystem.RegisterProcessor<OpenHarmonyCompensateDirectionProcessor>();
            InputSystem.RegisterProcessor<OpenHarmonyCompensateRotationProcessor>();

            InputSystem.RegisterLayout<OpenHarmonyAccelerometer>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Accelerometer));

            InputSystem.RegisterLayout<OpenHarmonyGyroscope>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Gyroscope));

            InputSystem.RegisterLayout<OpenHarmonyAmbientLightSensor>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.AmbientLight));

            InputSystem.RegisterLayout<OpenHarmonyMagneticFieldSensor>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.MagneticField));

            InputSystem.RegisterLayout<OpenHarmonyBarometerSensor>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Barometer));

            InputSystem.RegisterLayout<OpenHarmonyProximity>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Proximity));

            InputSystem.RegisterLayout<OpenHarmonyGravitySensor>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Gravity));

            InputSystem.RegisterLayout<OpenHarmonyLinearAccelerationSensor>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.LinearAccelerometer));

            InputSystem.RegisterLayout<OpenHarmonyRotationVector>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.RotationVector));

            InputSystem.RegisterLayout<OpenHarmonyPedometer>(
                matches: new InputDeviceMatcher()
                    .WithInterface(kOpenHarmonyInterface)
                    .WithDeviceClass("OpenHarmonySensor")
                    .WithCapability("sensorType", OpenHarmonySensorType.Pedometer));

            InputSystem.onFindLayoutForDevice += OnFindLayoutForDevice;
        }
        internal static string OnFindLayoutForDevice(ref InputDeviceDescription description,
           string matchedLayout, InputDeviceExecuteCommandDelegate executeCommandDelegate)
        {
            // If we already have a matching layout, someone registered a better match.
            // We only want to act as a fallback.
            if (!string.IsNullOrEmpty(matchedLayout) && matchedLayout != "OpenHarmonyGamepad" && matchedLayout != "OpenHarmonyJoystick")
                return null;

            if (description.interfaceName != "OpenHarmony" || string.IsNullOrEmpty(description.capabilities))
                return null;

            ////TODO: these should just be Controller and Sensor;
            switch (description.deviceClass)
            {
                case "OpenHarmonyGameController":
                    {
                        var caps = OpenHarmonyDeviceCapabilities.FromJson(description.capabilities);

                        if (caps.motionAxes == null)
                            return "OpenHarmonyGamepadWithDpadButtons";

                        // Vendor Ids, Product Ids can be found here http://www.linux-usb.org/usb.ids
                        const int kVendorMicrosoft = 0x045e;
                        const int kVendorSony = 0x054c;

                        // Fallback to generic gamepads
                        if (caps.motionAxes.Contains(OpenHarmonyAxis.HatX) &&
                            caps.motionAxes.Contains(OpenHarmonyAxis.HatY))
                            return "OpenHarmonyGamepadWithDpadAxes";

                        return "OpenHarmonyGamepadWithDpadButtons";
                    }
                default:
                    return null;
            }
        }
    }
}
#endif // UNITY_EDITOR || UNITY_OPENHARMONY
