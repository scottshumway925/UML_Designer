using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.Models
{
   public class UMLClassAttribute
   {
      public enum Visibility { Public, Private, Protected };
      public enum Abstraction { Abstract, Virtual, Regular };
      public string DataType { get; set; } = "Int";
      public string Name { get; set; } = "Attribute";
   }
}
