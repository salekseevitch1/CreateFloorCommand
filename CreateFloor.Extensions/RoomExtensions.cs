using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;

namespace CreateFloor.Extensions
{
    public static class RoomExtensions
    {
        public static Dictionary<Curve, Element> GetRoomBoundaries(
            this Room room)
        {
            Document document = room.Document;
            return room
                .GetBoundarySegments(new SpatialElementBoundaryOptions())
                .SelectMany(it => it)
                .ToDictionary(keySelector: it => it.GetCurve(), elementSelector: it => document.GetElement(it.ElementId));
        }

        public static List<List<Curve>> GetRoomBoundariesCurves(
            this Room room)
        {
            var curves = new List<List<Curve>>();
            foreach (var closedBoundary in room.GetBoundarySegments(new SpatialElementBoundaryOptions()))
            {
                List<Curve> closedLoop = new List<Curve>();
                foreach (var boudaries in closedBoundary)
                {
                    closedLoop.Add(boudaries.GetCurve());
                }
                curves.Add(closedLoop);
            }
            return curves;
        }

        public static List<List<XYZ>> GetRoomBoundariesPoints(
            this Room room)
        {
            List<List<XYZ>> points = new List<List<XYZ>>();
            var roomCurves = GetRoomBoundariesCurves(room);
            foreach (var contour in roomCurves)
            {
                var temp = new List<XYZ>();
                foreach (var curve in contour)
                {
                    temp.Add(curve.GetEndPoint(0));
                    temp.Add(curve.GetEndPoint(1));
                }

                points.Add(temp);
            }

            return points;
        }

    }
}
