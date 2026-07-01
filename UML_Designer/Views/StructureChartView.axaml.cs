using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System;
using System.IO;
using System.Security.AccessControl;
using UML_Designer.Services;
using UML_Designer.ViewModels;
using UML_Designer.ViewModels.CanvasNodes;
using UML_Designer.ViewModels.Connections;
using UML_Designer.ViewModels.DiagramTypes;

namespace UML_Designer.Views
{
   public partial class StructureChartView : UserControl
   {
      private StructureChartBubbleViewModel? _draggingBubble;
      private Point _dragStartPointerPosition;
      private Point _dragStartClassPosition;

      private AIFeedbackService _aiService = new AIFeedbackService("");
      private DiagramFileService _diagramFileService = new();

      public StructureChartView()
      {
         InitializeComponent();
      }


      /****************************************************************************************
      * Below are all of the methods for when a keyboard key is pressed
      ****************************************************************************************/
      private void OnKeyboardKeyDown(object? sender, KeyEventArgs e)
      {
         var vm = (StructureChartViewModel)DataContext!;
         if (e.Key == Key.Delete)
         {
            vm.BubbleCanvas.DeleteSelectedBubbles(vm);
            vm.ConnectionsCanvas.DeleteSelectedConnection();
         }
      }


      /*********************************************************************************
       Here are all of the methods used for handling canvas events
       *********************************************************************************/

      private void OnCanvasClicked(object? sender, PointerPressedEventArgs e)
      {
         if (e.Handled) return;
         var vm = (StructureChartViewModel)DataContext!;

         vm.ClearSelections();

         if (vm.CurrentTool != ToolMode.Create) return;

         var canvas = UMLCanvas.FindDescendantOfType<Canvas>();
         var position = e.GetPosition(canvas);
         vm.BubbleCanvas.AddBubble(position.X - 100, position.Y - 50);
      }

      private void OnCanvasScrolled(object? sender, PointerWheelEventArgs e)
      {
         if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

         var vm = (StructureChartViewModel)DataContext!;
         var delta = e.Delta.Y > 0 ? 0.1 : -0.1;
         vm.ZoomLevel += delta;
         e.Handled = true;
      }

      /*********************************************************************************
       Here are all of the methods used for handling the basic bubble operations
       *********************************************************************************/
      private void OnBubbleClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _draggingBubble= null;
         e.Pointer.Capture(null);

         var vm = (StructureChartViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedBubble = (StructureChartBubbleViewModel)border.DataContext!;

         if (vm.IsConnectMode)
         {
            HandleConnectionCreation(vm, selectedBubble);
            return;
         }

         vm.ClearSelections();

         bool isCtrlHeld = e.KeyModifiers.HasFlag(KeyModifiers.Control);

         vm.BubbleCanvas.SelectBubble(selectedBubble, isCtrlHeld);

         _draggingBubble = selectedBubble;
         _dragStartPointerPosition = e.GetPosition(UMLCanvas);
         _dragStartClassPosition = new Point(selectedBubble.X, selectedBubble.Y);

         e.Pointer.Capture(border);
      }

      private void OnBubbleMoved(object? sender, PointerEventArgs e)
      {
         if (_draggingBubble is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _draggingBubble = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _dragStartPointerPosition.X;
         var deltaY = currentPosition.Y - _dragStartPointerPosition.Y;

         _draggingBubble.X = _dragStartClassPosition.X + deltaX;
         _draggingBubble.Y = _dragStartClassPosition.Y + deltaY;
      }

      private void OnBubbleReleased(object? sender, PointerReleasedEventArgs e)
      {
         e.Pointer.Capture(null);
         _draggingBubble = null;
      }

      /*********************************************************************************
       Here are all of the methods used for handling the bubble text operations
       *********************************************************************************/
      private void OnBubbleTextClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Attributes Pressed");

         var vm = (StructureChartViewModel)DataContext!;

         if (vm.CurrentTool == ToolMode.Connect)
         {

         }
         else
         {
            if (e.ClickCount == 2)
            {
               e.Handled = true;
               var textBlock = (TextBlock)sender!;
               var node = (StructureChartBubbleViewModel)textBlock.DataContext!;
               node.IsEditingBubble = true;
            }
         }
      }

      private void OnBubbleTextLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (StructureChartBubbleViewModel)textBox.DataContext!;
         node.IsEditingBubble = false;
      }

