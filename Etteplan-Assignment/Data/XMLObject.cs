using System;
using System.Collections.Generic;

namespace Etteplan_Assignment.Data;

/// <summary>
/// Container for an XML tag.
/// </summary>
public class XMLObject
{
    public ReadOnlyMemory<char> Name => _name;
    public ReadOnlyMemory<char> Content => _content;
    public IReadOnlyList<(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Value)> Attributes => _attributes;
    public IReadOnlyList<XMLObject> Children => _children;

    private ReadOnlyMemory<char> _name;
    private ReadOnlyMemory<char> _content;
    private List<(ReadOnlyMemory<char>, ReadOnlyMemory<char>)> _attributes;
    private List<XMLObject> _children;

    public XMLObject(ReadOnlyMemory<char> name, List<(ReadOnlyMemory<char>, ReadOnlyMemory<char>)>? attributes = null)
    {
        _name = name;
        _attributes = attributes ?? new();
    }

    public void Close(ReadOnlyMemory<char>? content = null, List<XMLObject>? children = null)
    {
        _content = content ?? ReadOnlyMemory<char>.Empty;
        _children = children ?? new();
    }
}