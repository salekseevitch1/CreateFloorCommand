using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace CreateFloor.Extensions
{
    public static class CurveLoopExtension
    {
        public static void CreateDirectShape(
            this CurveLoop curves, Document document)
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
            this CurveLoop curveLoop, double heigth = 1)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { curveLoop }, XYZ.BasisZ, heigth);
        }

        public static Solid CreateSolidByCurveLoops(
            this List<CurveLoop> curveLoop, double heigth = 1)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(curveLoop, XYZ.BasisZ, heigth);
        }
    }
}
