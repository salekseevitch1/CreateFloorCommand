using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloorMVVM.Utils
{
    internal class MyGeometryUtils
    {
        public static List<List<XYZ>> OnlyUniquePoints(List<List<XYZ>> points, int tolerance=5)
        {
            var result = new List<List<XYZ>>();

            foreach (var item in points)
            {
                result.Add(
                    item.GroupBy(it => new { x = Math.Round(it.X, tolerance), y = Math.Round(it.Y, tolerance), z = Math.Round(it.Z, tolerance) })
                    .Select(it => it.First())
                    .ToList()
                );
            }

            return result;
        }

        public static List<List<XYZ>> RemovePointsOnSameLine(List<List<XYZ>> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int y = 0; y < points[i].Count - 3; y++)
                {
                    var p1 = points[i][y];
                    var p2 = points[i][y + 1];
                    var p3 = points[i][y + 2];

                    var triangleArea = 0.5 * Math.Abs(p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y));

                    if (triangleArea < 0.01)
                    {
                        points[i].RemoveAt(y + 1);
                    }
                }

            }
            return points;

        }

        public static List<CurveLoop> CreateCurveLoopByPoints(List<List<XYZ>> points)
        {
            List<CurveLoop> curveLoops = new List<CurveLoop>();
            foreach (var item in points)
            {
                CurveLoop curveLoop = new CurveLoop();
                for (int i = 0; i < item.Count; i++)
                {
                    var startPoint = item[i];
                    var endPoint = i + 1 != item.Count ? item[i + 1] : item.First();

                    var line = Line.CreateBound(startPoint, endPoint);
                    curveLoop.Append(line);
                }
                curveLoops.Add(curveLoop);
            }

            return curveLoops;

        }

        public static Solid UnionSolids(List<Solid> solids)
        {
            Solid unionSolid = solids[0];

            for (int i = 0; i < solids.Count - 1; i++)
            {
                var firstSolid = solids[i];
                var secondSolid = solids[i + 1];

                unionSolid = BooleanOperationsUtils.ExecuteBooleanOperation(firstSolid, secondSolid, BooleanOperationsType.Union);
                solids[i + 1] = unionSolid;
            }

            return unionSolid;
        }

        public static List<CurveLoop> GetBoundaryLargestFaceOfSolid(Solid solid)
        {
            var faces = solid.Faces.Cast<PlanarFace>().ToList();
            var maxBottomFace = faces.OrderBy(it => it.Area).Last();
            return maxBottomFace.GetEdgesAsCurveLoops().ToList();
        }

        public static void CreateDirectShape(Document document, Solid solid)
        {
            DirectShape directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
            List<GeometryObject> geometryObjects = new List<GeometryObject>() { solid };

            directShape.SetShape(geometryObjects);
        }

        public static void CreateDirectShape(Document document, CurveLoop curves)
        {
            if (curves == null) return;

            DirectShape directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
            List<GeometryObject> geometryObjects = new List<GeometryObject>();
            foreach (var curve in curves)
            {
                geometryObjects.Add(curve);
            }

            directShape.SetShape(geometryObjects);
        }

        public static Solid CreateSolidByCurveLoop(
            List<CurveLoop> curveLoop, double heigth = 1)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(curveLoop, XYZ.BasisZ, heigth);
        }
    }
}
