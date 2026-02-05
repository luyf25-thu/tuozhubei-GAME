#if UNITY_EDITOR || UNITY_OPENHARMONY || PACKAGE_DOCS_GENERATION
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.OpenHarmony.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.OpenHarmony.LowLevel
{
    /// <summary>
    /// Enum used to identity the axis type in the OpenHarmony motion input event. See <see cref="OpenHarmonyGameControllerState.axis"/>.
    /// See https://developer.openharmony.com/reference/openharmony/view/MotionEvent#constants_1 for more details.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "False positive")]
    public enum OpenHarmonyAxis
    {
        /// <summary>
        /// X axis of a motion event.
        /// </summary>
        X = 0,

        /// <summary>
        /// Y axis of a motion event.
        /// </summary>
        Y = 1,

        /// <summary>
        /// Z axis of a motion event.
        /// </summary>
        Z = 2,

        /// <summary>
        /// Z Rotation axis of a motion event.
        /// </summary>
        Rz = 3,

        /// <summary>
        /// Hat X axis of a motion event.
        /// </summary>
        HatX = 6,

        /// <summary>
        /// Hat Y axis of a motion event.
        /// </summary>
        HatY = 7,

        /// <summary>
        /// Gas axis of a motion event.
        /// </summary>
        Gas = 4,

        /// <summary>
        /// Break axis of a motion event.
        /// </summary>
        Brake = 5,

        ///// <summary>
        ///// Generic 1 axis of a motion event.
        ///// </summary>
        //Generic1 = 32,

        ///// <summary>
        ///// Generic 2 axis of a motion event.
        ///// </summary>
        //Generic2 = 33,

        ///// <summary>
        ///// Generic 3 axis of a motion event.
        ///// </summary>
        //Generic3 = 34,

        ///// <summary>
        ///// Generic 4 axis of a motion event.
        ///// </summary>
        //Generic4 = 35,

        ///// <summary>
        ///// Generic 5 axis of a motion event.
        ///// </summary>
        //Generic5 = 36,

        ///// <summary>
        ///// Generic 6 axis of a motion event.
        ///// </summary>
        //Generic6 = 37,

        ///// <summary>
        ///// Generic 7 axis of a motion event.
        ///// </summary>
        //Generic7 = 38,

        ///// <summary>
        ///// Generic 8 axis of a motion event.
        ///// </summary>
        //Generic8 = 39,

        ///// <summary>
        ///// Generic 9 axis of a motion event.
        ///// </summary>
        //Generic9 = 40,

        ///// <summary>
        ///// Generic 10 axis of a motion event.
        ///// </summary>
        //Generic10 = 41,

        ///// <summary>
        ///// Generic 11 axis of a motion event.
        ///// </summary>
        //Generic11 = 42,

        ///// <summary>
        ///// Generic 12 axis of a motion event.
        ///// </summary>
        //Generic12 = 43,

        ///// <summary>
        ///// Generic 13 axis of a motion event.
        ///// </summary>
        //Generic13 = 44,

        ///// <summary>
        ///// Generic 14 axis of a motion event.
        ///// </summary>
        //Generic14 = 45,

        ///// <summary>
        ///// Generic 15 axis of a motion event.
        ///// </summary>
        //Generic15 = 46,

        ///// <summary>
        ///// Generic 16 axis of a motion event.
        ///// </summary>
        //Generic16 = 47,
    }
}
#endif // UNITY_EDITOR || UNITY_ANDROID
