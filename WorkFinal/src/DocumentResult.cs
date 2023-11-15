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


    public void DocResult(List<double> results, string outPath, int degree, double eps, double minValue, double maxValue, double step)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var document = new Document())
            {
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                AddTitle(document);
                AddDataPoints(document);
                AddGraphImage(document);
                AddParameters(document, degree, eps, minValue, maxValue, step);
                AddResultInfo(document, results);

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

    private void AddParameters(Document document, int degree, double eps, double minValue, double maxValue, double step)
    {
        var degreeParagraph  = new Paragraph($"Degree used: {degree}");
        document.Add(degreeParagraph );

        var epsParagraph  = new Paragraph($"Epsilon used: {eps}");
        document.Add(epsParagraph );

        var iterationsParagraph = new Paragraph($"MinValue: {minValue}, MaxValue: {maxValue}, Step: {step}");
        document.Add(iterationsParagraph);
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