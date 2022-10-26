using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
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
                var points = MyGeometryUtils.RemovePointsOnSameLine(MyGeometryUtils.OnlyUniquePoints(MyRoomUtils.GetRoomBoundariesPoints(room)));
                var roomLoop = MyGeometryUtils.CreateCurveLoopByPoints(points);
                var roomSolid = MyGeometryUtils.CreateSolidByCurveLoop(roomLoop);

                floorSolids.Add(roomSolid);

                foreach (var boundaries in MyRoomUtils.GetRoomBoundaries(room.Document, room))
                {
                    if (boundaries.Value == null || !(boundaries.Value is Wall)) continue;

                    var doorsInRoom = FilterDoorsByRoom(room, GetDoorsInWall(boundaries.Value as Wall));

                    foreach (var door in doorsInRoom)
                    {
                        var doorLoop = CreateCurveLoopAtDoor(door, boundaries.Key);

                        if (doorLoop == null) continue;

                        var solid = MyGeometryUtils.CreateSolidByCurveLoop(new List<CurveLoop> { doorLoop });
                        floorSolids.Add(solid);
                    }
                }

                var floorSolid = MyGeometryUtils.UnionSolids(floorSolids);
                var floorLoops = MyGeometryUtils.GetBoundaryLargestFaceOfSolid(floorSolid);

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

        private static List<FamilyInstance> GetDoorsInWall(Wall wall)
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

        private static List<FamilyInstance> FilterDoorsByRoom(Room room, List<FamilyInstance> doors)
        {
            var filteredDoors = new List<FamilyInstance>();
            int roomId = room.Id.IntegerValue;

            foreach (var door in doors)
            {
                if (door.ToRoom != null && door.ToRoom.Id.IntegerValue == roomId) 
                    filteredDoors.Add(door);

                else if (door.FromRoom != null && door.FromRoom.Id.IntegerValue == roomId) 
                    filteredDoors.Add(door);
            }

            return filteredDoors;
        }

        #endregion

    }
}
