using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Designer.Models
{
   public class UMLClassMethod
   {
      public enum Visibility { Public, Private, Protected };
      public enum Abstraction { Abstract, Virtual, Regular };
      public string ReturnType { get; set; } = "Void";
      public string Name { get; set; } = "Method";
      public string PassedVariables { get; set; } = "number : Int";
   }
}
