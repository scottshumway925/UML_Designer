using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using UML_Designer.Models;
using Avalonia.Media;

namespace UML_Designer.ViewModels.Connections
{
   public class CompositionConnectionViewModel : ConnectionsBaseViewModel
   {
      public CompositionConnectionViewModel(ConnectionLineModel model, UMLClassViewModel parent, UMLClassViewModel child) : base(model, parent, child)
      {
      }

      protected override List<Point> CalculateEndShape()
      {
         throw new NotImplementedException();
      }

      protected override List<Point> SetLines()
      {
         throw new NotImplementedException();
      }
   }
}
