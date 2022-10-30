using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;

namespace CreateFloor.Extensions
{
    public static class FamilyInstanceExtensions
    {
        public static Element GetFamilyType(
            this FamilyInstance familyInstance)
        {
            return familyInstance.Document.GetElement(familyInstance.GetTypeId());
        }

        public static List<FamilyInstance> FilteredByRoom(
             this List<FamilyInstance> familyInstances, Room room)
        {
            var filteredDoors = new List<FamilyInstance>();
            int roomId = room.Id.IntegerValue;

            foreach (var door in familyInstances)
            {
                if (door.ToRoom != null && door.ToRoom.Id.IntegerValue == roomId)
                    filteredDoors.Add(door);

                else if (door.FromRoom != null && door.FromRoom.Id.IntegerValue == roomId)
                    filteredDoors.Add(door);
            }

            return filteredDoors;
        }
    }
}
