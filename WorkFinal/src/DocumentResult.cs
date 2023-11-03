using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using DataPoint = Polynom.DataPoint;
using Element = iTextSharp.text.Element;

namespace WorkFinal;

public class DocumentResult : IDocResult
{
    private readonly DataGrid _dataGrid = DataGrid.GetInstance();


    public void DocResult(double result, string outPath)
    {
        using (var memoryStream = new MemoryStream())
        {
            var document = new Document();
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var title = new Paragraph("Report");
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            var dataPointsParagraph = new Paragraph("Point coordinates:");
            document.Add(dataPointsParagraph);

            foreach (var dataPoint in _dataGrid.GetCollection())
            {
                var pointInfo = new Paragraph($"X: {dataPoint.X}, Y: {dataPoint.Y}");
                document.Add(pointInfo);
            }

            var graphText = new Paragraph("Graph:");
            document.Add(graphText);

            var graphImageStream = GenerateGraphImage(_dataGrid.GetCollection());
            var graphImage = Image.GetInstance(graphImageStream.ToArray());
            document.Add(graphImage);

            var calculationResult = new Paragraph($"Calculations results: {result}");
            document.Add(calculationResult);

            document.Close();

            File.WriteAllBytes(outPath, memoryStream.ToArray());
        }
    }

    private MemoryStream GenerateGraphImage(List<DataPoint> dataPoints)
    {
        var plotModel = new PlotModel();
        var series = new LineSeries();

        foreach (var dataPoint in dataPoints) series.Points.Add(new OxyPlot.DataPoint(dataPoint.X, dataPoint.Y));

        plotModel.Series.Add(series);

        var plotView = new PlotView
        {
            Model = plotModel,
            Width = 400,
            Height = 300
        };

        using (var imageStream = new MemoryStream())
        {
            var exporter = new PngExporter { Width = 400, Height = 300 };
            exporter.Export(plotModel, imageStream);
            return imageStream;
        }
    }
}