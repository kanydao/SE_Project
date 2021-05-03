using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MyWindows 
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    [DllImport("user32.dll")]
    static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;  
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;
    const int SW_SHOWMINIMIZED = 2;

    static public void SetWindowsSize(int cx,int cy)
    {
        Resolution[] resolutions = Screen.resolutions;
        int screen_width = resolutions[resolutions.Length - 1].width;
        int screen_height = resolutions[resolutions.Length - 1].height;
        int X = (screen_width - cx) / 2;
        int Y = (screen_height - cy) / 2;
        SetWindowPos(GetForegroundWindow(), 0, X, Y, cx, cy, SWP_SHOWWINDOW);
    }

    static public void Minimized()
    {
        ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
    }

    static public void SetWindowsNoFrame()
    {
        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);
    }

    static public void WindowsMove(RectTransform move_area, RectTransform[] exclude_area = null)
    {
        bool is_move = false;
        bool is_in_exclude_area = false;
        if(exclude_area!=null)
        {
            foreach (RectTransform rectTransform in exclude_area)
            {
                if(RectTransformUtility.RectangleContainsScreenPoint(rectTransform,Input.mousePosition))
                {
                    is_in_exclude_area = true;
                    break;
                }
            }
        }
        
        if (RectTransformUtility.RectangleContainsScreenPoint(move_area, Input.mousePosition)
        && !is_in_exclude_area)
        {
            if (Input.GetMouseButtonDown(0))
            {
                is_move = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                is_move = false;
            }

            if (is_move)
            {
                ReleaseCapture();
                SendMessage(GetForegroundWindow(), 0xA1, 0x02, 0);
                SendMessage(GetForegroundWindow(), 0x0202, 0, 0);
            }
        }
    }
}
