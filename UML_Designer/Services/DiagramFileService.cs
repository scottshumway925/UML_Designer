using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UML_Designer.Models;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls;
using System.IO;
using Avalonia;
using Avalonia.Threading;
using System.Threading.Tasks;
using UML_Designer.ViewModels.CanvasNodes;


namespace UML_Designer.Services
{
   public class DiagramFileService
   {
      private readonly JsonSerializerOptions _options = new()
      {
         WriteIndented = true
      };

      public string Serialize(DiagramSaveModel diagram)
      {
         return JsonSerializer.Serialize(diagram, _options);
      }

      public DiagramSaveModel? Deserialize(string json)
      {
         return JsonSerializer.Deserialize<DiagramSaveModel>(json, _options);
      }

      public string CreateAIPrompt(DiagramSaveModel diagram, String initialPrompt = "")
      {
         var classLookup = diagram.Classes.ToDictionary(c => c.Id, c => c.Title);

         string promptString = "Here is a class diagram that was built using an application converted to text. I would like to gather your feedback as to ways I can improve this class diagram. The user prompt is shown below, followed by the application prompt. If there is no user prompt, offer feedback based on how the overall diagram looks:\n";
         promptString += initialPrompt;

         promptString += "Classes\n";
         foreach (var classVm in diagram.Classes)
         {
            promptString += "\nClass\n";
            promptString += classVm.Title + "\n";
            promptString += "Attributes:\n" + classVm.AttributesString + "\n";
            promptString += "Methods:\n" + classVm.MethodsString + "\n";
         }

         promptString += "\nConnections\n";
         foreach (var conn in diagram.Connections)
         {
            var parentTitle = classLookup.TryGetValue(conn.ParentId, out var p) ? p : "Unknown";
            var childTitle = classLookup.TryGetValue(conn.ChildId, out var c) ? c : "Unknown";
            promptString += $"\n  {conn.ConnectionType}: {childTitle} -> {conn.Multiplicity} {parentTitle}\n";
         }

         return promptString;
      }

      public async Task ExportToPNG(Control rootElement, IEnumerable<UMLClassViewModel> classes, Stream stream)
      {
         if (!classes.Any())
            return;

         double currentZoom = 1.0;
         if (rootElement.RenderTransform is ScaleTransform scale)
            currentZoom = scale.ScaleX;

         rootElement.RenderTransform = new ScaleTransform(1.0, 1.0);

         await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);

         double padding = 20;

         // Step 1 — Render full canvas to RenderTargetBitmap
         var fullRender = new RenderTargetBitmap(new PixelSize(5000, 5000));
         fullRender.Render(rootElement);

         // Step 2 — Save full render to memory stream for decoding
         using var memStream = new MemoryStream();
         fullRender.Save(memStream);
         memStream.Position = 0;

         // Step 3 — Decode memory stream back as WriteableBitmap
         var fullWriteable = WriteableBitmap.Decode(memStream);

         int cropX = 0;
         int cropY = 0;
         int exportWidth = 0;
         int exportHeight = 0;

         // Step 4 — Scan pixels to find actual content bounds
         using (var srcBuf = fullWriteable.Lock())
         {
            unsafe
            {
               byte* src = (byte*)srcBuf.Address;
               int stride = srcBuf.RowBytes;

               int foundLeft = 5000;
               int foundTop = 5000;
               int foundRight = 0;
               int foundBottom = 0;

               for (int y = 0; y < 5000; y++)
               {
                  for (int x = 0; x < 5000; x++)
                  {
                     byte* pixel = src + ((long)y * stride) + ((long)x * 4);
                     byte b = pixel[0];
                     byte g = pixel[1];
                     byte r = pixel[2];
                     byte a = pixel[3];

                     // Skip transparent and light gray background pixels
                     bool isBackground = (b > 200 && g > 200 && r > 200) || a == 0;

                     if (!isBackground)
                     {
                        if (x < foundLeft) foundLeft = x;
                        if (x > foundRight) foundRight = x;
                        if (y < foundTop) foundTop = y;
                        if (y > foundBottom) foundBottom = y;
                     }
                  }
               }

               System.Diagnostics.Debug.WriteLine($"Content bounds: left={foundLeft} top={foundTop} right={foundRight} bottom={foundBottom}");

               cropX = Math.Max(0, foundLeft - (int)padding);
               cropY = Math.Max(0, foundTop - (int)padding);
               exportWidth = (foundRight - foundLeft) + (int)(padding * 2);
               exportHeight = (foundBottom - foundTop) + (int)(padding * 2);

               System.Diagnostics.Debug.WriteLine($"cropX: {cropX}, cropY: {cropY}");
               System.Diagnostics.Debug.WriteLine($"exportWidth: {exportWidth}, exportHeight: {exportHeight}");
            }
         }

