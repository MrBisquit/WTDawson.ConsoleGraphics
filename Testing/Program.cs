using WTDawson.ConsoleGraphics;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProgressWindow progressWindow = new ProgressWindow(true, "Updating something I guess?", 25, false);
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Warn, "Hello"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Info, "World"));
            progressWindow.Redraw(true);

            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Warn, "Hello World!"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Error, "Hello World!"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Info, "Hello World!"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Success, "Hello World!"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Failure, "Hello World!"));
            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Completed, "Hello World!"));

            for (int i = 0; i < 100; i++)
            {
                if(i >= 50)
                {
                    progressWindow.Title = "Updating...";
                }

                progressWindow.UpdateProgress(i + 1);
                progressWindow.Redraw(false);

                progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.None, $"New progress: {i + 1}%"));
                progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Info, "Waiting 500ms..."));

                Thread.Sleep(500);
            }
        }
    }
}
