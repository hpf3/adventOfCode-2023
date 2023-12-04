using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOCGUI.Interface;

public abstract class IDaySolution
{
    public abstract int DayNumber { get; }
    public abstract string Part1(string input, bool isTest = false);
    public abstract string Part2(string input, bool isTest = false);
    protected void Log(string message,bool isTest){
        if(!isTest)
            return;
        var desktop = App.Current!.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        if (desktop is not null)
        {
            ((MainWindow)desktop.MainWindow!).Log(message);
        }
    }
}
