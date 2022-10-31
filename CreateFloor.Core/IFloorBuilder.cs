using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloor.Core
{
    public interface IFloorBuilder
    {
        List<Floor> CreateFloor(IFloorCreateSettings floorCreateSettings); 
    }
}
