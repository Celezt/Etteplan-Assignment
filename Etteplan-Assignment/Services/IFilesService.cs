using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etteplan_Assignment.Services;

interface IFilesService
{
    public Task<IStorageFile?> SaveFileAsync(
                string? title = null, string? suggestedFileName = null, string? defaultExtension = null,
                IStorageFolder? suggestedStartLocation = null, IReadOnlyList<FilePickerFileType>? fileTypeChoices = null);
}
