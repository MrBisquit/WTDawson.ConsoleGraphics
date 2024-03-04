namespace WTDawson.ConsoleGraphics
{
    /// <summary>
    /// A full-screen (Full console window) progress window
    /// </summary>
    public class ProgressWindow
    {
        // Properties
        bool _UseThreading = true; // Default: true
        List<ConsoleLine> _Lines = new List<ConsoleLine>();
        string? _Title;

        int _Progress = 0;
        bool _IsIntermidiate = false; // Requires threading so that it can do the fancy left-right animation.

        // Public properties
        public bool UseThreading { get { return _UseThreading; } }
        public List<ConsoleLine> Lines { get { return _Lines; } set { _Lines = value; } }
        public string Title { get { return _Title; } set { _Title = value; } }
        public int Progress { get { return _Progress; } set { UpdateProgress(value); } }
        public bool IsIntermidiate { get { return _IsIntermidiate; } set { if (!UseThreading) { throw new Exception("Cannot use intermidiate while in single-threaded mode."); } else _IsIntermidiate = value; } }
        public bool UseLineTypes { get; set; } = true;
        public char ProgressChar { get; set; } = '█'; // Could be #, █ or any others.
        public char EmptyProgressChar { get; set; } = '▒'; // Could be ' ' or any others.

        private int LastBufferWidth = 0;
        private int LastBufferHeight = 0;

        public ProgressWindow(bool useThreading, string? title, int progress, bool isIntermidiate)
        {
            _UseThreading = useThreading;
            _Title = title;
            _Progress = progress;
            _IsIntermidiate = isIntermidiate;
        }

        public class ConsoleLine
        {
            public enum LineType
            {
                None = 0,
                Warn = 1, // Warning
                Error = 2,
                Info = 3,
                Success = 4,
                Failure = 5,
                Completed = 6
                
                // Top length: 9
            }

            public LineType Type = LineType.None;
            public string Message = "";

            public ConsoleLine(LineType type, string message)
            {
                Type = type;
                Message = message;
            }
        }

        public void UpdateProgress(int progress)
        {
            _Progress = progress;
        }

        public void Redraw(bool Clear = false)
        {
            if(Clear || (LastBufferWidth != Console.BufferWidth || LastBufferHeight != Console.BufferHeight)) Console.Clear(); // Completely clear the screen first (Not very good for quickly updating progress bars)

            Console.Title = $"{Title} - {Progress}%";

            int lines = Console.BufferHeight;
            int chars = Console.BufferWidth;

            int drawLineStartIndex = 0;

            Console.SetCursorPosition(0, 0); // Go to the start

            if(Title != null)
            {
                drawLineStartIndex++;

                int blank = chars - Title.Length;

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write($"\r {Title}{RepeatChar(' ', blank - 1)}");

                Console.ResetColor();
            }

            // Drawing the progress bar (Draw the text on last)

            // Obviously we want to leave around about 4 spaces + 1 padding at the end
            // (Maths helped by @xrc2alt on Discord)
            int _pbwidth = chars - 5;
            int _filled = (int)Math.Ceiling((double)(Progress * (_pbwidth - 2) / 100));
            int _unfilled = (int)Math.Ceiling((double)((100 - Progress) * (_pbwidth - 2) / 100));

            Console.SetCursorPosition(0, lines - 1);
            Console.Write('[');
            Console.Write($"{RepeatChar(ProgressChar, _filled)}{RepeatChar(EmptyProgressChar, _unfilled)}");
            Console.Write($"] {Progress}{(Progress == 100 ? "" : " ")}%");

            // Now writing the text in the middle
            int _totalLines = lines - (Title != null ? 1 : 0) - 1; // Work out how many lines we can draw based on the already worked out maths

            // Grab the lines (Last ones on the list) and display them
            List<ConsoleLine> _drawLines = new List<ConsoleLine>();

            if(_totalLines >= Lines.Count) _drawLines = Lines;
            else
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    if(i >= _totalLines) break;

                    _drawLines.Add(Lines[^(i + 1)]);
                }

                _drawLines.Reverse();
            }

            // Drawing them

            Console.SetCursorPosition(0, drawLineStartIndex);

            foreach (ConsoleLine line in _drawLines)
            {
                if(UseLineTypes)
                {
                    Console.Write(' ');
                    if(line.Type == ConsoleLine.LineType.None)
                    {
                        Console.Write(RepeatChar(' ', 10));
                    } else if(line.Type == ConsoleLine.LineType.Warn)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Warn" + RepeatChar(' ', 6));
                    } else if(line.Type == ConsoleLine.LineType.Error)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("Error" + RepeatChar(' ', 5));
                    } else if(line.Type == ConsoleLine.LineType.Info)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write("Info" + RepeatChar(' ', 6));
                    } else if(line.Type == ConsoleLine.LineType.Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Success" + RepeatChar(' ', 3));
                    } else if(line.Type == ConsoleLine.LineType.Failure)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Failure" + RepeatChar(' ', 2));
                    } else if(line.Type == ConsoleLine.LineType.Completed)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("Completed" + " ");
                    }

                    Console.ResetColor();

                    Console.Write(line.Message + "\n");
                } else
                {
                    Console.Write(line.Message + "\n");
                }
            }
        }

        private string RepeatChar(char c, int count) { string f = ""; for (int i = 0; i < count; i++) f += c; return f; }
    }
}
