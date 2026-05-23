namespace UML_Designer.ViewModels
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      public UMLClassCanvasViewModel ClassCanvas { get; } = new UMLClassCanvasViewModel();
   }
}