      private void OnBubbleTextKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var node = (StructureChartBubbleViewModel)textBox.DataContext!;
            node.IsEditingBubble = false;
            Focus();
         }
      }

      /*********************************************************************************
       Here are all of the methods used for handling the connection operations
       *********************************************************************************/
      private void OnConnectionClick(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine($"Reached The Handler");

         e.Handled = true;
         var vm = (StructureChartViewModel)DataContext!;

         if (!vm.IsSelectMode)
            return;

         var element = (Control)sender!;
         var conn = (ConnectionsBaseViewModel)element.DataContext!;

         vm.ClearSelections();

         vm.ConnectionsCanvas.SelectConnection(conn);

         System.Diagnostics.Debug.WriteLine($"Connection Clicked - Selected = {conn.IsSelected}");
      }
      
      private void OnConnectionTextClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Multiplicity Clicked");
         var vm = (StructureChartViewModel)DataContext!;

         if (vm.CurrentTool == ToolMode.Connect)
         {

         }
         else
         {
            if (e.ClickCount == 2)
            {
               e.Handled = true;
               var textBlock = (TextBlock)sender!;
               var connection = (ConnectionsBaseViewModel)textBlock.DataContext!;
               connection.IsEditingMultiplicity = true;
            }
         }
      }

      private void OnConnectionTextLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var connection = (ConnectionsBaseViewModel)textBox.DataContext!;
         connection.IsEditingMultiplicity = false;
      }

      private void OnConnectionTextKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var connection = (ConnectionsBaseViewModel)textBox.DataContext!;
            connection.IsEditingMultiplicity = false;
            Focus();
         }
      }

      /*********************************************************************************
       Here are all of the methods used for handling button clicks
       *********************************************************************************/

      private void OnSelectClicked(object? sender, RoutedEventArgs e)
      {
         var vm = (StructureChartViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Select;
         vm.ClearSelections();
      }

      private void OnCreateClicked(object? sender, RoutedEventArgs e)
      {
         var vm = (StructureChartViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Create;
         vm.ClearSelections();
      }

      private void OnConnectClicked(object? sender, RoutedEventArgs e)
      {
         var vm = (StructureChartViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Connect;
         vm.ClearSelections();
      }

      private async void OnExportClicked(object? sender, RoutedEventArgs e)
      {
         var topLevel = TopLevel.GetTopLevel(this);
         if (topLevel is null) return;

         var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
         {
            Title = "Export Diagram",
            DefaultExtension = "png",
            FileTypeChoices = new[]
   {
               new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } }
            }
         });

         if (file is null)
            return;

         var vm = (StructureChartViewModel)DataContext!;
         vm.ClearSelections();

         await using var stream = await file.OpenWriteAsync();
         var diagramPanel = GetCanvasPanel()!;

         await _diagramFileService.ExportToPNG(diagramPanel, vm.BubbleCanvas.Bubbles, stream);
      }

      private async void OnSaveClicked(object? sender, RoutedEventArgs e)
      {
         var topLevel = TopLevel.GetTopLevel(this);
         if (topLevel is null) return;

         var vm = (StructureChartViewModel)DataContext!;
         var save = vm.BuildSaveModel();
         var service = new DiagramFileService();
         var json = service.Serialize(save);

         var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
         {
            Title = "Save Diagram",
            DefaultExtension = "json",
            FileTypeChoices = new[]
            {
               new FilePickerFileType("Structure chart") { Patterns = new[] { "*.json" } }
            }
         });

         if (file is null) return;

         await using var stream = await file.OpenWriteAsync();
         await using var writer = new StreamWriter(stream);
         await writer.WriteAsync(json);
      }

      private async void OnLoadClicked(object? sender, RoutedEventArgs e)
      {
         var topLevel = TopLevel.GetTopLevel(this);
         if (topLevel is null) return;

         var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
         {
            Title = "Load Diagram",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
               new FilePickerFileType("Structure Chart") { Patterns = new[] { "*.json" } }
            }
         });

         if (files.Count == 0) return;

         await using var stream = await files[0].OpenReadAsync();
         using var reader = new StreamReader(stream);
         var json = await reader.ReadToEndAsync();

         var service = new DiagramFileService();
         var save = service.Deserialize(json);
         if (save is null) return;

         var vm = (StructureChartViewModel)DataContext!;
         vm.LoadFromSaveModel(save);
      }

      private void OnGetFeedbackClicked(object? sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         var save = vm.BuildSaveModel();
         string? apiKey = Environment.GetEnvironmentVariable("API_KEY");
         if (string.IsNullOrEmpty(apiKey))
            throw new Exception("API KEY NOT FOUND");

         _aiService = new AIFeedbackService(apiKey);
         var feedbackWindow = new FeedbackWindow(_aiService, _diagramFileService, save);
         feedbackWindow.Show();
      }


      /*********************************************************************************
      Here are all of the helper methods for the methods above
      *********************************************************************************/
      private void HandleConnectionCreation(StructureChartViewModel vm, StructureChartBubbleViewModel selectedBubble)
      {
         if (vm.ConnectionsCanvas.ParentBubble is null)
         {
            System.Diagnostics.Debug.WriteLine("Parent is null");
            vm.ConnectionsCanvas.SetParent(selectedBubble);
            vm.BubbleCanvas.SelectBubble(selectedBubble, false);
         }
         else
         {
            System.Diagnostics.Debug.WriteLine("Parent not null");
            vm.ConnectionsCanvas.CreateConnection(selectedBubble);
            vm.ClearSelections();
         }
      }

      private Panel? GetCanvasPanel()
      {
         return this.FindControl<Panel>("DiagramPanel");
      }
   }
}
