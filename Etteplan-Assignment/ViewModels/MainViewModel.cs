using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Etteplan_Assignment.Services;
using Etteplan_Assignment.Utilities;
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
                title: "Save Translation", 
                suggestedFileName: "translation", 
                defaultExtension: "txt", 
                fileTypeChoices: _fileExtensionTypes);

            if (file is null)
                return;

            var xliffService = App.Current?.Services?.GetService<XLIFFService>();

            if (xliffService is null)
                throw new NullReferenceException($"Missing {nameof(XLIFFService)} instance.");

            var transObject = xliffService.GetTranslationObject(Id);

            if (transObject is null)
                throw new XMLException($"Could not find any translation tag with id: {Id}.");

            var targetObject = transObject.Find("target");

            if (targetObject is null)
                throw new XMLException("'target' tag was not found.");

            using var stream = new MemoryStream(Encoding.Default.GetBytes(targetObject.Content.ToString()));
            await using var writerStream = await file.OpenWriteAsync();
            await stream.CopyToAsync(writerStream);
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }
}
