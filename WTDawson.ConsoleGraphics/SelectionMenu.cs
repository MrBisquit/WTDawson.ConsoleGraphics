using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTDawson.ConsoleGraphics
{
    public class SelectionMenu
    {
        // Variables
        private int SelectedIndex = 0;
        private List<int> SelectedIndexes = new List<int>();

        private int LastBufferWidth = 0;
        private int LastBufferHeight = 0;

        // Properties
        private bool multiSelect = false;

        private bool isCompleted = false;

        // Public properties
        public string? Title = null;
        public bool MultiSelect { get { return multiSelect; } }
        public List<SelectionElement> selectionElements = new List<SelectionElement>();

        public Action? Completed = null;
        public bool IsCompleted { get { return isCompleted; } }

        public SelectionMenu(string? title, List<SelectionElement> elements, bool multiSelect = false)
        {
            Title = title;
            selectionElements = elements;
            this.multiSelect = multiSelect;
        }

        public int GetSelectedIndex() => SelectedIndex;
        public int[] GetSelectedIndexes() => SelectedIndexes.ToArray();

        public class SelectionElement
        {
            public ConsoleColor? CustomColor = null;
            public string Title = "";

            public SelectionElement(string title, ConsoleColor? customColor = null)
            {
                Title = title;
                CustomColor = customColor;
            }
        }

        public void Redraw(bool Clear = false)
        {
            if (Clear || (LastBufferWidth != Console.BufferWidth || LastBufferHeight != Console.BufferHeight)) Console.Clear(); // Completely clear the screen first (Not very good for quickly updating progress bars)

            Console.Title = Title;

            int drawableLines = LastBufferHeight - (Title != null ? 1 : 0) - 1;

            if(drawableLines >= selectionElements.Count)
            {
                Console.WriteLine($"Please resize the console window vertically to be able to fit {selectionElements.Count} lines on.");
                throw new Exception("Invalid Console Buffer Height. Cannot fit all elements.");
            }

            Console.SetCursorPosition(0, 0);

            if(Title != null)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write($" {Title}{RepeatChar(' ', Console.BufferWidth - Title.Length - 1)}");

                Console.ResetColor();
            }

            // Display controls

            Console.SetCursorPosition(0, Console.BufferHeight - 1);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            string controls = $"^ = Up  v = Down  Esc = Exit  Enter = Select  F = Finish  {(multiSelect ? "Multi-select is enabled." : "")}";

            Console.Write($" {controls}{RepeatChar(' ', Console.BufferWidth - controls.Length - 1)}");

            Console.ResetColor();

            // Draw the options
            if (Title != null) Console.SetCursorPosition(0, 1);

            for (int i = 0; i < selectionElements.Count; i++)
            {
                if(SelectedIndex == i)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    Console.Write(" ");

                    if(SelectedIndexes.Contains(i)) WriteUnderlined(selectionElements[i].Title);
                    else Console.Write(selectionElements[i].Title);
                } else if(SelectedIndexes.Contains(i))
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write(" ");

                    WriteUnderlined(selectionElements[i].Title);
                } else
                {
                    Console.ResetColor();

                    if (selectionElements[i].CustomColor != null) Console.ForegroundColor = (ConsoleColor)selectionElements[i].CustomColor;

                    Console.Write(selectionElements[i].Title);
                }

                Console.Write("\n");
                Console.ResetColor();
            }

            Console.ResetColor();

            ConsoleKeyInfo key = Console.ReadKey();

            if(key.Key == ConsoleKey.UpArrow)
            {
                if (SelectedIndex > 0) SelectedIndex -= 1;
            } else if(key.Key == ConsoleKey.DownArrow)
            {
                if (SelectedIndex < selectionElements.Count - 1) SelectedIndex += 1;
            } else if(key.Key == ConsoleKey.Enter)
            {
                if(!multiSelect)
                {
                    isCompleted = true;
                    if(Completed != null) Completed();
                } else
                {
                    if(SelectedIndexes.Contains(SelectedIndex))
                    {
                        SelectedIndexes.Remove(SelectedIndex);
                    } else
                    {
                        SelectedIndexes.Add(SelectedIndex);
                    }
                }
            } else if(key.Key == ConsoleKey.F)
            {
                isCompleted = true;
                if (Completed != null) Completed();
            } else if(key.Key == ConsoleKey.Escape)
            {
                SelectedIndex = -1;
                SelectedIndexes.Clear();

                isCompleted = true;
                if (Completed != null) Completed();
            }

            if(!isCompleted) Redraw();
        }

        /// <summary>
        /// Might not be supported
        /// </summary>
        /// <param name="text">The text to write to the console underlined</param>
        private void WriteUnderlined(string text)
        {
            Console.Write($"\x1B[4m{text}\x1B[0m");
        }

        private string RepeatChar(char c, int count) { string f = ""; for (int i = 0; i < count; i++) f += c; return f; }
    }
}
