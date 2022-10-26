using Autodesk.Revit.DB.Architecture;
using CreateFloorMVVM.Core;

namespace CreateFloorMVVM.ViewModels
{
    public class RoomViewModel : BaseNotifyPropertyChanged
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
