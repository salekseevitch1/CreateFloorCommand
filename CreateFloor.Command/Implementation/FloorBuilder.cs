using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CreateFloor.Core;
using CreateFloor.UI.Models;
using CreateFloor.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CreateFloor.Command
{
    public class FloorBuilder : IFloorBuilder
    {
        #region Public methods
        public List<Floor> CreateFloor(IFloorCreateSettings floorCreateSettings)
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
                floor.SetHeightAboutLevel(floorCreateSettings.BaseOffset);

                floorList.Add(floor);
            }

            return floorList;
        }

        #endregion
    }
}
