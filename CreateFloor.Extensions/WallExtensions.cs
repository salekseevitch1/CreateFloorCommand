using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloor.Extensions
{
    public static class WallExtensions
    {
        public static List<FamilyInstance> GetDoorsInWall(
            this Wall wall)
        {
            var doors = new List<FamilyInstance>();
            var dependentDoors = wall.GetDependentElements(new ElementCategoryFilter(BuiltInCategory.OST_Doors));
            if (dependentDoors.Count != 0)
            {
                foreach (var dependentDoorId in dependentDoors)
                {
                    doors.Add(wall.Document.GetElement(dependentDoorId) as FamilyInstance);
                }
            }
            return doors;
        }

        public static XYZ GetDirection(
            this Wall wall)
        {
            var wallLocationCurve = wall.Location as LocationCurve;
            var wallCurve = wallLocationCurve.Curve as Line;
            return wallCurve.Direction;
        }
    }
}
