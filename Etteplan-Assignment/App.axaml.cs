using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Etteplan_Assignment.Services;
using Etteplan_Assignment.ViewModels;
using Etteplan_Assignment.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Etteplan_Assignment;

public partial class App : Application
{
    public new static App? Current => Application.Current as App;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            var services = new ServiceCollection();
            services.AddSingleton<IFilesService>(x => new FilesService(desktop.MainWindow));
            Services = services.BuildServiceProvider();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            
            throw new NotImplementedException();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
