using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using DataPoint = Polynom.DataPoint;
using Element = iTextSharp.text.Element;

namespace WorkFinal;

public class DocumentResult : IDocResult
{
    private readonly DataGrid _dataGrid = DataGrid.GetInstance();


    public void DocResult(List<double> result, string outPath, int degree)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var document = new Document())
            {
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                AddTitle(document);
                AddDataPoints(document);
                AddGraphImage(document);
                AddParameters(document, degree);
                AddResultInfo(document, result);

                document.Close();
            }

            File.WriteAllBytes(outPath, memoryStream.ToArray());
        }
    }

    private void AddTitle(Document document)
    {
        var title = new Paragraph("Report");
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
    }

    private void AddDataPoints(Document document)
    {
        var dataPointsParagraph = new Paragraph("Point coordinates:");
        document.Add(dataPointsParagraph);

        foreach (var dataPoint in _dataGrid.GetCollection())
        {
            var pointInfo = new Paragraph($"X: {dataPoint.X}, Y: {dataPoint.Y}");
            document.Add(pointInfo);
        }
    }

    private void AddGraphImage(Document document)
    {
        var graphText = new Paragraph("g(x) dots graph:");
        document.Add(graphText);
        var graphImageStream = GenerateGraphImage(_dataGrid.GetCollection());

        var graphImage = Image.GetInstance(graphImageStream.ToArray());
        document.Add(graphImage);
    }

    private void AddParameters(Document document, int degree)
    {
        var parag = new Paragraph($"Degree used: {degree}");
        document.Add(parag);

        var eParagraph = new Paragraph($"Epsilon used: {1e-6}");
        document.Add(eParagraph);

        var paragraph = new Paragraph($"Max iterations: {100}");
        document.Add(paragraph);
    }

    private void AddResultInfo(Document document, List<double> result)
    {
        if (result.Count > 0)
        {
            var calculationResult = new Paragraph("Root finding results:");
            document.Add(calculationResult);

            foreach (var res in result)
            {
                var txt = new Paragraph($"X: {res}");
                document.Add(txt);
            }

            var calculationResultGr = new Paragraph("Root finding results graph:");
            document.Add(calculationResultGr);

            var graphResultImageStream = GenerateGraphImage(result);
            var graphResultImage = Image.GetInstance(graphResultImageStream.ToArray());

            document.Add(graphResultImage);
        }
        else
        {
            var calculationResult = new Paragraph("No roots are found.");
            document.Add(calculationResult);
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

    private MemoryStream GenerateGraphImage(List<double> roots)
    {
        var plotModel = new PlotModel();
        var rootSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 5,
            MarkerFill = OxyColors.Red
        };

        foreach (var root in roots) rootSeries.Points.Add(new ScatterPoint(root, 0));

        plotModel.Series.Add(rootSeries);

        var xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "X" };

        plotModel.Axes.Add(xAxis);

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