using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AOCGUI.Interface;

namespace AOCGUI.Days;

public class Day1: IDaySolution
{
    override public int DayNumber => 1;
    override  public string Part1(string input, bool isTest = false)
    {
        var lines = input.Split("\n");
        var numberStrings = lines.Select((line)=>{
            StringBuilder sb = new();
            foreach (char c in line)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        });
        int sum = 0;
        foreach (var numberString in numberStrings)
        {
            if (numberString.Length == 0)
            {
                continue;
            }
            Log($"  number: {numberString}",isTest);
                string pair = numberString.First().ToString() + numberString.Last().ToString();
                Log($"    pair: {pair}",isTest);
                sum += int.Parse(pair);
            Log($"  sum: {sum}",isTest);
        }
        return sum.ToString();
    }

    override public string Part2(string input, bool isTest = false)
    {
        var lines = input.Split("\n");
        var numberStrings = lines.Select((line)=>{
            StringBuilder sb = new();
            //step1: replace named digits with numbers
            for(int i = 0; i < line.Length; i++)
            {
                string sub = line.Substring(i,line.Length-i);
                if (sub.StartsWith("zero"))
                {
                    sb.Append("0");
                }
                else if (sub.StartsWith("one"))
                {
                    sb.Append("1");
                }
                else if (sub.StartsWith("two"))
                {
                    sb.Append("2");
                }
                else if (sub.StartsWith("three"))
                {
                    sb.Append("3");
                }
                else if (sub.StartsWith("four"))
                {
                    sb.Append("4");
                }
                else if (sub.StartsWith("five"))
                {
                    sb.Append("5");
                }
                else if (sub.StartsWith("six"))
                {
                    sb.Append("6");
                }
                else if (sub.StartsWith("seven"))
                {
                    sb.Append("7");
                }
                else if (sub.StartsWith("eight"))
                {
                    sb.Append("8");
                }
                else if (sub.StartsWith("nine"))
                {
                    sb.Append("9");
                }
                else if (char.IsDigit(line[i]))
                {
                    sb.Append(line[i]);
                }
            }
            return sb.ToString();
        });
        int sum = 0;
        foreach (var numberString in numberStrings)
        {
            if (numberString.Length == 0)
            {
                continue;
            }
            Log($"  number: {numberString}",isTest);
                string pair = numberString.First().ToString() + numberString.Last().ToString();
                Log($"    pair: {pair}",isTest);
                sum += int.Parse(pair);
            //Log($"  sum: {sum}",isTest);
        }
        return sum.ToString();
    }
}
