using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using UML_Designer.ViewModels;
using UML_Designer.ViewModels.Connections;

namespace UML_Designer.Views
{
   internal class CanvasItemTemplateSelector : IDataTemplate
   {
      public IDataTemplate? ClassTemplate { get; set; }
      public IDataTemplate? ConnectionTemplate { get; set; }

      public Control? Build(object? param)
      {
         return param switch
         {
            UMLClassViewModel => ClassTemplate?.Build(param),
            InheritanceConnectionViewModel => ConnectionTemplate?.Build(param),
            _ => null
         };
      }

      public bool Match(object? data)
      {
         return data is UMLClassViewModel or InheritanceConnectionViewModel;
      }
   }
}
