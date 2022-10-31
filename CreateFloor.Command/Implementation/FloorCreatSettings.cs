using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CreateFloor.Core;
using System.Collections.Generic;
using System.Linq;

namespace CreateFloor.Command
{
    public class FloorCreateSettings : IFloorCreateSettings
    {
        public List<Room> Rooms { get; set; }
        public ElementId FloorTypeId { get; set; }
        public double BaseOffset { get; set; }
        public Document Document
        {
            get
            {
                return Rooms.First().Document;
            }
        }

        public FloorCreateSettings()
        {

        }
    }
}
