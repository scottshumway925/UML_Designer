using System;
using System.Collections.Generic;
using Avalonia;
using UML_Designer.Models.StructureChartModels;

namespace UML_Designer.Models
{
   public class ConnectionLineModel
   {
      public List<Point> RoutingPoints { get; set; } = new();
      public Guid ParentId { get; set; }
      public Guid ChildId { get; set; }
      public string ConnectionType { get; set; } = "Inheritance";
      public string? Multiplicity { get; set; } = "*";
      public Point MultiplicityLocation { get; set; }

      public ConnectionLineModel() { }
      public ConnectionLineModel(UMLClassModel parent, UMLClassModel child, string connType = "Inheritance")
      {
         ParentId = parent.Id;
         ChildId = child.Id;
         ConnectionType = connType;
      }

      public ConnectionLineModel(StructureChartBubbleModel caller, StructureChartBubbleModel callee)
      {
         ParentId = caller.Id;
         ChildId = callee.Id;
         ConnectionType = "FunctionCall";
         Multiplicity = "*****";
      }
   }
}
