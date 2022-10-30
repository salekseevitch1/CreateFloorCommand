using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateFloorMVVM.Utils
{
    internal class FloorBuilder
    {
        #region Public methods
        public static List<Floor> CreateFloorInRooms(List<Room> rooms, ElementId floorTypeId, double offsetByLevelInMillimeters=0)
        {
            Document document = rooms[0].Document;
            List<Floor> floorList = new List<Floor>();

            foreach (Room room in rooms)
            {
                List<Solid> floorSolids = new List<Solid>();

                var roomSolid = room.GetRoomBoundariesPoints().GetOnlyUniquePoints().RemovePointsOnSameLine().CreateCurveLoopByPoints().CreateSolidByCurveLoop();

                floorSolids.Add(roomSolid);

                foreach (var boundaries in room.GetRoomBoundaries())
                {
                    if (boundaries.Value == null || !(boundaries.Value is Wall)) continue;

                    var curve = boundaries.Key as Curve;
                    var wall = boundaries.Value as Wall;

                    var doorsInRoom = room.FilterDoorsByRoom(wall.GetDoorsInWall());

                    foreach (var door in doorsInRoom)
                    {
                        var doorLoop = CreateCurveLoopAtDoor(door, boundaries.Key);

                        if (doorLoop == null) continue;

                        var solid = new List<CurveLoop> { doorLoop }.CreateSolidByCurveLoop();
                        floorSolids.Add(solid);
                    }
                }

                var floorSolid = floorSolids.UnionSolids();
                var floorLoops = floorSolid.GetBoundaryLargestFaceOfSolid();

                var floor = Floor.Create(document, floorLoops, floorTypeId, room.LevelId);

                double value = UnitUtils.ConvertToInternalUnits(offsetByLevelInMillimeters, UnitTypeId.Millimeters);
                floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(value);

                floorList.Add(floor);
            }

            return floorList;
        }

        #endregion

        #region Private methods

        private static CurveLoop CreateCurveLoopAtDoor(FamilyInstance door, Curve boudariesCurve)
        {
            var doorLoop = new CurveLoop();

            var doorLocationPoint = door.Location as LocationPoint;
            var doorWidth = door.Document.GetElement(door.GetTypeId()).get_Parameter(BuiltInParameter.DOOR_WIDTH).AsDouble();

            var wall = door.Host as Wall;
            var wallLocationCurve = wall.Location as LocationCurve;
            var wallCurve = wallLocationCurve.Curve as Line;
            var wallDirection = wallCurve.Direction;

            var p1 = doorLocationPoint.Point + (wallDirection * (doorWidth / 2));
            var p2 = doorLocationPoint.Point - (wallDirection * (doorWidth / 2));

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

        #endregion

    }
}
