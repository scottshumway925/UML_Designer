using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.IO;
using UML_Designer.Services;
using UML_Designer.ViewModels;
using UML_Designer.ViewModels.Connections;
using UML_Designer.ViewModels.DiagramTypes;

namespace UML_Designer.Views
{
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void OnDiagramTypeChanged(object? sender, SelectionChangedEventArgs e)
      {
         if (DataContext is not MainWindowViewModel vm) return;

         var comboBox = (ComboBox)sender!;
         var selected = (ComboBoxItem)comboBox.SelectedItem!;

         vm.ActiveDiagram = selected.Content switch
         {
            "UML Class Diagram" => new UMLDiagramViewModel(),
            "Structure Chart" => new StructureChartViewModel(),
            "Data Flow Diagram" => new DataFlowDiagramViewModel(),
            _ => vm.ActiveDiagram
         };
      }
   }
}