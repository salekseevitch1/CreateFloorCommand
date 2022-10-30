using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitExtensions
{
    public static class SolidExtensions
    {
        public static Solid UnionSolids(
            this List<Solid> solids)
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

        public static List<CurveLoop> GetBoundaryLargestFaceOfSolid(
            this Solid solid)
        {
            var faces = solid.Faces.Cast<PlanarFace>().ToList();
            var maxBottomFace = faces.OrderBy(it => it.Area).Last();
            return maxBottomFace.GetEdgesAsCurveLoops().ToList();
        }

        public static void CreateDirectShape(
            this Solid solid, Document document)
        {
            DirectShape directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
            List<GeometryObject> geometryObjects = new List<GeometryObject>() { solid };

            directShape.SetShape(geometryObjects);
        }
    }
}
