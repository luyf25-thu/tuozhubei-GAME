#if UNITY_EDITOR || UNITY_OPENHARMONY || PACKAGE_DOCS_GENERATION
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.OpenHarmony.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.OpenHarmony.LowLevel
{
    /// <summary>
    /// Enum used to identity the key in the OpenHarmony key event. See <see cref="OpenHarmonyGameControllerState.buttons"/>.
    /// See https://developer.openharmony.com/reference/openharmony/view/KeyEvent#constants_1 for more details.
    /// </summary>
    public enum OpenHarmonyKeyCode
    {
        /// <summary>
        /// Unknown key code.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Soft Left key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom left of the display.
        /// </summary>
        // SoftLeft = 1,

        /// <summary>
        /// Soft Right key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom right of the display.
        /// </summary>
        // SoftRight = 2,

        /// <summary>
        /// Home key. This key is handled by the framework and is never delivered to applications.
        /// </summary>
        Home = 1,

        /// <summary>
        /// Back key.
        /// </summary>
        Back = 2,

        /// <summary>
        /// Call key.
        /// </summary>
        // Call = 5,

        /// <summary>
        /// End Call key.
        /// </summary>
        // Endcall = 6,

        /// <summary>
        /// '0' key.
        /// </summary>
        Alpha0 = 2000,

        /// <summary>
        /// '1' key.
        /// </summary>
        Alpha1 = 2001,

        /// <summary>
        /// '2' key.
        /// </summary>
        Alpha2 = 2002,

        /// <summary>
        /// '3' key.
        /// </summary>
        Alpha3 = 2003,

        /// <summary>
        /// '4' key.
        /// </summary>
        Alpha4 = 2004,

        /// <summary>
        /// '5' key.
        /// </summary>
        Alpha5 = 2005,

        /// <summary>
        /// '6' key.
        /// </summary>
        Alpha6 = 2006,

        /// <summary>
        /// '7' key.
        /// </summary>
        Alpha7 = 2007,

        /// <summary>
        /// '8' key.
        /// </summary>
        Alpha8 = 2008,

        /// <summary>
        /// '9' key.
        /// </summary>
        Alpha9 = 2009,

        /// <summary>
        /// '*' key.
        /// </summary>
        Star = 2010,

        /// <summary>
        /// '#' key.
        /// </summary>
        Pound = 2011,

        /// <summary>
        /// Directional Pad Up key. May also be synthesized from trackball motions.
        /// </summary>
        DpadUp = 2012,

        /// <summary>
        /// Directional Pad Down key. May also be synthesized from trackball motions.
        /// </summary>
        DpadDown = 2013,

        /// <summary>
        /// Directional Pad Left key. May also be synthesized from trackball motions.
        /// </summary>
        DpadLeft = 2014,

        /// <summary>
        /// Directional Pad Right key. May also be synthesized from trackball motions.
        /// </summary>
        DpadRight = 2015,

        /// <summary>
        /// Directional Pad Center key. May also be synthesized from trackball motions.
        /// </summary>
        DpadCenter = 2016,

        /// <summary>
        /// Volume Up key. Adjusts the speaker volume up.
        /// </summary>
        VolumeUp = 16,

        /// <summary>
        /// Volume Down key. Adjusts the speaker volume down.
        /// </summary>
        VolumeDown = 17,

        /// <summary>
        /// Power key.
        /// </summary>
        Power = 18,

        /// <summary>
        /// Camera key. Used to launch a camera application or take pictures.
        /// </summary>
        Camera = 19,

        /// <summary>
        /// Clear key.
        /// </summary>
        // Clear = 28,

        /// <summary>
        /// 'A' key.
        /// </summary>
        A = 2017,

        /// <summary>
        /// 'B' key.
        /// </summary>
        B = 2018,

        /// <summary>
        /// 'C' key.
        /// </summary>
        C = 2019,

        /// <summary>
        /// 'D' key.
        /// </summary>
        D = 2020,

        /// <summary>
        /// 'E' key.
        /// </summary>
        E = 2021,

        /// <summary>
        /// 'F' key.
        /// </summary>
        F = 2022,

        /// <summary>
        /// 'G' key.
        /// </summary>
        G = 2023,

        /// <summary>
        /// 'H' key.
        /// </summary>
        H = 2024,

        /// <summary>
        /// 'I' key.
        /// </summary>
        I = 2025,

        /// <summary>
        /// 'J' key.
        /// </summary>
        J = 2026,

        /// <summary>
        /// 'K' key.
        /// </summary>
        K = 2027,

        /// <summary>
        /// 'L' key.
        /// </summary>
        L = 2028,

        /// <summary>
        /// 'M' key.
        /// </summary>
        M = 2029,

        /// <summary>
        /// 'N' key.
        /// </summary>
        N = 2030,

        /// <summary>
        /// 'O' key.
        /// </summary>
        O = 2031,

        /// <summary>
        /// 'P' key.
        /// </summary>
        P = 2032,

        /// <summary>
        /// 'Q' key.
        /// </summary>
        Q = 2033,

        /// <summary>
        /// 'R' key.
        /// </summary>
        R = 2034,

        /// <summary>
        /// 'S' key.
        /// </summary>
        S = 2035,

        /// <summary>
        /// 'T' key.
        /// </summary>
        T = 2036,

        /// <summary>
        /// 'U' key.
        /// </summary>
        U = 2037,

        /// <summary>
        /// 'V' key.
        /// </summary>
        V = 2038,

        /// <summary>
        /// 'W' key.
        /// </summary>
        W = 2039,

        /// <summary>
        /// 'X' key.
        /// </summary>
        X = 2040,

        /// <summary>
        /// 'Y' key.
        /// </summary>
        Y = 2041,

        /// <summary>
        /// 'Z' key.
        /// </summary>
        Z = 2042,

        /// <summary>
        /// ',' key.
        /// </summary>
        Comma = 2043,

        /// <summary>
        /// '.' key.
        /// </summary>
        Period = 2044,

        /// <summary>
        /// Left Alt modifier key.
        /// </summary>
        AltLeft = 2045,

        /// <summary>
        /// Right Alt modifier key.
        /// </summary>
        AltRight = 2046,

        /// <summary>
        /// Left Shift modifier key.
        /// </summary>
        ShiftLeft = 2047,

        /// <summary>
        /// Right Shift modifier key.
        /// </summary>
        ShiftRight = 2048,

        /// <summary>
        /// Tab key.
        /// </summary>
        Tab = 2049,

        /// <summary>
        /// Space key.
        /// </summary>
        Space = 2050,

        /// <summary>
        /// Symbol modifier key. Used to enter alternate symbols.
        /// </summary>
        Sym = 2051,

        /// <summary>
        /// Explorer special function key. Used to launch a browser application.
        /// </summary>
        Explorer = 2052,

        /// <summary>
        /// Envelope special function key. Used to launch a mail application.
        /// </summary>
        Envelope = 2053,

        /// <summary>
        /// Enter key.
        /// </summary>
        Enter = 2054,

        /// <summary>
        /// Backspace key. Deletes characters before the insertion point, unlike <see cref="AndroidKeyCode.ForwardDel"/>.
        /// </summary>
        Del = 2055,

        /// <summary>
        /// '`' (backtick) key.
        /// </summary>
        Grave = 2056,

        /// <summary>
        /// '-' key.
        /// </summary>
        Minus = 2057,

        /// <summary>
        /// '=' key.
        /// </summary>
        Equals = 2058,

        /// <summary>
        /// '[' key.
        /// </summary>
        LeftBracket = 2059,

        /// <summary>
        /// ']' key.
        /// </summary>
        RightBracket = 2060,

        /// <summary>
        /// '\' key.
        /// </summary>
        Backslash = 2061,

        /// <summary>
        /// ';' key.
        /// </summary>
        Semicolon = 2062,

        /// <summary>
        /// ''' (apostrophe) key.
        /// </summary>
        Apostrophe = 2063,

        /// <summary>
        /// '/' key.
        /// </summary>
        Slash = 2064,

        /// <summary>
        /// '@' key.
        /// </summary>
        At = 2065,

        /// <summary>
        /// Number modifier key. Used to enter numeric symbols. This key is not Num Lock; it is more like <see cref="AndroidKeyCode.AltLeft"/>.
        /// </summary>
        // Num = 78,

        /// <summary>
        /// Headset Hook key. Used to hang up calls and stop media.
        /// </summary>
        // Headsethook = 79,

        /// <summary>
        /// Camera Focus key. Used to focus the camera.
        /// </summary>
        // Focus = 80,

        /// <summary>
        /// '+' key.
        /// </summary> // *Camera* focus
        Plus = 2066,

        /// <summary>
        /// Menu key.
        /// </summary>
        Menu = 2067,

        /// <summary>
        /// Notification key.
        /// </summary>
        // Notification = 83,

        /// <summary>
        /// Search key.
        /// </summary>
        Search = 9,

        /// <summary>
        /// Play/Pause media key.
        /// </summary>
        MediaPlayPause = 10,

        /// <summary>
        /// Stop media key.
        /// </summary>
        MediaStop = 11,

        /// <summary>
        /// Play Next media key.
        /// </summary>
        MediaNext = 12,

        /// <summary>
        /// Play Previous media key.
        /// </summary>
        MediaPrevious = 13,

        /// <summary>
        /// Rewind media key.
        /// </summary>
        MediaRewind = 14,

        /// <summary>
        /// Fast Forward media key.
        /// </summary>
        MediaFastForward = 15,

        /// <summary>
        /// Mute key. Mutes the microphone, unlike <see cref="AndroidKeyCode.VolumeMute"/>.
        /// </summary>
        Mute = 23,

        /// <summary>
        /// Page Up key.
        /// </summary>
        PageUp = 2068,

        /// <summary>
        /// Page Down key.
        /// </summary>
        PageDown = 2069,

        /// <summary>
        /// Picture Symbols modifier key. Used to switch symbol sets (Emoji, Kao-moji).
        /// </summary>
        // Pictsymbols = 94,

        /// <summary>
        /// Switch Charset modifier key. Used to switch character sets (Kanji, Katakana).
        /// </summary>
        // SwitchCharset = 95,

        /// <summary>
        /// A Button key. On a game controller, the A button should be either the button labeled A or the first button on the bottom row of controller buttons.
        /// </summary>
        ButtonA = 2301,

        /// <summary>
        /// B Button key. On a game controller, the B button should be either the button labeled B or the second button on the bottom row of controller buttons.
        /// </summary>
        ButtonB = 2302,

        /// <summary>
        /// C Button key. On a game controller, the C button should be either the button labeled C or the third button on the bottom row of controller buttons.
        /// </summary>
        ButtonC = 2303,

        /// <summary>
        /// X Button key. On a game controller, the X button should be either the button labeled X or the first button on the upper row of controller buttons.
        /// </summary>
        ButtonX = 2304,

        /// <summary>
        /// Y Button key. On a game controller, the Y button should be either the button labeled Y or the second button on the upper row of controller buttons.
        /// </summary>
        ButtonY = 2305,

        /// <summary>
        /// Z Button key. On a game controller, the Z button should be either the button labeled Z or the third button on the upper row of controller buttons.
        /// </summary>
        ButtonZ = 2306,

        /// <summary>
        /// L1 Button key. On a game controller, the L1 button should be either the button labeled L1 (or L) or the top left trigger button.
        /// </summary>
        ButtonL1 = 2307,

        /// <summary>
        /// R1 Button key. On a game controller, the R1 button should be either the button labeled R1 (or R) or the top right trigger button.
        /// </summary>
        ButtonR1 = 2308,

        /// <summary>
        /// L2 Button key. On a game controller, the L2 button should be either the button labeled L2 or the bottom left trigger button.
        /// </summary>
        ButtonL2 = 2309,

        /// <summary>
        /// R2 Button key. On a game controller, the R2 button should be either the button labeled R2 or the bottom right trigger button.
        /// </summary>
        ButtonR2 = 2310,

        /// <summary>
        /// Left Thumb Button key. On a game controller, the left thumb button indicates that the left (or only) joystick is pressed.
        /// </summary>
        ButtonThumbl = 2314,

        /// <summary>
        /// Right Thumb Button key. On a game controller, the right thumb button indicates that the right joystick is pressed.
        /// </summary>
        ButtonThumbr = 2315,

        /// <summary>
        /// Start Button key. On a game controller, the button labeled Start.
        /// </summary>
        ButtonStart = 2312,

        /// <summary>
        /// Select Button key. On a game controller, the button labeled Select.
        /// </summary>
        ButtonSelect = 2311,

        /// <summary>
        /// Mode Button key. On a game controller, the button labeled Mode.
        /// </summary>
        ButtonMode = 2313,

        /// <summary>
        /// Escape key.
        /// </summary>
        Escape = 2070,

        /// <summary>
        /// Forward Delete key. Deletes characters ahead of the insertion point, unlike <see cref="AndroidKeyCode.Del"/>.
        /// </summary>
        ForwardDel = 2071,

        /// <summary>
        /// Left Control modifier key.
        /// </summary>
        CtrlLeft = 2072,

        /// <summary>
        /// Right Control modifier key.
        /// </summary>
        CtrlRight = 2073,

        /// <summary>
        /// Caps Lock key.
        /// </summary>
        CapsLock = 2074,

        /// <summary>
        /// Scroll Lock key.
        /// </summary>
        ScrollLock = 2075,

        /// <summary>
        /// Left Meta modifier key.
        /// </summary>
        MetaLeft = 2076,

        /// <summary>
        /// Right Meta modifier key.
        /// </summary>
        MetaRight = 2077,

        /// <summary>
        /// Function modifier key.
        /// </summary>
        Function = 2078,

        /// <summary>
        /// System Request / Print Screen key.
        /// </summary>
        Sysrq = 2079,

        /// <summary>
        /// Break / Pause key.
        /// </summary>
        Break = 2080,

        /// <summary>
        /// Home Movement key. Used for scrolling or moving the cursor around to the start of a line or to the top of a list.
        /// </summary>
        MoveHome = 2081,

        /// <summary>
        /// End Movement key. Used for scrolling or moving the cursor around to the end of a line or to the bottom of a list.
        /// </summary>
        MoveEnd = 2082,

        /// <summary>
        /// Insert key. Toggles insert / overwrite edit mode.
        /// </summary>
        Insert = 2083,

        /// <summary>
        /// Forward key. Navigates forward in the history stack. Complement of <see cref="AndroidKeyCode.Back"/>.
        /// </summary>
        Forward = 2084,

        /// <summary>
        /// Play media key.
        /// </summary>
        MediaPlay = 2085,

        /// <summary>
        /// Play/Pause media key.
        /// </summary>
        MediaPause = 2086,

        /// <summary>
        /// Close media key. May be used to close a CD tray, for example.
        /// </summary>
        MediaClose = 2087,

        /// <summary>
        /// Eject media key. May be used to eject a CD tray, for example.
        /// </summary>
        MediaEject = 2088,

        /// <summary>
        /// Record media key.
        /// </summary>
        MediaRecord = 2089,

        /// <summary>
        /// F1 key.
        /// </summary>
        F1 = 2090,

        /// <summary>
        /// F2 key.
        /// </summary>
        F2 = 2091,

        /// <summary>
        /// F3 key.
        /// </summary>
        F3 = 2092,

        /// <summary>
        /// F4 key.
        /// </summary>
        F4 = 2093,

        /// <summary>
        /// F5 key.
        /// </summary>
        F5 = 2094,

        /// <summary>
        /// F6 key.
        /// </summary>
        F6 = 2095,

        /// <summary>
        /// F7 key.
        /// </summary>
        F7 = 2096,

        /// <summary>
        /// F8 key.
        /// </summary>
        F8 = 2097,

        /// <summary>
        /// F9 key.
        /// </summary>
        F9 = 2098,

        /// <summary>
        /// F10 key.
        /// </summary>
        F10 = 2099,

        /// <summary>
        /// F11 key.
        /// </summary>
        F11 = 2100,

        /// <summary>
        /// F12 key.
        /// </summary>
        F12 = 2101,

        /// <summary>
        /// Num Lock key. This is the Num Lock key; it is different from <see cref="AndroidKeyCode.Num"/>. This key alters the behavior of other keys on the numeric keypad.
        /// </summary>
        NumLock = 2102,

        /// <summary>
        /// Numeric keypad '0' key.
        /// </summary>
        Numpad0 = 2103,

        /// <summary>
        /// Numeric keypad '1' key.
        /// </summary>
        Numpad1 = 2104,

        /// <summary>
        /// Numeric keypad '2' key.
        /// </summary>
        Numpad2 = 2105,

        /// <summary>
        /// Numeric keypad '3' key.
        /// </summary>
        Numpad3 = 2106,

        /// <summary>
        /// Numeric keypad '4' key.
        /// </summary>
        Numpad4 = 2107,

        /// <summary>
        /// Numeric keypad '5' key.
        /// </summary>
        Numpad5 = 2108,

        /// <summary>
        /// 'Numeric keypad '6' key.
        /// </summary>
        Numpad6 = 2109,

        /// <summary>
        /// 'Numeric keypad '7' key.
        /// </summary>
        Numpad7 = 2110,

        /// <summary>
        /// Numeric keypad '8' key.
        /// </summary>
        Numpad8 = 2111,

        /// <summary>
        /// Numeric keypad '9' key.
        /// </summary>
        Numpad9 = 2112,

        /// <summary>
        /// Numeric keypad '/' key (for division).
        /// </summary>
        NumpadDivide = 2113,

        /// <summary>
        /// Numeric keypad '*' key (for multiplication).
        /// </summary>
        NumpadMultiply = 2114,

        /// <summary>
        /// Numeric keypad '-' key (for subtraction).
        /// </summary>
        NumpadSubtract = 2115,

        /// <summary>
        /// Numeric keypad '+' key (for addition).
        /// </summary>
        NumpadAdd = 2116,

        /// <summary>
        /// Numeric keypad '.' key (for decimals or digit grouping).
        /// </summary>
        NumpadDot = 2117,

        /// <summary>
        /// Numeric keypad ',' key (for decimals or digit grouping).
        /// </summary>
        NumpadComma = 2118,

        /// <summary>
        /// Numeric keypad Enter key.
        /// </summary>
        NumpadEnter = 2119,

        /// <summary>
        /// Numeric keypad '=' key.
        /// </summary>
        NumpadEquals = 2120,

        /// <summary>
        /// Numeric keypad '(' key.
        /// </summary>
        NumpadLeftParen = 2121,

        /// <summary>
        /// Numeric keypad ')' key.
        /// </summary>
        NumpadRightParen = 2122,

        /// <summary>
        /// Volume Mute key. Mutes the speaker, unlike <see cref="AndroidKeyCode.Mute"/>. This key should normally be implemented as a toggle such that the first press mutes the speaker and the second press restores the original volum
        /// </summary>
        VolumeMute = 22,

        /// <summary>
        /// Info key. Common on TV remotes to show additional information related to what is currently being viewed.
        /// </summary>
        Info = 2664,

        /// <summary>
        /// Channel up key. On TV remotes, increments the television channel.
        /// </summary>
        ChannelUp = 2690,

        /// <summary>
        /// Channel down key. On TV remotes, increments the television channel.
        /// </summary>
        ChannelDown = 2691,

        /// <summary>
        /// Zoom in key.
        /// </summary>
        ZoomIn = 2698,

        /// <summary>
        /// Zoom out key.
        /// </summary>
        ZoomOut = 2699,

        /// <summary>
        /// TV key. On TV remotes, switches to viewing live TV.
        /// </summary>
        Tv = 2672,

        /// <summary>
        /// Window key. On TV remotes, toggles picture-in-picture mode or other windowing functions. On Android Wear devices, triggers a display offset.
        /// </summary>
        // Window = 2811,

        /// <summary>
        /// Guide key. On TV remotes, shows a programming guide.
        /// </summary>
        // Guide = 172,

        /// <summary>
        /// DVR key. On some TV remotes, switches to a DVR mode for recorded shows.
        /// </summary>
        // Dvr = 173,

        /// <summary>
        /// Bookmark key. On some TV remotes, bookmarks content or web pages.
        /// </summary>
        Bookmark = 2628,

        /// <summary>
        /// Toggle captions key. Switches the mode for closed-captioning text, for example during television shows.
        /// </summary>
        // Captions = 175,

        /// <summary>
        /// Settings key. Starts the system settings activity.
        /// </summary>
        // Settings = 176,

        /// <summary>
        /// TV power key. On HDMI TV panel devices and Android TV devices that don't support HDMI, toggles the power state of the device. On HDMI source devices, toggles the power state of the HDMI-connected TV via HDMI-CEC and makes the source device follow this power state.
        /// </summary>
        // TvPower = 177,

        /// <summary>
        /// TV input key. On TV remotes, switches the input on a television screen.
        /// </summary>
        // TvInput = 178,

        /// <summary>
        /// Set-top-box power key. On TV remotes, toggles the power on an external Set-top-box.
        /// </summary>
        // StbPower = 179,

        /// <summary>
        /// Set-top-box input key. On TV remotes, switches the input mode on an external Set-top-box.
        /// </summary>
        // StbInput = 180,

        /// <summary>
        /// A/V Receiver power key. On TV remotes, toggles the power on an external A/V Receiver.
        /// </summary>
        // AvrPower = 181,

        /// <summary>
        /// A/V Receiver input key. On TV remotes, switches the input mode on an external A/V Receive
        /// </summary>
        // AvrInput = 182,

        /// <summary>
        /// Red "programmable" key. On TV remotes, acts as a contextual/programmable key.
        /// </summary>
        // ProgRed = 183,

        /// <summary>
        /// Green "programmable" key. On TV remotes, actsas a contextual/programmable key.
        /// </summary>
        // ProgGreen = 184,

        /// <summary>
        /// Yellow "programmable" key. On TV remotes, actsas a contextual/programmable key.
        /// </summary>
        // ProgYellow = 185,

        /// <summary>
        /// Blue "programmable" key. On TV remotes, actsas a contextual/programmable key.
        /// </summary>
        // ProgBlue = 186,

        /// <summary>
        /// App switch key. Should bring up the application switcher dialog.
        /// </summary>
        // AppSwitch = 187,

        /// <summary>
        /// Generic Game Pad Button #1.
        /// </summary>
        //Button1 = 188,

        /// <summary>
        /// Generic Game Pad Button #2.
        /// </summary>
        //Button2 = 189,

        /// <summary>
        /// Generic Game Pad Button #3.
        /// </summary>
        //Button3 = 190,

        /// <summary>
        /// Generic Game Pad Button #4.
        /// </summary>
        //Button4 = 191,

        /// <summary>
        /// Generic Game Pad Button #5.
        /// </summary>
        //Button5 = 192,

        /// <summary>
        /// Generic Game Pad Button #6.
        /// </summary>
        //Button6 = 193,

        /// <summary>
        /// Generic Game Pad Button #7.
        /// </summary>
        //Button7 = 194,

        /// <summary>
        /// Generic Game Pad Button #8.
        /// </summary>
        //Button8 = 195,

        /// <summary>
        /// Generic Game Pad Button #9.
        /// </summary>
        //Button9 = 196,

        /// <summary>
        /// Generic Game Pad Button #10.
        /// </summary>
        //Button10 = 197,

        /// <summary>
        /// Generic Game Pad Button #11.
        /// </summary>
        //Button11 = 198,

        /// <summary>
        /// Generic Game Pad Button #12.
        /// </summary>
        //Button12 = 199,

        /// <summary>
        /// Generic Game Pad Button #13.
        /// </summary>
        //Button13 = 200,

        /// <summary>
        /// Generic Game Pad Button #14.
        /// </summary>
        //Button14 = 201,

        /// <summary>
        /// Generic Game Pad Button #15.
        /// </summary>
        //Button15 = 202,

        /// <summary>
        /// Generic Game Pad Button #16.
        /// </summary>
        //Button16 = 203,

        /// <summary>
        /// Language Switch key. Toggles the current input language such as switching between English and Japanese on a QWERTY keyboard. On some devices, the same function may be performed by pressing Shift+Spacebar.
        /// </summary>
        // LanguageSwitch = 204,

        /// <summary>
        /// 'Manner Mode key. Toggles silent or vibrate mode on and off to make the device behave more politely in certain settings such as on a crowded train. On some devices, the key may only operate when long-pressed.
        /// </summary>
        // MannerMode = 205,

        /// <summary>
        /// 3D Mode key. Toggles the display between 2D and 3D mode.
        /// </summary>
        // Mode3D = 206,

        /// <summary>
        /// Contacts special function key. Used to launch an address book application.
        /// </summary>
        // Contacts = 207,

        /// <summary>
        /// Calendar special function key. Used to launch a calendar application.
        /// </summary>
        Calendar = 2685,

        /// <summary>
        /// Music special function key. Used to launch a music player application.
        /// </summary>
        // Music = 209,

        /// <summary>
        /// Calculator special function key. Used to launch a calculator application.
        /// </summary>
        // Calculator = 210,

        /// <summary>
        /// Japanese full-width / half-width key.
        /// </summary>
        // ZenkakuHankaku = 211,

        /// <summary>
        /// Japanese alphanumeric key.
        /// </summary>
        // Eisu = 212,

        /// <summary>
        /// Japanese non-conversion key.
        /// </summary>
        // Muhenkan = 213,

        /// <summary>
        /// Japanese conversion key.
        /// </summary>
        // Henkan = 214,

        /// <summary>
        /// Japanese katakana / hiragana key.
        /// </summary>
        // KatakanaHiragana = 215,

        /// <summary>
        /// Japanese Yen key.
        /// </summary>
        // Yen = 216,

        /// <summary>
        /// Japanese Ro key.
        /// </summary>
        // Ro = 217,

        /// <summary>
        /// Japanese kana key.
        /// </summary>
        // Kana = 218,

        /// <summary>
        /// Assist key. Launches the global assist activity. Not delivered to applications.
        /// </summary>
        Assist = 2722,
    }
}

#endif // UNITY_EDITOR || UNITY_ANDROID
