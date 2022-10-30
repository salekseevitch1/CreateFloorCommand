using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloor.Extensions
{
    public static class FloorExtensions
    {
        public static void SetHeightAboutLevel(
            this Floor floor, double heightAboutLevelInMillimeters)
        {
            double value = UnitUtils.ConvertToInternalUnits(heightAboutLevelInMillimeters, UnitTypeId.Millimeters);
            floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(value);
        }
    }
}
