using UnityEditor.ShortcutManagement;
using UnityEngine;

// ? --- Fa uno screenshot della gameview
public class MyEditorShortcut
{
    static int i = 0;

    [Shortcut("Custom/ScreenshotGameView", KeyCode.F1)]
    static void PrintHello()
    {
        Debug.Log("MadeScreenshot, shot" + i + ".png");
        ScreenCapture.CaptureScreenshot("shot" + i + ".png");
        i++;
    }
}