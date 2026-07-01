using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using UML_Designer.Models;
using Avalonia.Media;
using UML_Designer.ViewModels.CanvasNodes;

namespace UML_Designer.ViewModels.Connections
{
   public class StructureChartConnectionViewModel : ConnectionsBaseViewModel
   {
      public StructureChartConnectionViewModel(ConnectionLineModel model, StructureChartBubbleViewModel parent, StructureChartBubbleViewModel child) : base(model, parent, child)
      {
         _model.ConnectionType = typeOfConn;
      }

      protected override string typeOfConn => "FunctionCall";

      protected override List<Point> CalculateEndShape()
      {
         double yOffset = 20;
         double X2;
         double Y2;
         // Pointing Up
         if (_parent.Y + _parent.GetTotalHeight() / 2 < _child.Y + _child.GetTotalHeight() / 2)
         {
            X2 = _parent.X + _parent.Width / 2;
            Y2 = _parent.Y + _parent.GetTotalHeight();

            MultiplicityLocation = new Point(X2 + 15, Y2 + yOffset + 5);

            return new List<Point>
            {
               new Point(X2, Y2),           // tip pointing at parent
               new Point(X2 - 10, Y2 + yOffset), // bottom left corner
               new Point(X2 + 10, Y2 + yOffset), // bottom right corner
            };
         }
         // Pointing Down
         else
         {
            X2 = _parent.X + _parent.Width / 2;
            Y2 = _parent.Y;

            MultiplicityLocation = new Point(X2 + 15, Y2 - yOffset - 10);

            return new List<Point>
            {
               new Point(X2, Y2),           // tip pointing at parent
               new Point(X2 - 10, Y2 - yOffset), // bottom left corner
               new Point(X2 + 10, Y2 - yOffset), // bottom right corner
            };
         }
         
      }

      protected override List<Point> SetLines()
      {
         int yOffset = 20;

         double midParentX = _parent.X + _parent.Width / 2;
         double midChildX = _child.X + _child.Width / 2;

         // When the caller is on top
         if (_parent.Y + _parent.GetTotalHeight() / 2 < _child.Y + _child.GetTotalHeight() / 2)
         {
            return new List<Point>
            {
               new Point(midParentX, _parent.Y + (_parent.GetTotalHeight()) + yOffset),
               new Point(midChildX, _child.Y + 5)
            };
         }

         // When the callee is on top
         else
         {
            return new List<Point>
            {
               new Point(midParentX, _parent.Y - yOffset),
               new Point(midChildX, _child.Y + (_child.GetTotalHeight()))
            };
         }
      }
   }
}
