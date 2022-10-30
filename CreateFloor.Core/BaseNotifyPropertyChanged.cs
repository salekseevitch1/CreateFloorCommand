using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CreateFloorMVVM.Core
{
    public class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string porp = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(porp));
        }
    }
}
