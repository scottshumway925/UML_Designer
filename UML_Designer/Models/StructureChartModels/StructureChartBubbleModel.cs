using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.Models.StructureChartModels
{
   public class StructureChartBubbleModel
   {
      public Guid Id { get; set; }= Guid.NewGuid();
      public double X { get; set; }
      public double Y { get; set; }
      public int Height { get; set; } = 100;
      public int Width { get; set; } = 200;
      public string BubbleText { get; set; } = "functionName()";
   }
}
