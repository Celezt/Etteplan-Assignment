using Etteplan_Assignment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etteplan_Assignment.Services;

public interface IXLIFFService
{
    public IEnumerable<XMLObject> GetTranslationObjects();
    public XMLObject? GetTranslationObject(string id);
}
