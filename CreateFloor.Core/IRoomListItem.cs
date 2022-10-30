using Autodesk.Revit.DB.Architecture;

namespace CreateFloor.Core
{
    public interface IRoomListItem
    {
        Room RevitRoom { get; set; }
        string Name { get;  }

    }
}
