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

        public Customisation Custom = new Customisation();

        public class Customisation
        {
            public enum DisplayType
            {
                Original = 0, // Confusing
                Linux = 1,    // [ ] & [*]
                Circular = 2, // ( ) & (o)
                Custom = 3,   // A custom style
                Icecream = -1 // Because why not :)
            }

            public DisplayType SelectedDisplayType = DisplayType.Linux;

            public bool UseCustomKeys = true; // Else, just use the defaults
            public ConsoleKey Up = ConsoleKey.UpArrow;
            public ConsoleKey Down = ConsoleKey.DownArrow;
            public ConsoleKey Exit = ConsoleKey.Escape;
            public ConsoleKey Select = ConsoleKey.Enter;
            public ConsoleKey Finish = ConsoleKey.F;

            public bool UseCustomLabels = true; // Else, just use the defaults
            public string UpLabel = "^";
            public string DownLabel = "v";
            public string ExitLabel = "Esc";
            public string SelectLabel = "Enter";
            public string Finishlabel = "F";
        }

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

            Console.Title = Title == null ? "(Untitled)" : Title;

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

            string _upLabel = Custom.UseCustomLabels ? Custom.UpLabel : "^";
            string _downLabel = Custom.UseCustomLabels ? Custom.DownLabel : "v";
            string _exitLabel = Custom.UseCustomLabels ? Custom.ExitLabel : "Esc";
            string _enterLabel = Custom.UseCustomLabels ? Custom.SelectLabel : "Enter";
            string _finishLabel = Custom.UseCustomLabels ? Custom.Finishlabel : "F";

            string controls = $"^ = Up  v = Down  Esc = Exit  Enter = Select  F = Finish  {(multiSelect ? "Multi-select is enabled." : "")}";

            Console.Write($" {controls}{RepeatChar(' ', Console.BufferWidth - controls.Length - 1)}");

            Console.ResetColor();

            // Draw the options
            if (Title != null) Console.SetCursorPosition(0, 1);

            /*for (int i = 0; i < selectionElements.Count; i++)
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
            }*/

            switch (Custom.SelectedDisplayType)
            {
                case Customisation.DisplayType.Icecream:
                    Console.WriteLine("Icecream flavoured display type :)");
                    RedrawLinuxStyle(); // Draw the default
                    break;
                case Customisation.DisplayType.Original:
                    RedrawOriginalStyle();
                    break;
                case Customisation.DisplayType.Linux:
                    RedrawLinuxStyle();
                    break;
                case Customisation.DisplayType.Circular:
                    RedrawCircularStyle();
                    break;
                case Customisation.DisplayType.Custom:
                    throw new NotImplementedException();
                    break;
                default:
                    break;
            }

            Console.ResetColor();

            ConsoleKeyInfo key = Console.ReadKey();

            if(key.Key == (Custom.UseCustomKeys ? Custom.Up : ConsoleKey.UpArrow))
            {
                if (SelectedIndex > 0) SelectedIndex -= 1;
            } else if(key.Key == (Custom.UseCustomKeys ? Custom.Down : ConsoleKey.DownArrow))
            {
                if (SelectedIndex < selectionElements.Count - 1) SelectedIndex += 1;
            } else if(key.Key == (Custom.UseCustomKeys ? Custom.Select : ConsoleKey.Enter))
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
            } else if(key.Key == (Custom.UseCustomKeys ? Custom.Finish : ConsoleKey.F))
            {
                isCompleted = true;
                if (Completed != null) Completed();
            } else if(key.Key == (Custom.UseCustomKeys ? Custom.Exit : ConsoleKey.Escape))
            {
                SelectedIndex = -1;
                SelectedIndexes.Clear();

                isCompleted = true;
                if (Completed != null) Completed();
            }

            if(!isCompleted) Redraw();
        }

        private void RedrawOriginalStyle()
        {
            for (int i = 0; i < selectionElements.Count; i++)
            {
                if (SelectedIndex == i)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    Console.Write(" ");

                    if (SelectedIndexes.Contains(i)) WriteUnderlined(selectionElements[i].Title);
                    else Console.Write(selectionElements[i].Title);
                } else if (SelectedIndexes.Contains(i))
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
        }

        private void RedrawLinuxStyle()
        {
            for (int i = 0; i < selectionElements.Count; i++)
            {
                if (SelectedIndexes.Contains(i) && multiSelect)
                {
                    Console.Write("[*]");
                } else if(multiSelect)
                {
                    Console.Write("[ ]");
                }

                if (SelectedIndex == i)
                {
                    Console.Write(" ");

                    Console.ForegroundColor = ConsoleColor.Blue;

                    WriteUnderlined(selectionElements[i].Title);
                } else
                {
                    Console.Write(" ");

                    if (selectionElements[i].CustomColor != null) Console.ForegroundColor = (ConsoleColor)selectionElements[i].CustomColor;

                    Console.Write(selectionElements[i].Title);
                }

                Console.ResetColor();
                Console.Write("\n");
            }
        }

        private void RedrawCircularStyle()
        {
            for (int i = 0; i < selectionElements.Count; i++)
            {
                if (SelectedIndexes.Contains(i) && multiSelect)
                {
                    Console.Write("(o)");
                }
                else if (multiSelect)
                {
                    Console.Write("( )");
                }

                if (SelectedIndex == i)
                {
                    Console.Write(" ");

                    Console.ForegroundColor = ConsoleColor.Blue;

                    WriteUnderlined(selectionElements[i].Title);
                }
                else
                {
                    Console.Write(" ");

                    if (selectionElements[i].CustomColor != null) Console.ForegroundColor = (ConsoleColor)selectionElements[i].CustomColor;

                    Console.Write(selectionElements[i].Title);
                }

                Console.ResetColor();
                Console.Write("\n");
            }
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