using System.Threading.Tasks;
using AOCGUI.Interface;
using AOCGUI.Days;
using AOCGUI.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace AOCGUI.Data;

public class DaySelection
{
    public int DayNumber { get; set; }
    public ConsoleOutput[] ConsoleOutputs { get; set; }
    private TestSelection[]? _tests;
    public string? InputFilePath { get; set; }
    public IDaySolution DaySolution { get; set; }
    public DaySelection(int dayNumber)
    {
        DayNumber = dayNumber;
        ConsoleOutputs = new ConsoleOutput[0];
        DaySolution = DayRegistry._daySolutions[dayNumber - 1];
    }
    public TestSelection[] GetTests(){
        if (_tests is null)
        {
            _tests = ConfigManager.GetDayTests(DayNumber).Select((T,TestSelection)=>new TestSelection(T)).ToArray();
        }
        return _tests;
    }
    public void SetTests(TestSelection[] tests){
        if (!tests.Equals(_tests))
        {
            ConfigManager.SetDayTests(DayNumber, tests.Select((T,Test)=>T.Test).ToArray());
        }
        
        _tests = tests;
    }
    override public string ToString()
    {
        return $"Day {DayNumber}";
    }
}
