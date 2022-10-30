using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CreateFloor.Core;
using CreateFloorMVVM.Models;
using RevitExtensions;
using System.Collections.Generic;
using System.Linq;

namespace CreateFloorMVVM.Utils
{
    internal class FloorBuilder
    {
        #region Public methods
        public static List<Floor> CreateFloorInRooms(IFloorCreateSettings floorCreateSettings)
        {
            List<Floor> floorList = new List<Floor>();

            foreach (Room room in floorCreateSettings.Rooms)
            {
                List<Solid> floorSolids = new List<Solid>();

                var roomSolid = room.GetRoomBoundariesPoints().GetOnlyUniquePoints().RemovePointsOnSameLine().CreateCurveLoopByPoints().CreateSolidByCurveLoops();

                floorSolids.Add(roomSolid);

                foreach (var boundaries in room.GetRoomBoundaries())
                {
                    if (boundaries.Value == null || (boundaries.Value is Wall) == false) continue;

                    var curve = boundaries.Key as Curve;
                    var wall = boundaries.Value as Wall;

                    var doorsInRoom = wall.GetDoorsInWall().FilteredByRoom(room).Select(it => new Door(it));

                    foreach (var door in doorsInRoom)
                    {
                        var doorLoop = door.CreateCurveLoopAtDoor(curve);

                        if (doorLoop == null) continue;

                        var solid = doorLoop.CreateSolidByCurveLoop();
                        floorSolids.Add(solid);
                    }
                }

                var floorSolid = floorSolids.UnionSolids();
                var floorLoops = floorSolid.GetBoundaryLargestFaceOfSolid();

                var floor = Floor.Create(floorCreateSettings.Document, floorLoops, floorCreateSettings.FloorTypeId, room.LevelId);
                floor.SetHeightAboutLevel(floorCreateSettings.OffsetByLevelInMillimeters);

                floorList.Add(floor);
            }

            return floorList;
        }

        #endregion
    }
}
