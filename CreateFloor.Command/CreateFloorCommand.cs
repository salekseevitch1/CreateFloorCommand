using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CreateFloor.UI.Views;
using CreateFloor.UI.ViewModels;

namespace CreateFloor.Command
{
    [Transaction(TransactionMode.Manual)]

    public class CreateFloorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //AssemblyLoader.Load();

            var ui = new CreateFloorForm();
            ui.DataContext = new CreateFloorViewModel(commandData);
            ui.ShowDialog();

            return Result.Succeeded;
        }
    }
}
