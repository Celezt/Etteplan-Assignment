using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Etteplan_Assignment.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Etteplan_Assignment.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly FilePickerFileType[] _fileExtensionTypes = new FilePickerFileType[]
    {
        new("Text") { Patterns = new[] {"*.txt"} },
    };

    [ObservableProperty]
    private string _id = "42007";

    [RelayCommand]
    private async Task SaveFile()
    {
        ErrorMessages?.Clear();
        try
        {
            var fileService = App.Current?.Services?.GetService<IFilesService>();

            if (fileService is null)
                throw new NullReferenceException($"Missing {nameof(IFilesService)} instance.");

            var file = await fileService.SaveFileAsync(
                title: "Save Translation", suggestedFileName: "translation", defaultExtension: "txt", fileTypeChoices: _fileExtensionTypes);

            if (file is null)
                return;

            var stream = new MemoryStream(Encoding.Default.GetBytes(Id));
            await using var writeStream = await file.OpenWriteAsync();
            await stream.CopyToAsync(writeStream);
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }
}
