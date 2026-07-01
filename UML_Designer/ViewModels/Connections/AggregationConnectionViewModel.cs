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
   public class AggregationConnectionViewModel : ConnectionsBaseViewModel
   {
      public AggregationConnectionViewModel(ConnectionLineModel model, UMLClassViewModel parent, UMLClassViewModel child) : base(model, parent, child)
      {
         _model.ConnectionType = typeOfConn;
      }

      protected override string typeOfConn => "Aggregation";

      protected override List<Point> CalculateEndShape()
      {
         int offset = 10;
         double X2;
         double Y2 = _parent.Y + _parent.GetTotalHeight() / 2;

         if (_parent.X + (_parent.GetWidth() / 2) > _child.X + (_child.GetWidth() / 2))
         {
            X2 = _parent.X - offset - offset;
            MultiplicityLocation = new Point(_parent.X - (offset * 5), Y2 - (offset * 2));
         }
         else
         {
            X2 = _parent.X + _parent.GetWidth();
            MultiplicityLocation = new Point(_parent.X + _parent.GetWidth() + (offset * 3), Y2 - (offset * 2));
         }

         return new List<Point>
         {
            new Point(X2, Y2),           // tip pointing at parent
            new Point(X2 + offset, Y2 + offset), // bottom left corner
            new Point(X2 + offset + offset, Y2),
            new Point(X2 + offset, Y2 - offset) // bottom right corner
            
         };
      }

      protected override List<Point> SetLines()
      {
         int xOffset = 20;

         double midParentY = _parent.Y + _parent.GetTotalHeight() / 2;
         double midChildY = _child.Y + _child.GetTotalHeight() / 2;

         double connectChildLineOffset = 5;

         // For occasions when the child is to the right of the parent
         if (_parent.X + _parent.GetWidth() < _child.X)
         {
            double middleX = (_parent.X + _parent.GetWidth() + _child.X) / 2;

            return new List<Point>
            {
               new Point(_parent.X, midParentY),
               new Point(middleX, midParentY),
               new Point(middleX, midChildY),
               new Point(_child.X + connectChildLineOffset, midChildY)
            };
         }
         // For occasions when the child is to the left of the parent
         else if (_parent.X > _child.X + _child.GetWidth())
         {
            double middleX = (_child.X + _child.GetWidth() + _parent.X) / 2; ;

            return new List<Point>
            {
               new Point(_parent.X + xOffset, midParentY),
               new Point(middleX, midParentY),
               new Point(middleX, midChildY),
               new Point(_child.X + connectChildLineOffset, midChildY)
            };
         }
         // For occasions when the child is overlapping more to the left
         else if (_parent.X + (_parent.GetWidth() / 2) > _child.X + (_child.GetWidth() / 2))
         {
            double offsetValue;
            if (_child.X < _parent.X)
               offsetValue = _child.X - xOffset;
            else
               offsetValue = _parent.X - xOffset;

            return new List<Point>
            {
               new Point(_parent.X, midParentY),
               new Point(offsetValue, midParentY),
               new Point(offsetValue, midChildY),
               new Point(_child.X + xOffset, midChildY)
            };
         }
         else
         {
            double offsetValue;
            if (_child.X + _child.GetWidth() < _parent.X + _parent.GetWidth())
               offsetValue = _parent.X + _parent.GetWidth() + xOffset;
            else
               offsetValue = _child.X + _child.GetWidth() + xOffset;

            return new List<Point>
            {
               new Point(_parent.X + connectChildLineOffset, midParentY),
               new Point(offsetValue, midParentY),
               new Point(offsetValue, midChildY),
               new Point(_child.X + connectChildLineOffset, midChildY)
            };
         }
      }
   }
}
