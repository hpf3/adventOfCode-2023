using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AOCGUI.Interface;
using Avalonia.Controls.Primitives;
using AOCGUI.Data;
namespace AOCGUI.Windows;

public partial class AddTestPopup : Window
{
    private Test? Result;
    private Action<Test?> SubmitAction;
    public string TestInput { get=>txtTestInput.Text??""; set=>txtTestInput.Text=value; }
    public string TestOutput { get=>txtTestOutput.Text??""; set=>txtTestOutput.Text=value; }
    
    public AddTestPopup(Action<Test?> submitAction)
    {
        InitializeComponent();
        SubmitAction = submitAction;
    }
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        SubmitAction.Invoke(null);
        Close();
    }
    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        SubmitAction.Invoke(GetResult());
        Close();
    }

    public Test GetResult()
    {
        return new Test(TestInput,TestOutput);
    }
    
}