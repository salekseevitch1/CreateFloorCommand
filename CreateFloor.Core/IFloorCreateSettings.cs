using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloor.Core
{
    public interface IFloorCreateSettings
    {
        List<Room> Rooms { get; set; }
        ElementId FloorTypeId { get; set; }
        Document Document { get; }
        double OffsetByLevelInMillimeters { get; set; }
    }
}
