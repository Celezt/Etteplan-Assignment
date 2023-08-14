using Etteplan_Assignment.Data;
using System;
using System.Collections.Generic;

namespace Etteplan_Assignment.Utilities;

public static class XMLUtility
{
    private static Stack<(XMLObject XmlObject, List<XMLObject> Children, int Index)> _tags = new();

    /// <summary>
    /// Converts text to a <see cref="XMLTree"/>.
    /// </summary>
    /// <param name="text">To convert.</param>
    /// <exception cref="XMLException"></exception>
    public static XMLTree ReadAsXML(string text)
    {
        var roots = new List<XMLObject>();
        int beginIndex = 0;
        int endIndex = 0;
        int leftIndex = 0;
        int rightIndex = 0;

        _tags.Clear();

        ReadOnlyMemory<char> GetTagName()
        {
            int startIndex = leftIndex;

            for (; leftIndex < endIndex; leftIndex++) // <?=
            {
                if (text[leftIndex] is ' ')    // Ends if it finds a whitespace.
                    break;

                if (!char.IsLetter(text[leftIndex]) && text[leftIndex] is not '-')    // Invalid: name must be a letter. <tag-name> ! <%3+>
                    throw new XMLException($"Name cannot contain any numbers or symbols: '{text[leftIndex]}'");
            }

            return text.AsMemory(startIndex, leftIndex - startIndex);
        }

        bool TryGetAttribute(out (ReadOnlyMemory<char> AttributeName, ReadOnlyMemory<char> AttributeValue) attribute)
        {
            attribute = default;

            for (; leftIndex <= rightIndex; leftIndex++)     // Skip whitespace between attributes.
                if (text[leftIndex] is not ' ')
                    break;

            if (leftIndex == endIndex)
                return false;

            if (text[rightIndex] is '?' or '/')
                return false;

            //
            //  Extract Name
            //
            int startIndex = leftIndex;
            for (; leftIndex < endIndex; leftIndex++) // <?=
            {
                if (text[leftIndex] is '=')    // Ends if it finds a =.
                    break;

                if (text[leftIndex] is ' ')
                    throw new XMLException("Attribute names are not allowed to end with whitespace.");

                if (!char.IsLetter(text[leftIndex]) && text[leftIndex] is not '-')    // Invalid: name must be a letter. <tag-name> ! <%3+>
                    throw new XMLException($"Name cannot contain any numbers or symbols: '{text[leftIndex]}'");
            }

            var attributeName = text.AsMemory(startIndex, leftIndex - startIndex);

            leftIndex++;

            // 
            //  Extract Attribute
            //
            char decoration = '\0';
            startIndex = leftIndex;
            if (text[leftIndex] is '"' or '\'') // Ignore decoration.
            {
                decoration = text[leftIndex++];

                for (; leftIndex < endIndex; leftIndex++)
                {
                    // Argument ending.
                    if (text[leftIndex] == decoration)
                        break;

                    if (leftIndex >= endIndex - 1)    // Invalid: must have a closure. <tag="?"> ! <tag="?>
                        throw new XMLException("Attribute using \" or ' must close with the same character.");
                }
            }
            else
                throw new XMLException($"Attribute: {attributeName} is empty.");

            var attributeValue = text.AsMemory(startIndex + 1, leftIndex - startIndex - 1);
            attribute = (attributeName, attributeValue);

            leftIndex++;

            return true;
        }

        for (leftIndex = 0; leftIndex < text.Length; leftIndex++)
        {
            if (text[leftIndex] is '<')
            {
                beginIndex = leftIndex;
                endIndex = rightIndex = leftIndex + 1;

                if (leftIndex + 1 >= text.Length)
                    throw new XMLException("Tag must end with '>'");

                if (text[leftIndex + 1] is '>')     // Invalid: must contain a name. <tag> ! <>
                    throw new XMLException("Tag must contain an identifier.");

                for (; rightIndex < text.Length; rightIndex++)
                {
                    endIndex = rightIndex;

                    if (text[rightIndex] is '<')    // Invalid: must end with >. <tag> ! <tag<
                        throw new XMLException("Tag must end with '>'");

                    if (text[rightIndex] is '>')    // Found the ending of the tag.
                        break;

                    if (rightIndex + 1 >= text.Length)
                        throw new XMLException("Tag must end with '>'");
                }

                leftIndex++;    // After <.
                rightIndex--;   // Before >.

                if (text[leftIndex] is '/') // Tag is a closed tag. </tag>
                {
                    (XMLObject XmlObject, List<XMLObject> Children, int Index) = _tags.Pop();
                    XmlObject.Close(Children.Count > 0 ? null : text.AsMemory(Index, beginIndex - Index), Children);
                }
                else
                {
                    var arguments = new List<(ReadOnlyMemory<char>, ReadOnlyMemory<char>)>();

                    if (text[leftIndex] is '/' or '?')  // After / or ?.
                        leftIndex++;

                    var tagName = GetTagName();

                    while (leftIndex <= rightIndex)
                    {
                        if (!TryGetAttribute(out var attribute))
                            break;

                        arguments.Add(attribute);
                    }

                    var xmlObject = new XMLObject(tagName, arguments);

                    if (_tags.TryPeek(out var parent))
                        parent.Children.Add(xmlObject); // Add itself to parent.
                    else
                        roots.Add(xmlObject);  // Is root XMLObject if no parent exist.

                    if (text[rightIndex] is '/' or '?') // Tag is an independent tag. <tag/> || <?tag?>
                        xmlObject.Close();
                    else                        // Tag is an open tag. <tag>
                        _tags.Push((xmlObject, new List<XMLObject>(), endIndex + 1));
                }

                leftIndex = endIndex;
            }
        }

        return new XMLTree(roots);
    }

    /// <summary>
    /// Finds existing tag in the tree by name. 
    /// </summary>
    /// <param name="toFindName">Tag name to find.</param>
    /// <returns>First instance of that tag.</returns>
    public static XMLObject? Find(this XMLObject root, string toFindName)
    {
        foreach (var child in root.Children)
        {
            if (child.Name.Span.Equals(toFindName, StringComparison.Ordinal))
                return child;

            var foundXMLObject = child.Find(toFindName);    // Recursive search.
            if (foundXMLObject != null)
                return foundXMLObject;
        }

        return null;
    }
}

public class XMLException : Exception
{
    public XMLException(string message) : base(message) { }
}