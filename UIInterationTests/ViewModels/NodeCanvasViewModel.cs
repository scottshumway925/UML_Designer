using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UIInterationTests.Models;

namespace UIInterationTests.ViewModels
{
   public class NodeCanvasViewModel : ViewModelBase
   {
      public ObservableCollection<AddableObjectViewModel> Nodes { get; } = new();

      private AddableObjectViewModel? _selectedNode;
      public AddableObjectViewModel? SelectedNode
      {
         get => _selectedNode;
         set => SetProperty(ref _selectedNode, value);
      }

      public List<AddableObjectViewModel> SelectedNodes { get; } = new();

      public void AddItem(double x, double y)
      {
         DeselectAllNodes();

         if (_selectedNode is null)
            Nodes.Add(new AddableObjectViewModel(new AddableObjectModel() { X = x, Y = y }));
         else
            _selectedNode = null;
      }

      public void SelectNode(AddableObjectViewModel node, bool addToSelection)
      {
         if (addToSelection)
         {
            if (node.IsSelected)
            {
               node.IsSelected = false;
               SelectedNodes.Remove(node);
            }
            else
            {
               node.IsSelected = true;
               SelectedNode = node;
               SelectedNodes.Add(node);
            }
         }
         else
         {
            DeselectAllNodes();
            SelectedNode = node;
            node.IsSelected = true;
            SelectedNodes.Add(node);
         }
         System.Diagnostics.Debug.WriteLine($"Items in selected Nodes: {SelectedNodes.Count}");
      }

      private void DeselectAllNodes()
      {
         MarkSelectedAsFalse();
         foreach (var n in SelectedNodes)
            n.IsSelected = false;

         SelectedNodes.Clear();
      }

      public void DeleteSelectedNodes()
      {
         if (SelectedNode is not null)
            SelectedNode = null;
         var toDelete = SelectedNodes.ToList();
         foreach (var node in toDelete)
            Nodes.Remove(node);

         SelectedNodes.Clear();
      }

      private void MarkSelectedAsFalse()
      {
         if (SelectedNode is not null)
            SelectedNode.IsSelected = false;
      }
   }
}
