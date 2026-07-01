using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models;
using UML_Designer.ViewModels.CanvasNodes;
using UML_Designer.ViewModels.Connections;

namespace UML_Designer.ViewModels.DiagramTypes
{
   public class StructureChartViewModel : ViewModelBase
   {
      public StructureChartCanvasViewModel BubbleCanvas { get; } = new StructureChartCanvasViewModel();
      public ConnectionsHandlerViewModel ConnectionsCanvas { get; } = new ConnectionsHandlerViewModel();
      public ObservableCollection<object> CanvasItems { get; } = new();

      private double _zoomLevel = 1.0;
      public double ZoomLevel
      {
         get => _zoomLevel;
         set => SetProperty(ref _zoomLevel, Math.Clamp(value, 0.01, 5.0));
      }

      public bool IsSelectMode => CurrentTool == ToolMode.Select;
      public bool IsConnectMode => CurrentTool == ToolMode.Connect;
      public bool IsCreateMode => CurrentTool == ToolMode.Create;

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


      private DiagramType _selectedDiagram = DiagramType.ClassDiagram;
      public DiagramType SelectedDiagram
      {
         get => _selectedDiagram;
         set
         {
            SetProperty(ref _selectedDiagram, value);
            OnPropertyChanged(nameof(IsClassDiagram));
            OnPropertyChanged(nameof(IsDataFlowDiagram));
            OnPropertyChanged(nameof(IsStructureChart));
         }
      }

      public bool IsClassDiagram => SelectedDiagram == DiagramType.ClassDiagram;
      public bool IsDataFlowDiagram => SelectedDiagram == DiagramType.DataFlowDiagram;
      public bool IsStructureChart => SelectedDiagram == DiagramType.StructureChart;

      public StructureChartViewModel()
      {
         BubbleCanvas.Bubbles.CollectionChanged += (s, e) =>
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

      public void ClearSelections()
      {
         BubbleCanvas.DeselectAllBubbles();
      }

      public DiagramSaveModel BuildSaveModel()
      {
         var save = new DiagramSaveModel();

         foreach (var bubbleVm in BubbleCanvas.Bubbles)
            save.Bubbles.Add(bubbleVm.GetModel());

         foreach (var connectionVm in ConnectionsCanvas.Connections)
            save.Connections.Add(connectionVm.GetModel());

         return save;
      }

      public void LoadFromSaveModel(DiagramSaveModel save)
      {
         BubbleCanvas.Bubbles.Clear();
         ConnectionsCanvas.Connections.Clear();
         CanvasItems.Clear();

         var classLookup = new Dictionary<Guid, StructureChartBubbleViewModel>();
         foreach (var bubbleModel in save.Bubbles)
         {
            var vm = new StructureChartBubbleViewModel(bubbleModel);
            BubbleCanvas.Bubbles.Add(vm);
            classLookup[bubbleModel.Id] = vm;
         }

         foreach (var connModel in save.Connections)
         {
            if (!classLookup.TryGetValue(connModel.ParentId, out var parent)) continue;
            if (!classLookup.TryGetValue(connModel.ChildId, out var child)) continue;

            StructureChartConnectionViewModel conn = new StructureChartConnectionViewModel(connModel, parent, child);

            ConnectionsCanvas.Connections.Add(conn);
         }
      }

      public void SelectBubble()
      {

      }
   }
}