         // Step 5 — Create cropped WriteableBitmap and copy pixels
         var cropped = new WriteableBitmap(
             new PixelSize(exportWidth, exportHeight),
             new Vector(96, 96),
             Avalonia.Platform.PixelFormat.Bgra8888,
             Avalonia.Platform.AlphaFormat.Premul);

         using (var srcBuf = fullWriteable.Lock())
         using (var dstBuf = cropped.Lock())
         {
            unsafe
            {
               byte* src = (byte*)srcBuf.Address;
               byte* dst = (byte*)dstBuf.Address;

               int srcBytesPerPixel = srcBuf.RowBytes / 5000;
               int dstBytesPerPixel = dstBuf.RowBytes / exportWidth;

               System.Diagnostics.Debug.WriteLine($"srcBytesPerPixel: {srcBytesPerPixel}");
               System.Diagnostics.Debug.WriteLine($"dstBytesPerPixel: {dstBytesPerPixel}");

               for (int y = 0; y < exportHeight; y++)
               {
                  int srcRow = y + cropY;
                  if (srcRow < 0 || srcRow >= 5000) continue;

                  byte* srcLine = src + ((long)srcRow * srcBuf.RowBytes) + ((long)cropX * srcBytesPerPixel);
                  byte* dstLine = dst + ((long)y * dstBuf.RowBytes);

                  int bytesToCopy = exportWidth * srcBytesPerPixel;
                  Buffer.MemoryCopy(srcLine, dstLine, dstBuf.RowBytes, bytesToCopy);
               }
            }
         }

         System.Diagnostics.Debug.WriteLine($"Pixel copy complete");

         // Debug — verify cropped bitmap has content
         var debug4 = System.IO.Path.Combine(
             System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
             "debug_4_cropped.png");
         using (var s = System.IO.File.OpenWrite(debug4))
            cropped.Save(s);
         System.Diagnostics.Debug.WriteLine($"Debug 4 saved to: {debug4}");

         // Final save to the provided stream
         cropped.Save(stream);
         System.Diagnostics.Debug.WriteLine($"Final save complete");

         rootElement.RenderTransform = new ScaleTransform(currentZoom, currentZoom);
      }

      public async Task ExportToPNG(Control rootElement, IEnumerable<StructureChartBubbleViewModel> classes, Stream stream)
      {
         if (!classes.Any())
            return;

         double currentZoom = 1.0;
         if (rootElement.RenderTransform is ScaleTransform scale)
            currentZoom = scale.ScaleX;

         rootElement.RenderTransform = new ScaleTransform(1.0, 1.0);

         await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);

         double padding = 20;

         // Step 1 — Render full canvas to RenderTargetBitmap
         var fullRender = new RenderTargetBitmap(new PixelSize(5000, 5000));
         fullRender.Render(rootElement);

         // Step 2 — Save full render to memory stream for decoding
         using var memStream = new MemoryStream();
         fullRender.Save(memStream);
         memStream.Position = 0;

         // Step 3 — Decode memory stream back as WriteableBitmap
         var fullWriteable = WriteableBitmap.Decode(memStream);

         int cropX = 0;
         int cropY = 0;
         int exportWidth = 0;
         int exportHeight = 0;

