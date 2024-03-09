using WTDawson.ConsoleGraphics;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SelectionMenu selectionMenu = new SelectionMenu("Select feature to test", new List<SelectionMenu.SelectionElement>());
            selectionMenu.Custom.SelectedDisplayType = SelectionMenu.Customisation.DisplayType.Linux;
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("ProgressWindow"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("SelectionMenu"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("FileSystemMenu"));
            SelectionMenu.SelectionResult result = selectionMenu.RedrawWithResult();

            switch(result.SelectedIndex)
            {
                case 0:
                    DrawProgressWindow();
                    break;
                case 1:
                    DrawSelectionMenu();
                    break;
                case 2:
                    DrawFileSystemMenu();
                    break;

                case -1:
                    Console.WriteLine("You didn't select anything...");
                    Console.ReadLine();
                    break;
            }

            Console.Clear();

            Main(args);
        }

        static void DrawProgressWindow()
        {
            ProgressWindow progressWindow = new ProgressWindow(true, "Updating something I guess?", 25, false, true);
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
                if (i >= 50)
                {
                    progressWindow.Title = "Updating...";
                }

                progressWindow.UpdateProgress(i + 1);
                progressWindow.Redraw(false);

                progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.None, $"New progress: {i + 1}%"));
                progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Info, "Waiting 500ms..."));

                Thread.Sleep(50);
            }

            progressWindow.Lines.Add(new ProgressWindow.ConsoleLine(ProgressWindow.ConsoleLine.LineType.Completed, "Done"));
            progressWindow.UpdateProgress(100);
            progressWindow.Redraw(true);
        }

        static void DrawSelectionMenu()
        {
            SelectionMenu selectionMenu = new SelectionMenu("Selecting something I guess?", new List<SelectionMenu.SelectionElement>(), true);
            selectionMenu.Custom.SelectedDisplayType = SelectionMenu.Customisation.DisplayType.Circular;
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("Selection item 1"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("Selection item 2"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("Selection item 3"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("Selection item 4"));
            selectionMenu.selectionElements.Add(new SelectionMenu.SelectionElement("Selection item 5"));
            selectionMenu.Redraw(true);
        }

        static void DrawFileSystemMenu()
        {
            FileSystemMenu fileSystemMenu = new FileSystemMenu("Select a file I guess?", false, null);
            fileSystemMenu.RequestFileSelection();
        }
    }
}
