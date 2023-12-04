using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace AOCGUI.Data;

public class ConsoleOutput: INotifyPropertyChanged
{
    private int _repeatCount=1;
    public int RepeatCount
    {
        get { return _repeatCount; }
        set
        {
            if (_repeatCount != value)
            {
                _repeatCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRepeated));
            }
        }
    }
    public bool IsRepeated { get {return _repeatCount > 1;}}

    private string _message="";
    public string Message
    {
        get { return _message; }
        set
        {
            if (_message != value)
            {
                _message = value;
                OnPropertyChanged();
            }
        }
    }
    public ConsoleOutput(string message, int repeatCount = 1)
    {
        _message = message;
        _repeatCount = repeatCount;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    override public string ToString()
    {
        if (RepeatCount == 1)
            return Message;
        return $"({RepeatCount}) {Message}";
    }
}
