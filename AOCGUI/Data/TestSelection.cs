using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AOCGUI.Data;

public class TestSelection : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private Test _test;
    public Test Test
    {
        get { return _test; }
        set
        {
            if (_test != value)
            {
                _test = value;
                OnPropertyChanged();
            }
        }
    }
    private bool _isPassed;
    public bool IsPassed
    {
        get { return _isPassed; }
        set
        {
            if (_isPassed != value)
            {
                _isPassed = value;
                OnPropertyChanged();
            }
        }
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public TestSelection(Test test)
    {
        Test = test;
        IsPassed = false;
    }

    //implicit conversion
    public static implicit operator TestSelection(Test test)
    {
        return new TestSelection(test);
    }
    public static implicit operator Test(TestSelection testSelection)
    {
        return testSelection.Test;
    }
}
