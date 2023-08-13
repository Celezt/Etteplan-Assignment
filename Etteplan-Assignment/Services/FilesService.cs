using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Etteplan_Assignment.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etteplan_Assignment.Services;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public Task<IStorageFile?> SaveFileAsync(
                string? title = null, string? suggestedFileName = null, string? defaultExtension = null, 
                IStorageFolder? suggestedStartLocation = null, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null)
        => _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = title,
            SuggestedFileName = suggestedFileName,
            SuggestedStartLocation = suggestedStartLocation,
            DefaultExtension = defaultExtension,
            FileTypeChoices = fileTypeChoices,
            ShowOverwritePrompt = true,
        });
}
