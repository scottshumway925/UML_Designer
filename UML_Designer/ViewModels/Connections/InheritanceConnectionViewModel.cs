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
   public class InheritanceConnectionViewModel : ViewModelBase
   {
      private readonly ConnectionLineModel _model;
      private UMLClassViewModel _parent;
      private UMLClassViewModel _child;

      private int _yOffset = 20;
      private int _yConnectionOffset = 30;
      private int _xConnectionOffset = 20;
      private int _yChildOffset = 10;

      private bool _isSelected;
      public bool IsSelected
      {
         get => _isSelected;
         set
         {
            SetProperty(ref _isSelected, value);
            OnPropertyChanged(nameof(StrokeColor));
            OnPropertyChanged(nameof(StrokeThickness));
         }
      }

      public IBrush StrokeColor => IsSelected ? Brushes.Blue : Brushes.Black;
      public double StrokeThickness => IsSelected ? 4 : 2;

      public double X2 => _parent.X + _parent.Width / 2;
      public double Y2 => _parent.Y + _parent.GetTotalHeight();

      public List<Point> ArrowPoints => CalculateArrowPoints();
      public List<Point> LinePoints
      {
         get => _model.RoutingPoints;
         private set
         {
            _model.RoutingPoints = value;
            OnPropertyChanged(nameof(LinePoints));
         }
      }

      public void ChangeParent(UMLClassViewModel newParent)
      {
         _parent.PropertyChanged -= OnNodeChanged;
         _parent = newParent;
         _model.Parent = newParent.GetModel();
         _parent.PropertyChanged += OnNodeChanged;
         LinePoints = SetLines();
         OnPropertyChanged(nameof(ArrowPoints));
      }

      public void ChangeChild(UMLClassViewModel newChild)
      {
         _child.PropertyChanged -= OnNodeChanged;
         _child = newChild;
         _model.Parent = newChild.GetModel();
         _child.PropertyChanged += OnNodeChanged;
         LinePoints = SetLines();
         OnPropertyChanged(nameof(ArrowPoints));
      }

      public InheritanceConnectionViewModel(ConnectionLineModel model, UMLClassViewModel parent, UMLClassViewModel child)
      {
         _model = model;
         _parent = parent;
         _child = child;

         _parent.PropertyChanged += OnNodeChanged;
         _child.PropertyChanged += OnNodeChanged;

         LinePoints = SetLines();
      }

      private List<Point> CalculateArrowPoints()
      {
         double tipX = X2;
         double tipY = Y2;

         return new List<Point>
         {
            new Point(tipX, tipY),           // tip pointing at parent
            new Point(tipX - 10, tipY + _yOffset), // bottom left corner
            new Point(tipX + 10, tipY + _yOffset)  // bottom right corner
         };
      }

      public void OnNodeChanged(object? sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName is nameof(UMLClassViewModel.X)
                            or nameof(UMLClassViewModel.Y)
                            or nameof(UMLClassViewModel.Width)
                            or nameof(UMLClassViewModel.AttributeHeight)
                            or nameof(UMLClassViewModel.MethodHeight))
         {
            LinePoints = SetLines();
            OnPropertyChanged(nameof(ArrowPoints));
         }
      }

      public UMLClassViewModel GetParentClass()
      {
         return _parent;
      }

      public UMLClassViewModel GetChildClass()
      {
         return _child;
      }

      public List<Point> SetLines()
      {
         double connectChildLineOffset = 5;

         // For occasions when the child is below the parent
         if (_parent.Y + _parent.GetTotalHeight() + _yConnectionOffset < _child.Y - _yChildOffset)
         {
            double middleY = _child.Y - _yOffset;
            double midParentX = _parent.X + _parent.GetWidth() / 2;
            double midChildX = _child.X + _child.GetWidth() / 2;

            return new List<Point>
            {
               new Point(midParentX, _parent.Y + _parent.GetTotalHeight()),   // Bottom of parent
               new Point(midParentX, middleY),                                // Bottom of parent offset
               new Point(midChildX, middleY),                                 // Top of child offset
               new Point(midChildX, _child.Y + connectChildLineOffset)        // Top of child
            };
         }
         // For occasions when the child is above the parent
         else
         {
            // Line going from the bottom of the parent straight down
            double midParentX = _parent.X + _parent.GetWidth() / 2;
            double bottomParentY = _parent.Y + _parent.GetTotalHeight();
            double bottomParentYOffset = bottomParentY + _yConnectionOffset;
            

            // Line going from the top of the child straight up
            double midChildX = _child.X + _child.GetWidth() / 2;
            double topChildY = _child.Y;
            double topChildYOffset = topChildY - _yChildOffset;
            

            double offsetXParentChild;
            // When the child is not overlapping on the x axis
            if (_child.X + _child.GetWidth() < _parent.X || _parent.X + _parent.GetWidth() < _child.X)
            {
               // When the child is to the left of the parent and doesn't overlap on the x axis
               if (_child.X + _child.GetWidth() < _parent.X)
                  offsetXParentChild = (_child.X + _child.GetWidth() + _parent.X) / 2;
               // When the child is to the right of the parent and doesn't overlap on the x axis
               else
                  offsetXParentChild = (_child.X + _parent.GetWidth() + _parent.X) / 2;               
            }

            // When the child is overlapping on the x axis
            else
            {
               
               // When the child is overlapping on the x axis, but is to the right of the parent
               if (_parent.X + _parent.GetWidth() / 2 < _child.X + _child.GetWidth() / 2 && _child.X < _parent.X + _parent.GetWidth() + 5)
               {
                  if (_child.X + _child.GetWidth() < _parent.X + _parent.GetWidth())
                     offsetXParentChild = _parent.X + _parent.GetWidth() + _xConnectionOffset;
                  else
                     offsetXParentChild = _child.X + _child.GetWidth() + _xConnectionOffset;
               }
               // When the child is overlapping on the x axis, but is to the left of the parent
               else
               {
                  if (_child.X < _parent.X)
                     offsetXParentChild = _child.X - _xConnectionOffset;
                  else
                     offsetXParentChild = _parent.X - _xConnectionOffset;
               }
            }

            return new List<Point>
            {
               new Point(midParentX, bottomParentY),                    // Bottom of the parent
               new Point(midParentX, bottomParentYOffset),              // Bottom of the parent y offset
               new Point(offsetXParentChild, bottomParentYOffset),      // Offset of the parent and child parent y level
               new Point(offsetXParentChild, topChildYOffset),          // Offset of the parent and child child y level
               new Point(midChildX, topChildYOffset),                   // Top of child offset
               new Point(midChildX, topChildY + connectChildLineOffset) // Top of child
            };
         }
      }



   }
}
