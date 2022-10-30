using Autodesk.Revit.DB;
using CreateFloor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateFloor.UI.Models
{
    public class Door
    {
        public FamilyInstance RevitElement { get; private set; }
        public Wall Wall
        {
            get
            {
                return RevitElement.Host as Wall;
            }
        }
        public LocationPoint LocationPoint
        {
            get
            {
                return RevitElement.Location as LocationPoint;
            }
        }
        public double Width
        {
            get
            {
                return RevitElement.GetFamilyType().get_Parameter(BuiltInParameter.DOOR_WIDTH).AsDouble();
            }
        }

        public Door(FamilyInstance revitDoor)
        {
            RevitElement = revitDoor;
        }

        public CurveLoop CreateCurveLoopAtDoor(Curve boudariesCurve) // Нужно придумать название
        {
            var doorLoop = new CurveLoop();

            var p1 = LocationPoint.Point + (Wall.GetDirection() * (Width / 2));
            var p2 = LocationPoint.Point - (Wall.GetDirection() * (Width / 2));

            var p3 = boudariesCurve.Project(p1).XYZPoint;
            var p4 = boudariesCurve.Project(p2).XYZPoint;

            try
            {
                doorLoop.Append(Line.CreateBound(p1, p2));
                doorLoop.Append(Line.CreateBound(p2, p4));
                doorLoop.Append(Line.CreateBound(p4, p3));
                doorLoop.Append(Line.CreateBound(p3, p1));
            }

            catch (Exception) { return null; }

            return doorLoop;
        }
    }
}
