using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models.StructureChartModels;

namespace UML_Designer.Models
{
   public class DiagramSaveModel
   {
      public List<UMLClassModel> Classes { get; set; } = new();
      public List<StructureChartBubbleModel> Bubbles { get; set; } = new();
      public List<ConnectionLineModel> Connections { get; set; } = new();
   }
}
