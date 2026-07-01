using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.Models
{
   public class NodeModelBase
   {
      public int Height = 100;
      public Guid Id { get; set; } = Guid.NewGuid();
      public double X { get; set; }
      public double Y { get; set; }
      public int Width { get; set; } = 250;
   }
}
