using Avalonia.Controls;
using Avalonia.Controls.Templates;
using UML_Designer.ViewModels.CanvasNodes;
using UML_Designer.ViewModels.Connections;

namespace UML_Designer.Views
{
   public class StructureChartItemTemplateSelector : IDataTemplate
   {
      public IDataTemplate? BubbleTemplate { get; set; }
      public IDataTemplate? ConnectionTemplate { get; set; }

      public Control? Build(object? param)
      {
         return param switch
         {
            StructureChartBubbleViewModel => BubbleTemplate?.Build(param),
            ConnectionsBaseViewModel => ConnectionTemplate?.Build(param),
            _ => null
         };
      }

      public bool Match(object? data)
      {
         return data is StructureChartBubbleViewModel or ConnectionsBaseViewModel;
      }
   }
}
