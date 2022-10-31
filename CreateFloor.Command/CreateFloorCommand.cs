using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CreateFloor.UI.Views;
using CreateFloor.UI.ViewModels;
using Prism.DryIoc;
using CreateFloor.Core;
using Prism.Ioc;
using Prism.Mvvm;

namespace CreateFloor.Command
{
    [Transaction(TransactionMode.Manual)]

    public class CreateFloorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            DryIocContainerExtension container = new DryIocContainerExtension();
            container.Register<IFloorBuilder, FloorBuilder>();
            container.Register<IFloorCreateSettings, FloorCreateSettings>();
            container.Register<CreateFloorViewModel>();
            container.Register<CreateFloorWindow>();
            container.Register<ExternalCommandData>(() => commandData);
            ViewModelLocationProvider.Register<CreateFloorWindow, CreateFloorViewModel>();

            container.Resolve<CreateFloorWindow>().ShowDialog();

            return Result.Succeeded;
        }
    }
}
