using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using CreateFloor.Core;
using CreateFloor.UI.Models;
using CreateFloor.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CreateFloor.UI.ViewModels
{
    public class CreateFloorViewModel : BaseNotifyPropertyChanged
    {
        #region Private Propertys
        Document _document;
        private string _searchString;
        private ListCollectionView _roomsListView;
        private ObservableCollection<RoomViewModel> _roomsOnActiveView;
        private ObservableCollection<FloorType> _floorTypes;
        private FloorType _selectionFloorType;
        private double _widthSelectionFloorType;
        private double _offsetByLevel;
        #endregion

        #region Public Propertys
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                OnPropertyChanged();
                RoomsListView.Refresh();
            }
        }
        public ListCollectionView RoomsListView
        {
            get
            {
                return _roomsListView;
            }
        }
        public ObservableCollection<RoomViewModel> RoomsOnActiveView
        {
            get
            {
                return _roomsOnActiveView;
            }
            set
            {
                _roomsOnActiveView = value;
                OnPropertyChanged();
            }
        }
        public FloorType SelectionFloorType
        {
            get
            {
                return _selectionFloorType;
            }
            set
            {
                _selectionFloorType = value as FloorType;
                OnPropertyChanged();
                WidthSelectionFloorType = GetWidthSelectionFloor();
            }
        }
        public ObservableCollection<FloorType> FloorTypes
        {
            get
            {
                return _floorTypes;
            }
            set
            {
                _floorTypes = value;
                OnPropertyChanged();
            }
        }
        public double WidthSelectionFloorType
        {
            get
            {
                return _widthSelectionFloorType;
            }
            set
            {
                _widthSelectionFloorType = value;
                OnPropertyChanged();
            }
        }
        public double OffsetByLevel
        {
            get
            {
                return _offsetByLevel;
            }
            set
            {
                _offsetByLevel = value;
                OnPropertyChanged();
            }
        }

        public IFloorCreateSettings Settings { get; set; }

        #endregion

        #region Public Constructor
        public CreateFloorViewModel(ExternalCommandData commandData)
        {
            _document = commandData.Application.ActiveUIDocument.Document;

            RoomsOnActiveView = new ObservableCollection<RoomViewModel>(new FilteredElementCollector(_document, _document.ActiveView.Id)
                                    .OfCategory(BuiltInCategory.OST_Rooms)
                                    .WhereElementIsNotElementType()
                                    .Cast<Room>()
                                    .Select(it => new RoomViewModel(it))
                                    .ToList());

            this._roomsListView = new ListCollectionView(RoomsOnActiveView);
            this._roomsListView.Filter = RoomNameFilter;

            FloorTypes = new ObservableCollection<FloorType>(new FilteredElementCollector(_document)
                                    .OfCategory(BuiltInCategory.OST_Floors)
                                    .WhereElementIsElementType()
                                    .Cast<FloorType>()
                                    .ToList());

            SelectionFloorType = FloorTypes.First();
        }
        #endregion

        #region Public Commands
        public ICommand CheckAll
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    foreach (RoomViewModel room in RoomsListView)
                    {
                        room.IsSelected = true;
                    }
                });
            }
        }
        public ICommand UnCheckAll
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    foreach (RoomViewModel room in RoomsListView)
                    {
                        room.IsSelected = false;
                    }
                });
            }
        }
        public ICommand ToggleAll
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    foreach (RoomViewModel room in RoomsListView)
                    {
                        if (room.IsSelected == true) room.IsSelected = false;
                        else if (room.IsSelected == false) room.IsSelected = true;
                    }
                });
            }
        }
        public ICommand CreateFloor
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    CreateFloors();
                });
            }
        }
        #endregion

        #region Private Commands

        private void CreateFloors()
        {
            Settings = new FloorCreateSettings() { Rooms = GetSelectionRooms(), FloorTypeId = SelectionFloorType.Id };

            using (Transaction tr = new Transaction(_document, "CreateFloors"))
            {
                tr.Start();
                Utils.FloorBuilder.CreateFloorInRooms(Settings);
                tr.Commit();
            }

        }
        private bool RoomNameFilter(object item)
        {
            if (String.IsNullOrEmpty(SearchString))
                return true;
            else
                return ((item as RoomViewModel).Name.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private List<Room> GetSelectionRooms()
        {
            return RoomsOnActiveView.Where(it => it.IsSelected).Select(it => it.RevitRoom).ToList();
        }
        private double GetWidthSelectionFloor()
        {
            var selectionFloorWidth = SelectionFloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
            return UnitUtils.ConvertFromInternalUnits(selectionFloorWidth, UnitTypeId.Millimeters);
        }
        #endregion

    }
}
