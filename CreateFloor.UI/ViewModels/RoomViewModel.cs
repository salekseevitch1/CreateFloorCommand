using Autodesk.Revit.DB.Architecture;
using CreateFloor.Core;

namespace CreateFloor.UI.ViewModels
{
    public class RoomViewModel : BaseNotifyPropertyChanged, ISelectableItem, IRoomListItem
    {

        private bool _isSelected { get; set; }

        public Room RevitRoom { get; set; }
        public string Name
        {
            get
            {
                return $"{RevitRoom.Number} - {RevitRoom.Name}";
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public RoomViewModel(Room revitRoom)
        {
            RevitRoom = revitRoom;
        }
    }
}
