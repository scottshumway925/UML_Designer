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
   public abstract class ConnectionsBaseViewModel : ViewModelBase
   {
      protected readonly ConnectionLineModel _model;
      protected UMLClassViewModel _parent;
      protected UMLClassViewModel _child;

      protected bool _isSelected;
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

      public List<Point> ShapePoints => CalculateEndShape();
      public List<Point> LinePoints
      {
         get => _model.RoutingPoints;
         private set
         {
            _model.RoutingPoints = value;
            OnPropertyChanged(nameof(LinePoints));
         }
      }

      public ConnectionsBaseViewModel(ConnectionLineModel model, UMLClassViewModel parent, UMLClassViewModel child)
      {
         _model = model;
         _parent = parent;
         _child = child;

         _parent.PropertyChanged += OnNodeChanged;
         _child.PropertyChanged += OnNodeChanged;

         LinePoints = SetLines();
      }

      public void ChangeParent(UMLClassViewModel newParent)
      {
         _parent.PropertyChanged -= OnNodeChanged;
         _parent = newParent;
         _model.Parent = newParent.GetModel();
         _parent.PropertyChanged += OnNodeChanged;
         LinePoints = SetLines();
         OnPropertyChanged(nameof(ShapePoints));
      }

      public void ChangeChild(UMLClassViewModel newChild)
      {
         _child.PropertyChanged -= OnNodeChanged;
         _child = newChild;
         _model.Parent = newChild.GetModel();
         _child.PropertyChanged += OnNodeChanged;
         LinePoints = SetLines();
         OnPropertyChanged(nameof(ShapePoints));
      }

      protected abstract List<Point> CalculateEndShape();

      public void OnNodeChanged(object? sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName is nameof(UMLClassViewModel.X)
                            or nameof(UMLClassViewModel.Y)
                            or nameof(UMLClassViewModel.Width)
                            or nameof(UMLClassViewModel.AttributeHeight)
                            or nameof(UMLClassViewModel.MethodHeight))
         {
            LinePoints = SetLines();
            OnPropertyChanged(nameof(ShapePoints));
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

      protected abstract List<Point> SetLines();
   }
}
