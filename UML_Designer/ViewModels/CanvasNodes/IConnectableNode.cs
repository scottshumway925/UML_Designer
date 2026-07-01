using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.ViewModels.CanvasNodes
{
   public interface IConnectableNode : INotifyPropertyChanged
   {
      double X { get; set; }
      double Y { get; set; }
      int Width { get; }
      public int GetTotalHeight();
      public int GetWidth();
   }
}
