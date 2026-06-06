using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace UML_Designer.Models
{
   public class ConnectionLineModel
   {
      public List<Point> RoutingPoints { get; set; } = new();
      public UMLClassModel Parent { get; set; }
      public UMLClassModel Child { get; set; }

      public ConnectionLineModel(UMLClassModel parent, UMLClassModel child)
      {
         Parent = parent;
         Child = child;
      }
   }
}