         // Step 4 — Scan pixels to find actual content bounds
         using (var srcBuf = fullWriteable.Lock())
         {
            unsafe
            {
               byte* src = (byte*)srcBuf.Address;
               int stride = srcBuf.RowBytes;

               int foundLeft = 5000;
               int foundTop = 5000;
               int foundRight = 0;
               int foundBottom = 0;

               for (int y = 0; y < 5000; y++)
               {
                  for (int x = 0; x < 5000; x++)
                  {
                     byte* pixel = src + ((long)y * stride) + ((long)x * 4);
                     byte b = pixel[0];
                     byte g = pixel[1];
                     byte r = pixel[2];
                     byte a = pixel[3];

                     // Skip transparent and light gray background pixels
                     bool isBackground = (b > 200 && g > 200 && r > 200) || a == 0;

                     if (!isBackground)
                     {
                        if (x < foundLeft) foundLeft = x;
                        if (x > foundRight) foundRight = x;
                        if (y < foundTop) foundTop = y;
                        if (y > foundBottom) foundBottom = y;
                     }
                  }
               }

               System.Diagnostics.Debug.WriteLine($"Content bounds: left={foundLeft} top={foundTop} right={foundRight} bottom={foundBottom}");

               cropX = Math.Max(0, foundLeft - (int)padding);
               cropY = Math.Max(0, foundTop - (int)padding);
               exportWidth = (foundRight - foundLeft) + (int)(padding * 2);
               exportHeight = (foundBottom - foundTop) + (int)(padding * 2);

               System.Diagnostics.Debug.WriteLine($"cropX: {cropX}, cropY: {cropY}");
               System.Diagnostics.Debug.WriteLine($"exportWidth: {exportWidth}, exportHeight: {exportHeight}");
            }
         }

         // Step 5 — Create cropped WriteableBitmap and copy pixels
         var cropped = new WriteableBitmap(
             new PixelSize(exportWidth, exportHeight),
             new Vector(96, 96),
             Avalonia.Platform.PixelFormat.Bgra8888,
             Avalonia.Platform.AlphaFormat.Premul);

         using (var srcBuf = fullWriteable.Lock())
         using (var dstBuf = cropped.Lock())
         {
            unsafe
            {
               byte* src = (byte*)srcBuf.Address;
               byte* dst = (byte*)dstBuf.Address;

               int srcBytesPerPixel = srcBuf.RowBytes / 5000;
               int dstBytesPerPixel = dstBuf.RowBytes / exportWidth;

               System.Diagnostics.Debug.WriteLine($"srcBytesPerPixel: {srcBytesPerPixel}");
               System.Diagnostics.Debug.WriteLine($"dstBytesPerPixel: {dstBytesPerPixel}");

               for (int y = 0; y < exportHeight; y++)
               {
                  int srcRow = y + cropY;
                  if (srcRow < 0 || srcRow >= 5000) continue;

                  byte* srcLine = src + ((long)srcRow * srcBuf.RowBytes) + ((long)cropX * srcBytesPerPixel);
                  byte* dstLine = dst + ((long)y * dstBuf.RowBytes);

                  int bytesToCopy = exportWidth * srcBytesPerPixel;
                  Buffer.MemoryCopy(srcLine, dstLine, dstBuf.RowBytes, bytesToCopy);
               }
            }
         }

         System.Diagnostics.Debug.WriteLine($"Pixel copy complete");

         // Debug — verify cropped bitmap has content
         var debug4 = System.IO.Path.Combine(
             System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
             "debug_4_cropped.png");
         using (var s = System.IO.File.OpenWrite(debug4))
            cropped.Save(s);
         System.Diagnostics.Debug.WriteLine($"Debug 4 saved to: {debug4}");

         // Final save to the provided stream
         cropped.Save(stream);
         System.Diagnostics.Debug.WriteLine($"Final save complete");

         rootElement.RenderTransform = new ScaleTransform(currentZoom, currentZoom);
      }
   }
}
