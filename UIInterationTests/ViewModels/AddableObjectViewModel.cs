using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIInterationTests.Models;

namespace UIInterationTests.ViewModels
{
   public class AddableObjectViewModel : ViewModelBase
   {
      private readonly AddableObjectModel _node;

      public double X => _node.X;
      public double Y => _node.Y;
      public string Label => _node.Label;

      private bool _isSelected;
      public bool IsSelected
      {
         get => _isSelected;
         set => SetProperty(ref _isSelected, value);
      }

      public AddableObjectViewModel(AddableObjectModel node)
      {
         _node = node;
      }
   }
}
