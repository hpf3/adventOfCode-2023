using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AOCGUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        System.Console.WriteLine("App initialized");
        
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = desktop.Args;
            #if DEBUG
            if (args is not null)
            foreach (var arg in args)
            {
                System.Console.WriteLine($"arg: {arg}");
            }
            #endif
            
            
            if (args.Length > 0)
            {
                if (Uri.TryCreate(args[0], UriKind.RelativeOrAbsolute, out Uri uri))
                {
                    if (uri.IsAbsoluteUri)
                    {
                        ConfigManager.RootPath = uri.AbsolutePath;
                    }
                    else
                    {
                        //relative path from current directory
                        ConfigManager.RootPath = System.IO.Path.Combine(Environment.CurrentDirectory, uri.OriginalString);
                    }
                }
            }

            ConfigManager.Initialize();
            desktop.Exit += (s, e) => ConfigManager.Shutdown();
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
        System.Console.WriteLine("App framework initialization completed");
    }

}