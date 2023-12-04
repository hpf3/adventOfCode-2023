using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AOCGUI.Interface;

namespace AOCGUI.Days;

public class Day2 : IDaySolution
{
    //line format:
    // Game 6: 2 red, 3 green; 1 blue, 15 red, 2 green; 1 green, 7 red


    override public int DayNumber => 2;
    override public string Part1(string input, bool isTest = false)
    {
        var bagTotals = new Dictionary<string, int>(){
                {"red", 12},
                {"green", 13},
                {"blue", 14},
            };
        var lines = input.Split("\n");
        int total = 0;
        foreach (var line in lines)
        {
            var parts = line.Split(":");
            var Pulls = parts[1].Split(";");
            bool isPossible = true;
            Log($"{parts[0]}", isTest);
            foreach (var pull in Pulls)
            {
                Log($"  pull: {pull}", isTest);
                var pullGroups = pull.Split(",");
                foreach (var pullGroup in pullGroups)
                {
                    var pullGroupParts = pullGroup.Trim().Split(" ");
                    var pullGroupColor = pullGroupParts[1];
                    var pullGroupCount = int.Parse(pullGroupParts[0]);
                    if (bagTotals[pullGroupColor] < pullGroupCount)
                    {
                        isPossible = false;
                        break;
                    }
                }
                if (!isPossible)
                {
                    Log($"    not possible", isTest);
                    break;
                }
                Log($"possible", isTest);
            }

            if (isPossible)
            {
                total += int.Parse(parts[0].Trim().Split(" ")[1]);
            }
        }
        return total.ToString();
    }

    override public string Part2(string input, bool isTest = false)
    {
        var lines = input.Split("\n");
        int total = 0;
        foreach (var line in lines)
        {
            var parts = line.Split(":");
            var Pulls = parts[1].Split(";");
            Log(line, isTest);
            int minRed = 0;
            int minGreen = 0;
            int minBlue = 0;
            foreach (var pull in Pulls)
            {
                var pullGroups = pull.Split(",");
                foreach (var pullGroup in pullGroups)
                {
                    var pullGroupParts = pullGroup.Trim().Split(" ");
                    var pullGroupColor = pullGroupParts[1];
                    var pullGroupCount = int.Parse(pullGroupParts[0]);
                    if (pullGroupColor == "red")
                    {
                        minRed = Math.Max(minRed, pullGroupCount);
                    }
                    else if (pullGroupColor == "green")
                    {
                        minGreen = Math.Max(minGreen, pullGroupCount);
                    }
                    else if (pullGroupColor == "blue")
                    {
                        minBlue = Math.Max(minBlue, pullGroupCount);
                    }
                }
            }
            int power = minRed * minGreen * minBlue;
            Log($"  power: {power} | Colors: {minRed} red, {minGreen} green, {minBlue} blue", isTest);
            total += power;
        }
        return total.ToString();
    }
}
