using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models;
using UML_Designer.Models.StructureChartModels;

namespace UML_Designer.ViewModels.CanvasNodes
{
   public class StructureChartBubbleViewModel : ViewModelBase, IConnectableNode
   {
      private readonly StructureChartBubbleModel _bubbleModel;
      public StructureChartBubbleModel GetModel()
      {
         return _bubbleModel;
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the positioning of a bubble node
      ****************************************************************************************/
      public double X
      {
         get => _bubbleModel.X;
         set
         {
            _bubbleModel.X = value;
            OnPropertyChanged(nameof(X));
         }
      }

      public double Y
      {
         get => _bubbleModel.Y;
         set
         {
            _bubbleModel.Y = value;
            OnPropertyChanged(nameof(Y));
         }
      }

      public int Width
      {
         get => _bubbleModel.Width;
         set
         {
            _bubbleModel.Width = value;
            OnPropertyChanged(nameof(Width));
         }
      }

      public int Height
      {
         get => _bubbleModel.Height;
         set
         {
            _bubbleModel.Height = value;
            OnPropertyChanged(nameof(Height));
         }
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the text of a bubble node
      ****************************************************************************************/
      public string BubbleText
      {
         get => _bubbleModel.BubbleText;
         set
         {
            _bubbleModel.BubbleText = value;
            OnPropertyChanged(nameof(BubbleText));
         }
      }

      /****************************************************************************************
      * Below are all of the attributes that handle the booleans of a bubble node
      ****************************************************************************************/
      private bool _isEditingBubble;
      public bool IsEditingBubble
      {
         get => _isEditingBubble;
         set
         {
            _isEditingBubble = value;
            OnPropertyChanged(nameof(IsEditingBubble));
         }
      }

      private bool _isSelected;
      public bool IsSelected
      {
         get => _isSelected;
         set
         {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
         }
      }


      public StructureChartBubbleViewModel() 
      {
         _bubbleModel = new StructureChartBubbleModel();
      }

      public StructureChartBubbleViewModel(StructureChartBubbleModel bubbleModel)
      {
         _bubbleModel = bubbleModel;
      }

      public int GetWidth()
      {
         return Width;
      }

      public int GetTotalHeight()
      {
         return Height;
      }
   }
}
