using Avalonia;
using Avalonia.Platform;
using Etteplan_Assignment.Data;
using Etteplan_Assignment.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Etteplan_Assignment.Services;

public class XLIFFService : IXLIFFService
{
    private string _text;
    private XMLTree _tree;

    public XLIFFService(Uri fileUri)
    {
        using var stream = AssetLoader.Open(fileUri);
        using var readerStream = new StreamReader(stream);
        _text = readerStream.ReadToEnd();
        _tree = XMLUtility.ReadAsXML(_text);
    }

    /// <summary>
    /// Get all translation <see cref="XMLObject"/>s found.
    /// </summary>
    /// <returns>Translation <see cref="XMLObject"/>s.</returns>
    /// <exception cref="XMLException"></exception>
    public IEnumerable<XMLObject> GetTranslationObjects()
    {
        XMLObject? fileObject = null;       // <file source-language="en" target-language="sv-se" >
        foreach (var root in _tree.Roots)   
        {
            fileObject = root.Find("file");

            if (fileObject is not null)
                break;
        }

        if (fileObject is null)
            throw new XMLException("'file' tag was not found.");

        var bodyObject = fileObject.Find("body");    // <body>

        if (bodyObject is null)
            throw new XMLException("'body' tag was not found.");

        foreach (var child in bodyObject.Children)
        {
            if (child.Name.Span.Equals("trans-unit", StringComparison.Ordinal))
                yield return child;
        }
    }

    /// <summary>
    /// Get first translation <see cref="XMLObject"/> based on id.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <returns><see cref="XMLObject"/> if it exist.</returns>
    public XMLObject? GetTranslationObject(string id)
    {
        foreach (var xmlObject in GetTranslationObjects())
        {
            if (xmlObject.Attributes.Any(x => x.Name.Span.Equals("id", StringComparison.Ordinal) && x.Value.Span.Equals(id, StringComparison.Ordinal)))
                return xmlObject;
        }

        return null;
    }
}
