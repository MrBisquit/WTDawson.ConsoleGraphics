using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace WTDawson.ConsoleGraphics
{
    public class FileSystemMenu
    {
        private bool multiSelect = false;
        private string currentDirectory = "";
        private FileInfo[]? files = null;
        private List<FileInfo> selectedFiles = new List<FileInfo>();

        private int LastBufferWidth = 0;
        private int LastBufferHeight = 0;

        private int _startIndex = 0;
        private int _endIndex = 0;

        public string? Title = null;
        private string? _lastTitle = null;

        private FileSystemMenuResult _beforeResult = new FileSystemMenuResult();

        private int selectedIndex = 0;
        private int maxIndex = 0;
        private bool selectingDrives = false;

        private bool _selectEnabled = true;
        private bool _enterEnabled = true;

        public FileSystemMenu(string? title, bool multiSelect, string? startingDirectory)
        {
            Title = title;
            this.multiSelect = multiSelect;

            if(startingDirectory == null)
            {
                selectingDrives = true;
                return;
            }

            if(Directory.Exists(startingDirectory))
            {
                currentDirectory = startingDirectory;
            } else
            {
                throw new Exception("Invalid path, directory not found.");
            }
        }

        public class FileSystemMenuResult
        {
            public FileInfo[] SelectedFiles;
            public FileInfo SelectedFile;
        }
        
        // I'll use the Linux-style selection menu because It's better than my original one.
        public FileSystemMenuResult RequestFileSelection()
        {
            return DrawFS();
        }

        private FileSystemMenuResult DrawFS(bool Clear = false)
        {
            bool finished = false;

            RedrawFS(true);

            _lastTitle = Title;

            while(!finished)
            {
                bool requireFullRedraw = false;
                if (Clear || (LastBufferWidth != Console.BufferWidth || LastBufferHeight != Console.BufferHeight)) { Console.Clear(); requireFullRedraw = true; } // Completely clear the screen first

                Console.Title = Title == null ? "(Untitled)" : Title;

                if(Title != _lastTitle)
                {
                    _lastTitle = Title;
                    requireFullRedraw = true;
                }

                RedrawFS(requireFullRedraw);

                ConsoleKeyInfo key = Console.ReadKey();

                if(key.Key == ConsoleKey.UpArrow)
                {
                    if (selectedIndex <= _startIndex - 1)
                    {
                        _startIndex -= 1;
                        _endIndex -= 1;
                    }

                    if (selectedIndex != 0) selectedIndex -= 1;
                } else if(key.Key == ConsoleKey.DownArrow)
                {
                    if(selectedIndex >= _endIndex + 1)
                    {
                        _startIndex += 1;
                        _endIndex += 1;
                    }

                    if(selectedIndex != maxIndex - 1) selectedIndex += 1;
                } else if(key.Key == ConsoleKey.Spacebar && _selectEnabled)
                {

                } else if(key.Key == ConsoleKey.Enter && _enterEnabled)
                {
                    selectedIndex = 0;

                    if(selectingDrives)
                    {
                        DriveInfo[] drives = DriveInfo.GetDrives();

                        currentDirectory = drives[selectedIndex].Name;
                        _selectEnabled = true;
                        selectingDrives = false;

                        _endIndex = Console.BufferHeight - 2;
                    } else
                    {
                        if(selectedIndex == 0 && currentDirectory.Split("/").Length == 1)
                        {
                            selectingDrives = true;
                        }
                    }
                }

                //Console.WriteLine(selectedIndex + ":" + maxIndex);
                //Console.WriteLine(_startIndex + ":" + _endIndex);
                //Console.ReadKey();
            }

            return _beforeResult;
        }

        private void RedrawFS(bool fullRedraw = false)
        {
            if(fullRedraw)
            {
                Console.SetCursorPosition(0, 0);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;

                string _title = $" {currentDirectory}{(Title != null ? $" - {Title}" : "")}";

                Console.Write($"{_title}{RepeatChar(' ', Console.BufferWidth - _title.Length - 1)}");
            }

            Console.ResetColor();

            Console.SetCursorPosition(0, Console.BufferHeight - 1);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write(" ^ = Up  v = Down  ");

            if (!_selectEnabled) Console.ForegroundColor = ConsoleColor.Gray;
            else Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Space = Select  ");

            if (!_enterEnabled) Console.ForegroundColor = ConsoleColor.Gray;
            else Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Enter = Finish  ");

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write($"{(multiSelect ? "Multi-select is enabled." : "")}");

            string controls = $"^ = Up  v = Down  Space = Select  Enter = Finish  {(multiSelect ? "Multi-select is enabled." : "")}";

            //Console.Write($" {controls}{RepeatChar(' ', Console.BufferWidth - controls.Length - 1)}");
            Console.Write($"{RepeatChar(' ', Console.BufferWidth - controls.Length - 1)}");

            Console.ResetColor();

            if(selectingDrives)
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                maxIndex = drives.Length;

                _selectEnabled = false;

                Console.SetCursorPosition(0, 1);

                for (int i = 0; i < drives.Length; i++)
                {
                    if(selectedIndex == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Black;
                        
                        if(!drives[i].IsReady) _enterEnabled = false;
                        else _enterEnabled = true;
                    } else
                    {
                        Console.ResetColor();
                    }

                    if(!drives[i].IsReady)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.WriteLine(drives[i].Name + RepeatChar(' ', Console.BufferWidth - drives[i].Name.Length - 1));

                    Console.ResetColor();
                }
            } else
            {
                if (!Directory.Exists(currentDirectory))
                {
                    selectingDrives = true;
                }

                _selectEnabled = true;

                Console.SetCursorPosition(0, 1);

                string[] dirs = Directory.GetDirectories(currentDirectory);
                string[] files = Directory.GetFiles(currentDirectory);

                maxIndex = dirs.Length + files.Length;

                if (selectedIndex == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ResetColor();
                }
                Console.WriteLine(".." + RepeatChar(' ', Console.BufferWidth - "..".Length - 1));

                int _aP = 2;
                int _fColW = Console.BufferWidth / 4; // 20
                int _sColW = Console.BufferWidth / 10; // 20

                for (int i = 0; i < dirs.Length; i++)
                {
                    int _sE = selectedIndex - 1;

                    if (_sE <= _startIndex - 1) continue;

                    DirectoryInfo dir = new DirectoryInfo(dirs[i]);

                    if (i == _sE)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Black;
                    } else
                    {
                        Console.ResetColor();
                    }

                    if(dir.Attributes == FileAttributes.Hidden)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    string _dirname = CutOffEnd("<DIR> " + dir.Name, _fColW);

                    Console.WriteLine(_dirname + RepeatChar(' ', Console.BufferWidth - _dirname.Length - 1));
                    Console.SetCursorPosition(_fColW + _aP, Console.CursorTop - 1);
                    try
                    {
                        Console.Write($"{CutOffEnd(FormatBytes(GetBytesFromDir(dir)), _sColW)}");
                    } catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("N/A");
                        Console.ResetColor();
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }

                for (int i = 0; i < files.Length; i++)
                {
                    int _sE = selectedIndex - 1 + dirs.Length;

                    if (_sE <= _startIndex - 1) continue;

                    FileInfo file = new FileInfo(files[i]);

                    if (i + dirs.Length == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ResetColor();
                    }

                    string _filename = CutOffEnd(file.Name, _fColW);

                    Console.WriteLine(_filename + RepeatChar(' ', Console.BufferWidth - _filename.Length - 1));
                    Console.SetCursorPosition(_fColW + _aP, Console.CursorTop - 1);
                    try
                    {
                        Console.Write($"{CutOffEnd(FormatBytes(file.Length), _sColW)}");
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("N/A");
                        Console.ResetColor();
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
            }
        }

        private string FormatBytes(int bytes) => FormatBytes((long)bytes);

        // https://stackoverflow.com/a/67945659/16426057
        private string FormatBytes(long bytes)
        {
            const long OneKB = 1024;
            const long OneMB = OneKB * OneKB;
            const long OneGB = OneMB * OneKB;
            const long OneTB = OneGB * OneKB;

            return bytes switch
            {
                (< OneKB) => $"{bytes}B",
                (>= OneKB) and (< OneMB) => $"{bytes / OneKB}KB",
                (>= OneMB) and (< OneGB) => $"{bytes / OneMB}MB",
                (>= OneGB) and (< OneTB) => $"{bytes / OneMB}GB",
                (>= OneTB) => $"{bytes / OneTB}"
            };
        }

        private long GetBytesFromDir(DirectoryInfo dir)
        {
            long size = 0;

            foreach (var item in dir.GetFiles())
            {
                size += item.Length;
            }

            return size;
        }

        private string CutOffEnd(string str, int size)
        {
            char[] split = str.ToCharArray();
            if(split.Length >= size)
            {
                string _newstr = "";
                for (int i = 0; i < size - 3; i++)
                {
                    _newstr += split[i];
                }
                _newstr += "...";
                return _newstr;
            } else
            {
                return str;
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
