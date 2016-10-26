using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System;
namespace DiscordLCD
{
    public class LogitechGSDK
    {
        //LCD	SDK
        public const int LOGI_LCD_COLOR_BUTTON_LEFT = (0x00000100);
        public const int LOGI_LCD_COLOR_BUTTON_RIGHT = (0x00000200);
        public const int LOGI_LCD_COLOR_BUTTON_OK = (0x00000400);
        public const int LOGI_LCD_COLOR_BUTTON_CANCEL = (0x00000800);
        public const int LOGI_LCD_COLOR_BUTTON_UP = (0x00001000);
        public const int LOGI_LCD_COLOR_BUTTON_DOWN = (0x00002000);
        public const int LOGI_LCD_COLOR_BUTTON_MENU = (0x00004000);

        public const int LOGI_LCD_MONO_BUTTON_0 = (0x00000001);
        public const int LOGI_LCD_MONO_BUTTON_1 = (0x00000002);
        public const int LOGI_LCD_MONO_BUTTON_2 = (0x00000004);
        public const int LOGI_LCD_MONO_BUTTON_3 = (0x00000008);

        public const int LOGI_LCD_MONO_WIDTH = 160;
        public const int LOGI_LCD_MONO_HEIGHT = 43;

        public const int LOGI_LCD_COLOR_WIDTH = 320;
        public const int LOGI_LCD_COLOR_HEIGHT = 240;

        public const int LOGI_LCD_TYPE_MONO = (0x00000001);
        public const int LOGI_LCD_TYPE_COLOR = (0x00000002);


        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdInit(String friendlyName, int lcdType);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsConnected(int lcdType);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsButtonPressed(int button);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdUpdate();

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdShutdown();

        //	Monochrome	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetBackground(byte[] monoBitmap);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetText(int lineNumber, String text);

        //	Color	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetBackground(byte[] colorBitmap);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetTitle(String text, int red, int green, int blue);

        [DllImport("LogitechLcdEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetText(int lineNumber, String text, int red, int green, int blue);
    }

    public class LogitechArx
    {
        // Arx SDK

        public const int LOGI_ARX_ORIENTATION_PORTRAIT = 0x01;
        public const int LOGI_ARX_ORIENTATION_LANDSCAPE = 0x10;
        public const int LOGI_ARX_EVENT_FOCUS_ACTIVE = 0x01;
        public const int LOGI_ARX_EVENT_FOCUS_INACTIVE = 0x02;
        public const int LOGI_ARX_EVENT_TAP_ON_TAG = 0x04;
        public const int LOGI_ARX_EVENT_MOBILEDEVICE_ARRIVAL = 0x08;
        public const int LOGI_ARX_EVENT_MOBILEDEVICE_REMOVAL = 0x10;
        public const int LOGI_ARX_DEVICETYPE_IPHONE = 0x01;
        public const int LOGI_ARX_DEVICETYPE_IPAD = 0x02;
        public const int LOGI_ARX_DEVICETYPE_ANDROID_SMALL = 0x03;
        public const int LOGI_ARX_DEVICETYPE_ANDROID_NORMAL = 0x04;
        public const int LOGI_ARX_DEVICETYPE_ANDROID_LARGE = 0x05;
        public const int LOGI_ARX_DEVICETYPE_ANDROID_XLARGE = 0x06;
        public const int LOGI_ARX_DEVICETYPE_ANDROID_OTHER = 0x07;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void logiArxCB(int eventType, int eventValue, [MarshalAs(UnmanagedType.LPWStr)]String eventArg, IntPtr context);

        public struct logiArxCbContext
        {
            public logiArxCB arxCallBack;
            public IntPtr arxContext;
        }

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxInit(String identifier, String friendlyName, ref logiArxCbContext callback);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxInitWithIcon(String identifier, String friendlyName, ref logiArxCbContext callback, byte[] iconBitmap);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxAddFileAs(String filePath, String fileName, String mimeType = "");

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxAddContentAs(byte[] content, int size, String fileName, String mimeType = "");

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxAddUTF8StringAs(String stringContent, String fileName, String mimeType = "");

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxAddImageFromBitmap(byte[] bitmap, int width, int height, String fileName);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxSetIndex(String fileName);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxSetTagPropertyById(String tagId, String prop, String newValue);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxSetTagsPropertyByClass(String tagsClass, String prop, String newValue);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxSetTagContentById(String tagId, String newContent);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiArxSetTagsContentByClass(String tagsClass, String newContent);

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LogiArxGetLastError();

        [DllImport("LogitechGArxControlEnginesWrapper.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiArxShutdown();
    }
}