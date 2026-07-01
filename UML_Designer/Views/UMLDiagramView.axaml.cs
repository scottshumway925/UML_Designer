using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.IO;
using UML_Designer.Services;
using UML_Designer.ViewModels;
using UML_Designer.ViewModels.CanvasNodes;
using UML_Designer.ViewModels.Connections;
using UML_Designer.ViewModels.DiagramTypes;
using DotNetEnv;
using System;

namespace UML_Designer.Views
{
   public partial class UMLDiagramView : UserControl
   {
      private UMLClassViewModel? _resizingClass;
      private Point _resizeStartPointerPosition;
      private Point _resizeStartClassPosition;

      private UMLClassViewModel? _draggingClass;
      private Point _dragStartPointerPosition;
      private Point _dragStartClassPosition;

      private AIFeedbackService _aiService = new AIFeedbackService("");
      private DiagramFileService _diagramFileService = new();

      public UMLDiagramView()
      {
         InitializeComponent();
      }

      private void OnSelectClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Select;
         vm.ClearSelections();
      }


      private void OnCreateClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Create;
         vm.ClearSelections();
      }

      private void OnConnectClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentTool = ToolMode.Connect;
         vm.ClearSelections();
      }

      private void OnGetFeedbackClicked(object sender, RoutedEventArgs e)
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

      private async void OnSaveClicked(object sender, RoutedEventArgs e)
      {
         var topLevel = TopLevel.GetTopLevel(this);
         if (topLevel is null) return;

         var vm = (UMLDiagramViewModel)DataContext!;
         var save = vm.BuildSaveModel();
         var service = new DiagramFileService();
         var json = service.Serialize(save);

         var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
         {
            Title = "Save Diagram",
            DefaultExtension = "json",
            FileTypeChoices = new[]
            {
               new FilePickerFileType("UML Diagram") { Patterns = new[] { "*.json" } }
            }
         });

         if (file is null) return;

         await using var stream = await file.OpenWriteAsync();
         await using var writer = new StreamWriter(stream);
         await writer.WriteAsync(json);
      }

      private async void OnLoadClicked(object sender, RoutedEventArgs e)
      {
         var topLevel = TopLevel.GetTopLevel(this);
         if (topLevel is null) return;

         var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
         {
            Title = "Load Diagram",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
               new FilePickerFileType("UML Diagram") { Patterns = new[] { "*.json" } }
            }
         });

         if (files.Count == 0) return;

         await using var stream = await files[0].OpenReadAsync();
         using var reader = new StreamReader(stream);
         var json = await reader.ReadToEndAsync();

         var service = new DiagramFileService();
         var save = service.Deserialize(json);
         if (save is null) return;

         var vm = (UMLDiagramViewModel)DataContext!;
         vm.LoadFromSaveModel(save);
      }

      private async void OnExportClicked(object sender, RoutedEventArgs e)
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

         var vm = (UMLDiagramViewModel)DataContext!;
         vm.ClearSelections();

         await using var stream = await file.OpenWriteAsync();
         var diagramPanel = GetCanvasPanel()!;

         await _diagramFileService.ExportToPNG(diagramPanel, vm.ClassCanvas.Classes, stream);
      }

      //
      // Multiplicity Methods
      //
      private void OnMultiplicityClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Multiplicity Clicked");
         var vm = (UMLDiagramViewModel)DataContext!;

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

      private void OnMultiplicityLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var connection = (ConnectionsBaseViewModel)textBox.DataContext!;
         connection.IsEditingMultiplicity = false;
      }

      private void OnMultiplicityKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var connection = (ConnectionsBaseViewModel)textBox.DataContext!;
            connection.IsEditingMultiplicity = false;
            Focus();
         }
      }

      //
      // Now for the Connection Buttons
      //

      private void OnInheritanceClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentConnectionType = ConnectionType.Inheritance;
         vm.ClearSelections();

         System.Diagnostics.Debug.WriteLine($"Changed type to {vm.CurrentConnectionType}");
      }

      private void OnCompositionClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentConnectionType = ConnectionType.Composition;
         vm.ClearSelections();

         System.Diagnostics.Debug.WriteLine($"Changed type to {vm.CurrentConnectionType}");
      }

      private void OnAggregationClicked(object sender, RoutedEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         vm.CurrentConnectionType = ConnectionType.Aggregation;
         vm.ClearSelections();

         System.Diagnostics.Debug.WriteLine($"Changed type to {vm.CurrentConnectionType}");
      }

      /****************************************************************************************
      * Below are all of the methods for when a keyboard key is pressed
      ****************************************************************************************/
      private void OnKeyboardKeyDown(object? sender, KeyEventArgs e)
      {
         var vm = (UMLDiagramViewModel)DataContext!;
         if (e.Key == Key.Delete)
         {
            vm.ClassCanvas.DeleteSelectedClasses(vm);
            vm.ConnectionsCanvas.DeleteSelectedConnection();
         }
      }

      /****************************************************************************************
       * Below are all of the methods for when the canvas is clicked. This will be where the 
       * main crux of the system UI logic will be routed through
       ****************************************************************************************/
      private void OnCanvasClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine($"Canvas clicked, sender = {sender?.GetType().Name}, handled = {e.Handled}");

         if (e.Handled) return;
         var vm = (UMLDiagramViewModel)DataContext!;

         vm.ClearSelections();
         
         if (vm.CurrentTool != ToolMode.Create) return;

         var canvas = UMLCanvas.FindDescendantOfType<Canvas>();
         var position = e.GetPosition(canvas);
         vm.ClassCanvas.AddClass(position.X - 125, position.Y - 115);
      }

      private void OnUMLClassClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _draggingClass = null;
         e.Pointer.Capture(null);

         var vm = (UMLDiagramViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;

         if (vm.IsConnectMode)
         {
            HandleConnectionCreation(vm, selectedClass);
            return;
         }

         vm.ClearSelections();

         bool isCtrlHeld = e.KeyModifiers.HasFlag(KeyModifiers.Control);

         vm.ClassCanvas.SelectClass(selectedClass, isCtrlHeld);

         _draggingClass = selectedClass;
         _dragStartPointerPosition = e.GetPosition(UMLCanvas);
         _dragStartClassPosition = new Point(selectedClass.X, selectedClass.Y);

         e.Pointer.Capture(border);
      }

      private void OnUMLClassMoved(object? sender, PointerEventArgs e)
      {
         if (_draggingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _draggingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _dragStartPointerPosition.X;
         var deltaY = currentPosition.Y - _dragStartPointerPosition.Y;

         _draggingClass.X = _dragStartClassPosition.X + deltaX;
         _draggingClass.Y = _dragStartClassPosition.Y + deltaY;
      }

      private void OnUMLClassReleased(object? sender, PointerReleasedEventArgs e)
      {
         e.Pointer.Capture(null);
         _draggingClass = null;
      }

      /****************************************************************************************
      * Below are all of the methods for handling the text changing of the classes.
      * This will hold seperate methods for the title section of the UMLClass object.
      ****************************************************************************************/
      private void OnClassTitleClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Title Pressed");

         var vm = (UMLDiagramViewModel)DataContext!;

         if (vm.CurrentTool == ToolMode.Connect)
         {

         }
         else
         {
            if (e.ClickCount == 2)
            {
               e.Handled = true;
               var textBlock = (TextBlock)sender!;
               var node = (UMLClassViewModel)textBlock.DataContext!;
               node.IsEditingTitle = true;
            }
         }
      }

      private void OnClassTitleLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingTitle = false;
      }

      private void OnClassTitleKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingTitle = false;
            Focus();
         }
      }

      /****************************************************************************************
      * Below are all of the methods for handling the text changing of the classes.
      * This will hold seperate methods for the attributes section of the UMLClass object.
      ****************************************************************************************/
      private void OnClassAttributesClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Attributes Pressed");

         var vm = (UMLDiagramViewModel)DataContext!;

         if (vm.CurrentTool == ToolMode.Connect)
         {

         }
         else
         {
            if (e.ClickCount == 2)
            {
               e.Handled = true;
               var textBlock = (TextBlock)sender!;
               var node = (UMLClassViewModel)textBlock.DataContext!;
               node.IsEditingAttributes = true;
            }
         }
      }

      private void OnClassAttributesLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingAttributes = false;
      }

      private void OnClassAttributesKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingAttributes = false;
            Focus();
         }
      }

      /****************************************************************************************
       * Below are all of the methods for handling the text changing of the classes.
       * This will hold seperate methods for the methods section of the UMLClass object.
       ****************************************************************************************/
      private void OnClassMethodsClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Attributes Pressed");

         var vm = (UMLDiagramViewModel)DataContext!;

         if (vm.CurrentTool == ToolMode.Connect)
         {

         }
         else
         {
            if (e.ClickCount == 2)
            {
               e.Handled = true;
               var textBlock = (TextBlock)sender!;
               var node = (UMLClassViewModel)textBlock.DataContext!;
               node.IsEditingMethods = true;
            }
         }
      }

      private void OnClassMethodsLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingMethods = false;
      }

      private void OnClassMethodsKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingMethods = false;
            Focus();
         }
      }

      /****************************************************************************************
      * This portion will handle all of the resizing operations performed on a UMLClass
      * node.
      ****************************************************************************************/
      private void OnResizeAttributesHandleClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _resizingClass = null;
         e.Pointer.Capture(null);

         var vm = (UMLDiagramViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;

         vm.ClassCanvas.SelectClass(selectedClass, false);

         _resizingClass = selectedClass;
         _resizeStartPointerPosition = e.GetPosition(UMLCanvas);
         _resizeStartClassPosition = new Point(_resizingClass.Width, _resizingClass.AttributeHeight);

         e.Pointer.Capture(border);
      }

      private void OnResizeMethodsHandleClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _resizingClass = null;
         e.Pointer.Capture(null);

         var vm = (UMLDiagramViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;

         vm.ClassCanvas.SelectClass(selectedClass, false);

         _resizingClass = selectedClass;
         _resizeStartPointerPosition = e.GetPosition(UMLCanvas);
         _resizeStartClassPosition = new Point(_resizingClass.Width, _resizingClass.MethodHeight);

         e.Pointer.Capture(border);
      }

      private void OnResizeAttributesHandleMoved(object? sender, PointerEventArgs e)
      {
         if (_resizingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _resizingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _resizeStartPointerPosition.X + _resizeStartClassPosition.X;
         var deltaY = currentPosition.Y - _resizeStartPointerPosition.Y + _resizeStartClassPosition.Y;

         _resizingClass.AddWidth(deltaX);
         _resizingClass.AddAttributeHeight(deltaY);
      }

      private void OnResizeMethodsHandleMoved(object? sender, PointerEventArgs e)
      {
         if (_resizingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _resizingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _resizeStartPointerPosition.X + _resizeStartClassPosition.X;
         var deltaY = currentPosition.Y - _resizeStartPointerPosition.Y + _resizeStartClassPosition.Y;

         _resizingClass.AddWidth(deltaX);
         _resizingClass.AddMethodHeight(deltaY);
      }

      private void OnResizeHandleReleased(object? sender, PointerReleasedEventArgs e)
      {
         e.Pointer.Capture(null);
         _resizingClass = null;
      }

      /****************************************************************************************
      * This portion will handle all of the editing and deletion operations of 
      * connections between nodes.
      ****************************************************************************************/
      private void OnConnectionClick(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine($"Reached The Handler");

         e.Handled = true;
         var vm = (UMLDiagramViewModel)DataContext!;

         if (!vm.IsSelectMode)
            return;

         var element = (Control)sender!;
         var conn = (ConnectionsBaseViewModel)element.DataContext!;

         vm.ClearSelections();

         vm.ConnectionsCanvas.SelectConnection(conn);

         System.Diagnostics.Debug.WriteLine($"Connection Clicked - Selected = {conn.IsSelected}");
      }

      private void HandleConnectionCreation(UMLDiagramViewModel vm, UMLClassViewModel selectedClass)
      {
         if (vm.ConnectionsCanvas.ParentClass is null)
         {
            vm.ConnectionsCanvas.SetParent(selectedClass);
            vm.ClassCanvas.SelectClass(selectedClass, false);
         }
         else
         {
            vm.ConnectionsCanvas.CreateConnection(selectedClass, vm.CurrentConnectionType);
            vm.ClassCanvas.DeselectAllClasses();
         }
      }


      private void OnCanvasScrolled(object? sender, PointerWheelEventArgs e)
      {
         if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

         var vm = (UMLDiagramViewModel)DataContext!;
         var delta = e.Delta.Y > 0 ? 0.1 : -0.1;
         vm.ZoomLevel += delta;
         e.Handled = true;
      }

      private Panel? GetCanvasPanel()
      {
         return this.FindControl<Panel>("DiagramPanel");
      }
   }
}
