using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOCGUI.Data;

public struct Test
{
    public string Input { get; set; }
    public string ExpectedOutput { get; set; }
    public Test(string input, string expectedOutput)
    {
        Input = input;
        ExpectedOutput = expectedOutput;
    }

    public override string ToString()
    {
        return Input;
    }

    public override bool Equals(object? obj)
    {
        return obj is Test test &&
               Input == test.Input &&
               ExpectedOutput == test.ExpectedOutput;
    }

    //operator overloads
    public static bool operator ==(Test left, Test right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(Test left, Test right)
    {
        return !(left == right);
    }
}
