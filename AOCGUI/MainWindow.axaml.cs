using System.Collections.Generic;
using Avalonia.Controls;
using AOCGUI.Data;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using System.Linq;
using Avalonia.Platform.Storage;

namespace AOCGUI;

public partial class MainWindow : Window
{
    //binding properties
    public ObservableCollection<DaySelection> Days { get; set; } = new();
    public ObservableCollection<ConsoleOutput> OutputLines { get; set; } = new();
    public ObservableCollection<TestSelection> Tests { get; set; } = new();
    public MainWindow()
    {
        //load days
        for (int i = 1; i <= DayRegistry._daySolutions.Length; i++)
        {
            Days.Add(new DaySelection(i));
        }
        InitializeComponent();
        DaySelector.ItemsSource = Days;
        OutputLog.ItemsSource = OutputLines;
        TestList.ItemsSource = Tests;

        DaySelector.SelectionChanged += DayChanged;
        DaySelector.SelectedIndex = 0;
        SolutionSelector.SelectedIndex = 0;
        //prevent solution selector from being empty
        SolutionSelector.SelectionChanged += (sender, e) =>
        {
            if (SolutionSelector.SelectedIndex == -1)
            {
                SolutionSelector.SelectedIndex = 0;
            }
        };

        //save on close
        this.Closing += (sender, e) =>
        {
            if (DaySelector.SelectedItem is not DaySelection day)
            {
                return;
            }
            SaveDay(day);
        };
    }

    private void DayChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
        {
            return;
        }
        if (e.RemovedItems.Count > 0)
        {
            SaveDay(e.RemovedItems.OfType<DaySelection>().FirstOrDefault());
        }
        LoadDay(e.AddedItems.OfType<DaySelection>().FirstOrDefault());
    }
    private void SaveDay(DaySelection day)
    {
        if (day.InputFilePath is not null)
        {
            ConfigManager.SetConfig($"Day{day.DayNumber}_SolutionFile", day.InputFilePath);
        }
        //store tests
        day.SetTests(Tests.ToArray());
    }
    private void LoadDay(DaySelection day)
    {
        //load input file
        string inputFilePath = ConfigManager.GetConfig($"Day{day.DayNumber}_SolutionFile");
        if (inputFilePath != "")
        {
            day.InputFilePath = inputFilePath;
        }
        //load tests
        Tests.Clear();
        foreach (var test in day.GetTests())
        {
            Tests.Add(test);
        }
    }


    private void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DaySelector.SelectedIndex == -1)
        {
            Log("No day selected");
            return;
        }
        var day = Days[DaySelector.SelectedIndex];

        // Start async operation to open the dialog.
        var files = this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });
        // Wait for the user to close the dialog.
        files.ContinueWith(task =>
        {
            // Get the selected files.
            var result = files.Result;
            if (result.Count >= 1)
            {
                day.InputFilePath = result[0].Path.LocalPath.ToString();
                Log($"Loaded file {result[0].Path.LocalPath.ToString()}");
            }
        });
    }
    private void Test_Click(object sender, RoutedEventArgs e)
    {
        if (DaySelector.SelectedItem is not DaySelection day)
        {
            Log("No day selected");
            return;
        }
        if (TestList.Items.Count == 0)
        {
            Log("No tests added");
            return;
        }
        TestList.SelectAll();
        RunTest_Click(sender, e);
    }
    private void Run_Click(object sender, RoutedEventArgs e)
    {
        if (DaySelector.SelectedItem is not DaySelection day)
        {
            Log("No day selected");
            return;
        }
        if (day.InputFilePath is null)
        {
            Log("No input file loaded");
            return;
        }
        Log($"Running solution {SolutionSelector.SelectedIndex+1} for {day}");
        string input = System.IO.File.ReadAllText(day.InputFilePath);
        string result = RunSolution(input);
        Log(result);
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        OutputLines.Clear();
    }

    public void Log(string message)
    {
        if (OutputLog.Items.Count > 0 && OutputLog.Items[^1] is ConsoleOutput lastLine && lastLine.Message == message)
        {
            lastLine.RepeatCount++;
            return;
        }
        OutputLines.Add(new ConsoleOutput(message));
#if DEBUG
        System.Console.WriteLine(message);
#endif
    }
    private string RunSolution(string input, bool isTest = false){
        if (DaySelector.SelectedItem is not DaySelection day)
        {
            Log("No day selected");
            return "";
        }
        switch (SolutionSelector.SelectedIndex)
        {
            case 0:
                return day.DaySolution.Part1(input, isTest);
            case 1:
                return day.DaySolution.Part2(input, isTest);
            default:
                return "";
        }
    }

    //TestList
    private void RunTest_Click(object sender, RoutedEventArgs e)
    {
        if (DaySelector.SelectedItem is not DaySelection day)
        {
            Log("No day selected");
            return;
        }
        if (TestList.SelectedItems.Count == 0)
        {
            Log("No tests selected");
            return;
        }
        int Count = TestList.SelectedItems.Count;
        int current = 0;
        int passed = 0;
        foreach (var item in TestList.SelectedItems)
        {
            if (item is TestSelection test)
            {
                Log($"Running test {++current}/{Count}");
                string result = RunSolution(test.Test.Input, true);
                if (result == test.Test.ExpectedOutput)
                {
                    test.IsPassed = true;
                    passed++;
                }
                else
                {
                    test.IsPassed = false;
                    Log($"Test failed: {result} != {test.Test.ExpectedOutput}");
                }
            }
        }
        Log($"Tests passed: {passed}/{Count}");
    }
    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (TestList.SelectedItems.Count == 0)
        {
            Log("No tests selected");
            return;
        }
        foreach (var item in TestList.SelectedItems)
        {
            if (item is TestSelection test)
            {
                Tests.Remove(test);
            }
        }
    }
    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (DaySelector.SelectedItem is not DaySelection day)
        {
            Log("No day selected");
            return;
        }
        int currentDay = day.DayNumber;
        Test? result = null;
        var popup = new Windows.AddTestPopup((test) => result = test);
        var dialogTask = popup.ShowDialog(this);
        dialogTask.ContinueWith((task) =>
        {
            if (result.HasValue)
            {
                Tests.Add(new TestSelection(result.Value));
            }
        });

    }
}