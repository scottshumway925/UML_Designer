using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.Models
{
   public class UMLClassModel : NodeModelBase
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public double X {  get; set; }
      public double Y { get; set; }
      public int Width { get; set; } = 250;
      public int TitleHeight { get; set; } = 30;
      public int AttributeHeight { get; set; } = 100;
      public int MethodHeight { get; set; } = 100;
      public string Title { get; set; } = "Class";
      public string AttributesString { get; set; } = "Attributes";
      public string MethodsString { get; set; } = "Methods";
   }
}
