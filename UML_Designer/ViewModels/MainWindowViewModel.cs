using System.Collections.ObjectModel;
using System;
using UML_Designer.Views;
using UML_Designer.ViewModels.Connections;

namespace UML_Designer.ViewModels
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      private ToolMode _currentTool;
      public ToolMode CurrentTool
      {
         get => _currentTool;
         set
         {
            SetProperty(ref _currentTool, value);
            OnPropertyChanged(nameof(IsSelectMode));
            OnPropertyChanged(nameof(IsConnectMode));
            OnPropertyChanged(nameof(IsCreateMode));
         }
      }

      private ConnectionType _currentConnectionType;
      public ConnectionType CurrentConnectionType
      {
         get => _currentConnectionType;
         set
         {
            SetProperty(ref _currentConnectionType, value);
            OnPropertyChanged(nameof(IsAggregationMode));
            OnPropertyChanged(nameof(IsCompositionMode));
            OnPropertyChanged(nameof(IsInheritanceMode));
         }
      }

      public ObservableCollection<object> CanvasItems { get; } = new();

      public MainWindowViewModel()
      {
         ClassCanvas.Classes.CollectionChanged += (s, e) =>
         {
            if (e.NewItems != null)
               foreach (var item in e.NewItems)
                  CanvasItems.Add(item);

            if (e.OldItems != null)
               foreach (var item in e.OldItems)
                  CanvasItems.Remove(item);
         };

         ConnectionsCanvas.Connections.CollectionChanged += (s, e) =>
         {
            if (e.NewItems != null)
               foreach (var item in e.NewItems)
                  CanvasItems.Insert(0, item);

            if (e.OldItems != null)
               foreach (var item in e.OldItems)
                  CanvasItems.Remove(item);
         };
      }

      private double _zoomLevel = 1.0;
      public double ZoomLevel
      {
         get => _zoomLevel;
         set => SetProperty(ref _zoomLevel, Math.Clamp(value, 0.1, 5.0));
      }

      public bool IsSelectMode => CurrentTool == ToolMode.Select;
      public bool IsConnectMode => CurrentTool == ToolMode.Connect;
      public bool IsCreateMode => CurrentTool == ToolMode.Create;

      public bool IsCompositionMode => CurrentConnectionType == ConnectionType.Composition;
      public bool IsAggregationMode => CurrentConnectionType == ConnectionType.Aggregation;
      public bool IsInheritanceMode => CurrentConnectionType == ConnectionType.Inheritance;

      public UMLClassCanvasViewModel ClassCanvas { get; } = new UMLClassCanvasViewModel();
      public InheritanceConnectionsHandlerViewModel ConnectionsCanvas { get; } = new InheritanceConnectionsHandlerViewModel();

      public void ClearSelections()
      {
         ClassCanvas.DeselectAllClasses();
         ConnectionsCanvas.DeselectClasses();
         ConnectionsCanvas.DeselectConnections();
      }

      public void ClearClasses()
      {
         ClassCanvas.DeselectAllClasses();
      }

      public void ClearConnections()
      {
         ConnectionsCanvas.DeselectConnections();
      }
   }

   public enum ToolMode
   {
      Select,
      Create,
      Connect
   }

   public enum ConnectionType
   {
      Inheritance,
      Composition,
      Aggregation
   }
}
