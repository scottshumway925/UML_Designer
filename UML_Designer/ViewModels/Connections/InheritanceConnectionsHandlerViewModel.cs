using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models;
using UML_Designer.ViewModels.CanvasNodes;
using UML_Designer.ViewModels.DiagramTypes;

namespace UML_Designer.ViewModels.Connections
{
   public class ConnectionsHandlerViewModel : ViewModelBase
   {
      public ObservableCollection<ConnectionsBaseViewModel> Connections { get; } = new();
      public UMLClassViewModel? ParentClass;
      public StructureChartBubbleViewModel? ParentBubble;
      public ConnectionsBaseViewModel? SelectedConnection { get; set; }

      private bool _newParentSelected;
      public bool NewParentSelected
      {
         get { return _newParentSelected; }
         set
         {
            _newParentSelected = value;
            OnPropertyChanged(nameof(NewParentSelected));
         }
      }
      
      public void DeselectClasses()
      {
         ParentClass = null;
         NewParentSelected = false;
      }

      public void DeselectBubbles()
      {
         ParentBubble = null;
         NewParentSelected = false;
      }

      public void SetParent(UMLClassViewModel selectedClass)
      {
         NewParentSelected = true;
         ParentClass = selectedClass;
      }

      public void CreateConnection(UMLClassViewModel childClass, ConnectionType type)
      {
         if (ParentClass is null || ParentClass == childClass)
         {
            DeselectClasses();
            return;
         }

         var model = new ConnectionLineModel(ParentClass.GetModel(), childClass.GetModel());
         
         switch (type)
         {
            case ConnectionType.Inheritance:
               Connections.Add(new InheritanceConnectionViewModel(model, ParentClass, childClass));
               break;
            case ConnectionType.Aggregation:
               Connections.Add(new AggregationConnectionViewModel(model, ParentClass, childClass));
               break;
            case ConnectionType.Composition:
               Connections.Add(new CompositionConnectionViewModel(model, ParentClass, childClass));
               break;
         }
         
         ParentClass = null;
         NewParentSelected = false;
      }

      public void SetParent(StructureChartBubbleViewModel selectedBubble)
      {
         NewParentSelected = true;
         ParentBubble = selectedBubble;
      }

      public void CreateConnection(StructureChartBubbleViewModel childBubble)
      {
         if (ParentBubble is null || ParentBubble == childBubble)
         {
            DeselectBubbles();
            return;
         }

         var model = new ConnectionLineModel(ParentBubble.GetModel(), childBubble.GetModel());

         Connections.Add(new StructureChartConnectionViewModel(model, ParentBubble, childBubble));

         ParentBubble = null;
         NewParentSelected = false;
      }

      public void SelectConnection(ConnectionsBaseViewModel connection)
      {
         if (connection.IsSelected)
         {
            connection.IsSelected = false;
            SelectedConnection = null;
         }
         else
         {
            connection.IsSelected = true;
            SelectedConnection = connection;
         }
      }

      public void DeselectConnections()
      {
         if (SelectedConnection is null)
            return;

         SelectedConnection.IsSelected = false;
         SelectedConnection = null;
      }
      
      public void DeleteSelectedConnection()
      {
         if (SelectedConnection is null)
            return;

         Connections.Remove(SelectedConnection);
         DeselectConnections();
      }

      public void DeleteRelatedConnections(object? TBDClass)
      {
         var potentialDeletes = Connections.ToList();
         foreach (var conn in potentialDeletes)
         {
            if (conn.GetParentClass() == TBDClass || conn.GetChildClass() == TBDClass)
            {
               SelectedConnection = conn;
               DeleteSelectedConnection();
            }
         }
      }

      public void ChangeParent(InheritanceConnectionViewModel connection, UMLClassViewModel newParent)
      {
         connection.ChangeParent(newParent);
      }

      public void ChangeChild(InheritanceConnectionViewModel connection, UMLClassViewModel newChild)
      {
         connection.ChangeChild(newChild);
      }
   }
}
