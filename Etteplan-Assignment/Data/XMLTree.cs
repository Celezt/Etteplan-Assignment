using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etteplan_Assignment.Data;

public record struct XMLTree(IReadOnlyList<XMLObject> Roots);
