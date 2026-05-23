using System.Collections.ObjectModel;
using UIInterationTests.Models;

namespace UIInterationTests.ViewModels
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      public NodeCanvasViewModel Canvas { get; } = new NodeCanvasViewModel();
   }
}
