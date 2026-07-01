using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UML_Designer.Models;
using UML_Designer.ViewModels.Connections;
using UML_Designer.ViewModels.DiagramTypes;

namespace UML_Designer.ViewModels
{
   public partial class MainWindowViewModel : ViewModelBase
   {
      private object _activeDiagram;
      public object ActiveDiagram
      {
         get => _activeDiagram;
         set => SetProperty(ref _activeDiagram, value);
      }

      public MainWindowViewModel()
      {
         ActiveDiagram = new UMLDiagramViewModel();
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

   public enum DiagramType
   {
      ClassDiagram,
      StructureChart,
      DataFlowDiagram
   }
}
