﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Dalamud.Game.ClientState.Keys;
using ECommons.Interop;

namespace SomethingNeedDoing.Misc;

/// <summary>
/// Simulate pressing keyboard input.
/// </summary>
internal static class Keyboard
{
    private static IntPtr? handle = null;

    /// <summary>
    /// Send a virtual key.
    /// </summary>
    /// <param name="key">Key to send.</param>
    public static void Send(VirtualKey key) => Send(key, null);

    /// <summary>
    /// Send a virtual key with modifiers.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="mods">Modifiers to press.</param>
    public static void Send(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        if (key != 0)
        {
            //var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;
            if (WindowFunctions.TryFindGameWindow(out var hWnd))
            {
                if (mods != null)
                {
                    foreach (var mod in mods)
                        _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)mod, IntPtr.Zero);
                }

                _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
                Thread.Sleep(100);
                _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);

                if (mods != null)
                {
                    foreach (var mod in mods)
                        _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)mod, IntPtr.Zero);
                }
            }
        }
    }

    public static void Hold(VirtualKey key) => Hold(key, null);

    public static void Hold(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYDOWN = 0x100;

        if (key != 0)
        {
            //var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;
            if (WindowFunctions.TryFindGameWindow(out var hWnd))
            {
                if (mods != null)
                {
                    foreach (var mod in mods)
                        _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)mod, IntPtr.Zero);
                }

                _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            }
        }
    }

    public static void Release (VirtualKey key) => Release(key, null);

    public static void Release(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYUP = 0x101;

        if (key != 0)
        {
            //var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;
            if (WindowFunctions.TryFindGameWindow(out var hWnd))
            {
                _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);

                if (mods != null)
                {
                    foreach (var mod in mods)
                        _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)mod, IntPtr.Zero);
                }
            }
        }
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
}
