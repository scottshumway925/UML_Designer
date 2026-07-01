using Avalonia.Controls;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UML_Designer.Models;
using UML_Designer.Models.StructureChartModels;
using UML_Designer.ViewModels.DiagramTypes;

namespace UML_Designer.ViewModels.CanvasNodes
{
   public class StructureChartCanvasViewModel : ViewModelBase
   {
      public ObservableCollection<StructureChartBubbleViewModel> Bubbles { get; } = new();
      private StructureChartBubbleViewModel? _selectedBubble;
      public StructureChartBubbleViewModel? SelectedBubble
      {
         get => _selectedBubble;
         set => SetProperty(ref _selectedBubble, value);
      }

      public List<StructureChartBubbleViewModel> SelectedBubbles{ get; } = new();

      public StructureChartCanvasViewModel() 
      {
           
      }

      public void DeleteSelectedBubbles(StructureChartViewModel vm)
      {
         if (SelectedBubble is not null)
            SelectedBubble = null;
         var toDelete = SelectedBubbles.ToList();
         foreach (StructureChartBubbleViewModel node in toDelete)
         {
            vm.ConnectionsCanvas.DeleteRelatedConnections(node);
            Bubbles.Remove(node);
         }

         SelectedBubbles.Clear();
      }

      public void AddBubble(double x, double y)
      {
         DeselectAllBubbles();

         if (_selectedBubble is null)
            Bubbles.Add(new StructureChartBubbleViewModel(new StructureChartBubbleModel() { X = x, Y = y }));
         else
            _selectedBubble = null;
      }

      public void DeselectAllBubbles()
      {
         MarkSelectedAsFalse();
         foreach (var b in SelectedBubbles)
            b.IsSelected = false;

         SelectedBubbles.Clear();
      }

      private void MarkSelectedAsFalse()
      {
         if (SelectedBubble is not null)
            SelectedBubble.IsSelected = false;
         SelectedBubble = null;
      }

      public void SelectBubble(StructureChartBubbleViewModel bubble, bool addToSelectedList)
      {
         SelectedBubble = bubble;
         SelectedBubble.IsSelected = true;
      }
   }
}
