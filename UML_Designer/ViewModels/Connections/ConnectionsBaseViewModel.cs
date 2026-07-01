using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using UML_Designer.Models;
using Avalonia.Media;
using UML_Designer.ViewModels.CanvasNodes;

namespace UML_Designer.ViewModels.Connections
{
   public abstract class ConnectionsBaseViewModel : ViewModelBase
   {
      protected readonly ConnectionLineModel _model;
      protected IConnectableNode _parent;
      protected IConnectableNode _child;

      protected bool _isSelected;
      public bool IsSelected
      {
         get => _isSelected;
         set
         {
            SetProperty(ref _isSelected, value);
            OnPropertyChanged(nameof(StrokeColor));
            OnPropertyChanged(nameof(StrokeThickness));
            OnPropertyChanged(nameof(FillColor));
         }
      }

      protected virtual string typeOfConn => "Inheritance";

      public IBrush StrokeColor => IsSelected ? Brushes.Blue : Brushes.Black;
      public virtual IBrush FillColor => Brushes.LightGray;
      public double StrokeThickness => IsSelected ? 4 : 2;

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

      /****************************************************
       Methods and attributes for handling multiplicity
       ****************************************************/

      public Point MultiplicityLocation
      {
         get => _model.MultiplicityLocation;
         set
         {
            _model.MultiplicityLocation = value;
            OnPropertyChanged(nameof(MultiplicityLocation));
         }
      }

      public string MultiplicityString
      {
         get => _model.Multiplicity ?? "";
         set
         {
            _model.Multiplicity = value;
            OnPropertyChanged(nameof(MultiplicityString));
         }
      }

      protected bool _isEditingMultiplicity;
      public bool IsEditingMultiplicity
      {
         get => _isEditingMultiplicity;
         set => SetProperty(ref _isEditingMultiplicity, value);
      }


      public ConnectionsBaseViewModel(ConnectionLineModel model, IConnectableNode parent, IConnectableNode child)
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
         _model.ParentId = newParent.GetModel().Id;
         _parent.PropertyChanged += OnNodeChanged;
         LinePoints = SetLines();
         OnPropertyChanged(nameof(ShapePoints));
      }

      public void ChangeChild(UMLClassViewModel newChild)
      {
         _child.PropertyChanged -= OnNodeChanged;
         _child = newChild;
         _model.ParentId = newChild.GetModel().Id;
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

      public IConnectableNode GetParentClass()
      {
         return _parent;
      }

      public IConnectableNode GetChildClass()
      {
         return _child;
      }

      public ConnectionLineModel GetModel()
      {
         return _model;
      }

      protected abstract List<Point> SetLines();
   }
}
