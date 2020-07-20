using System;
using System.Collections.Generic;
using GameAssets.World.Scripts;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class WindowManager : MonoBehaviour
    {
        public static List<Window> Windows = new List<Window>();

        private void Start()
        {
            Windows.Clear();
        }

        public static void RegisterWindow(Window window)
        {
            Windows.Add(window);
        }

        public static void RemoveWindow(Window window)
        {
            Windows.Remove(window);
        }
    }
}