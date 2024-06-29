using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTDawson.ConsoleGraphics
{
    public class FileSystemMenu
    {
        private int SelectedIndex = 0;

        private bool _InDriveMenu = false;
        public bool InDriveMenu { get { return _InDriveMenu; } }

        public DriveInfo? SelectedDrive = null;
        public string? SelectedPath = @"C:\";

        private int LastBufferWidth = 0;
        private int LastBufferHeight = 0;

        public string? Title = null;

        public enum Mode
        {
            File = 0,
            MultiFile = 1,
            Folder = 2,
            MultiFolder = 3,
            Drive = 4,
            MultiDrive = 5
        }

        public FileSystemMenu(DriveInfo SelectedDrive, string? title)
        {
            this.SelectedDrive = SelectedDrive;

            _InDriveMenu = false;
            Title = title;
        }

        public FileSystemMenu(string path, string? title)
        {
            bool backSlash = path.Contains("\\");
            string Path = !backSlash ? path.Replace("/", "\\") : path; // New path

            SelectedDrive = new DriveInfo(Path.Split("\\")[0].Split(":")[0]);
            SelectedPath = path;

            _InDriveMenu = false;

            Title = title;
        }

        public FileSystemMenu(string title)
        {
            Title = title;
        }

        public FileSystemMenu() { } // Not neccecary for the user to select a drive or path

        public void Redraw(bool Clear = false)
        {
            if (Clear || (LastBufferWidth != Console.BufferWidth || LastBufferHeight != Console.BufferHeight)) Console.Clear(); // Completely clear the screen first (Not very good for quickly updating progress bars)

            RenderRowHeaders(true, ["Path", "Size", "Last Modified"]);
        }

        public int[]? RenderRowHeaders(bool isheader, string[] items)
        {
            int totalWidth = 0;
            for (int i = 0; i < items.Length; i++)
            {
                totalWidth += items.Length;
            }
            int totalSpacing = LastBufferWidth - totalWidth - 1; // -1 because of the end
            int spacingPerItem = totalSpacing / items.Length;

            int[] spacing = new int[items.Length];

            if(totalSpacing <= 0)
            {
                return null; // Invalid, won't draw
            }

            for (int i = 0; i < items.Length; i++)
            {
                Console.Write($"{items[i]}{(i == (items.Length - 1) ? new string(' ', spacingPerItem) : "")}");
                spacing[i] = spacingPerItem + items[i].Length;
            }

            return spacing;
        }
    }
}
