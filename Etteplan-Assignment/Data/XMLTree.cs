using System.Collections.Generic;

namespace Etteplan_Assignment.Data;

/// <summary>
/// Top level container for an XML file. Can contain multiple roots without any parent.
/// </summary>
/// <param name="Roots"></param>
public record struct XMLTree(IReadOnlyList<XMLObject> Roots);
