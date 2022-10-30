using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace CreateFloorMVVM
{
    [Transaction(TransactionMode.Manual)]

    public class CreateFloorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Views.CreateFloorForm ui = new Views.CreateFloorForm();
            ui.DataContext = new ViewModels.CreateFloorViewModel(commandData);
            ui.ShowDialog();

            return Result.Succeeded;
        }
    }
}
