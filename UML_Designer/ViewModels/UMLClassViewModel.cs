using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models;

namespace UML_Designer.ViewModels
{
   public class UMLClassViewModel : ViewModelBase
   {
      private readonly UMLClassModel _classModel;
      public UMLClassModel GetModel()
      {
         return _classModel;
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the positioning of a UML Class node
      ****************************************************************************************/
      public double X
      {
         get => _classModel.X;
         set
         {
            _classModel.X = value;
            OnPropertyChanged(nameof(X));
         }
      }
      
      public double Y
      {
         get => _classModel.Y;
         set
         {
            _classModel.Y = value;
            OnPropertyChanged(nameof(Y));
         }
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the text contained within a 
      * UMLClass node. These can be edited to create new classes.
      ****************************************************************************************/
      public string Title
      {
         get => _classModel.Title;
         set
         {
            _classModel.Title = value;
            OnPropertyChanged(nameof(Title));
         }
      }

      public string AttributesString
      {
         get => _classModel.AttributesString;
         set
         {
            _classModel.AttributesString = value;
            OnPropertyChanged(nameof(AttributesString));
         }
      }

      public string MethodsString
      {
         get => _classModel.MethodsString;
         set
         {
            _classModel.MethodsString = value;
            OnPropertyChanged(nameof(MethodsString));
         }
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the sizing of a UMLClass node
      * on the canvas. These are created and gathered this way to allow for the
      * nodes to be resized on the individual scale. 
      ****************************************************************************************/
      public int Width
      {
         get => _classModel.Width;
         set
         {
            _classModel.Width = value;
            OnPropertyChanged(nameof(Width));
         }
      }

      public int TitleHeight
      {
         get => _classModel.TitleHeight;
         set
         {
            _classModel.TitleHeight = value;
            OnPropertyChanged(nameof(TitleHeight));
         }
      }

      public int AttributeHeight
      {
         get => _classModel.AttributeHeight;
         set
         {
            _classModel.AttributeHeight = value;
            OnPropertyChanged(nameof(AttributeHeight));
         }
      }

      public int MethodHeight
      {
         get => _classModel.MethodHeight;
         set
         {
            _classModel.MethodHeight = value;
            OnPropertyChanged(nameof(MethodHeight));
         }
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the changing of strings stored
      * in the UMLClass. These include the editing booleans.
      ****************************************************************************************/
      private bool _isEditingTitle;
      public bool IsEditingTitle
      {
         get => _isEditingTitle;
         set => SetProperty(ref _isEditingTitle, value);
      }

      private bool _isEditingAttributes;
      public bool IsEditingAttributes
      {
         get => _isEditingAttributes;
         set => SetProperty(ref _isEditingAttributes, value);
      }

      private bool _isEditingMethods;
      public bool IsEditingMethods
      {
         get => _isEditingMethods;
         set => SetProperty(ref _isEditingMethods, value);
      }



      public List<UMLClassAttribute> Attributes => _classModel.Attributes;
      public List<UMLClassMethod> Methods => _classModel.Methods;

      private bool _isSelected;
      public bool IsSelected
      {
         get => _isSelected;
         set => SetProperty(ref _isSelected, value);
      }

      public UMLClassViewModel(UMLClassModel classModel) 
      { 
         _classModel = classModel;
      }

      /****************************************************************************************
       * This portion will handle all of the resizing operations performed on a UMLClass
       * node.
       ****************************************************************************************/
      public void AddAttributeHeight(double deltaY)
      {
         AttributeHeight = (int)deltaY;
         if (AttributeHeight < 30)
            AttributeHeight = 30;
      }

      public void AddMethodHeight(double deltaY)
      {
         MethodHeight = (int)deltaY;
         if (MethodHeight < 30)
            MethodHeight = 30;
      }

      public void AddWidth(double deltaX)
      {
         Width = (int)deltaX;
         if (Width < 100)
            Width = 100;
      }

      public int GetWidth()
      {
         return Width;
      }

      public int GetTotalHeight()
      {
         return AttributeHeight + MethodHeight + 30;
      }
   }
}
