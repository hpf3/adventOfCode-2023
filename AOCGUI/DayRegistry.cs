using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AOCGUI.Interface;
namespace AOCGUI;

public static class DayRegistry
{
    public static IDaySolution[] _daySolutions = new IDaySolution[]{
        new Days.Day1(),
        new Days.Day2(),
    };
}
