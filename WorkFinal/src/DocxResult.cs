using System.Collections.Generic;
using System.IO;
using NPOI.Util;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using DataPoint = Polynom.DataPoint;
using NPOI.XWPF.UserModel;
using OxyPlot.Axes;

namespace WorkFinal;

public class DocxResult : IDocResult
{
    private readonly DataGrid _dataGrid = DataGrid.GetInstance();


    public void DocResult(List<double> results, string outPath, int degree, double eps, double minValue,
        double maxValue, double step)
    {
        using (var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
        {
            var doc = new XWPFDocument();

            CreateTitle(doc);
            CreateDataPointsSection(doc);
            CreateGraphSection(doc);
            CreateDegreeSection(doc, degree);
            CreateEpsilonSection(doc, eps);
            CreateMaxIterationsSection(doc, minValue, maxValue, step);
            CreateRootFindingResultsSection(doc, results);

            doc.Write(fs);
        }
    }

    private void CreateTitle(XWPFDocument doc)
    {
        var title = doc.CreateParagraph();
        title.Alignment = ParagraphAlignment.CENTER;
        var titleRun = title.CreateRun();
        titleRun.SetText("Report");
    }

    private void CreateDataPointsSection(XWPFDocument doc)
    {
        var dataPointsParagraph = doc.CreateParagraph();
        dataPointsParagraph.Alignment = ParagraphAlignment.LEFT;
        var dataPointsRun = dataPointsParagraph.CreateRun();
        dataPointsRun.SetText("Point coordinates:");

        foreach (var dataPoint in _dataGrid.GetCollection())
        {
            var pointInfo = doc.CreateParagraph();
            pointInfo.Alignment = ParagraphAlignment.LEFT;
            var pointRun = pointInfo.CreateRun();
            pointRun.SetText($"X: {dataPoint.X}, Y: {dataPoint.Y}");
        }
    }

    private void CreateGraphSection(XWPFDocument doc)
    {
        using (var imageStream = GenerateGraphImage(_dataGrid.GetCollection()))
        {
            imageStream.Position = 0;
            var graphParagraph = doc.CreateParagraph();
            graphParagraph.Alignment = ParagraphAlignment.LEFT;

            var graphRun = graphParagraph.CreateRun();
            graphRun.AddPicture(imageStream, (int)PictureType.PNG, "Graph", Units.ToEMU(400),
                Units.ToEMU(300));
        }
    }

    private void CreateDegreeSection(XWPFDocument doc, int degree)
    {
        var paragraph1 = doc.CreateParagraph();
        paragraph1.Alignment = ParagraphAlignment.LEFT;

        var paragraphRun1 = paragraph1.CreateRun();
        paragraphRun1.SetText($"Degree used: {degree}");
    }

    private void CreateEpsilonSection(XWPFDocument doc, double eps)
    {
        var paragraph = doc.CreateParagraph();
        paragraph.Alignment = ParagraphAlignment.LEFT;
        var paragraphRun = paragraph.CreateRun();
        paragraphRun.SetText($"Eps used: {eps}");
    }

    private void CreateMaxIterationsSection(XWPFDocument doc, double minValue, double maxValue, double step)
    {
        var xwpfParagraph = doc.CreateParagraph();
        xwpfParagraph.Alignment = ParagraphAlignment.LEFT;
        var xwpfParagraphRun = xwpfParagraph.CreateRun();
        xwpfParagraphRun.SetText($" MinValue: {minValue}, MaxValue: {maxValue}, Step: {step}");
    }

    private void CreateRootFindingResultsSection(XWPFDocument doc, List<double> results)
    {
        if (results.Count > 0)
        {
            var calculationResult = doc.CreateParagraph();
            calculationResult.Alignment = ParagraphAlignment.LEFT;
            var calculationRun = calculationResult.CreateRun();
            calculationRun.SetText("Root finding results:");

            foreach (var result in results)
            {
                var pointInfo = doc.CreateParagraph();
                pointInfo.Alignment = ParagraphAlignment.LEFT;
                var pointRun = pointInfo.CreateRun();
                pointRun.SetText($"X: {result}");
            }

            using (var imageStream = GenerateGraphImage(results))
            {
                imageStream.Position = 0;
                var graphParagraph = doc.CreateParagraph();
                graphParagraph.Alignment = ParagraphAlignment.LEFT;

                var graphRun = graphParagraph.CreateRun();
                var picture = graphRun.AddPicture(imageStream, (int)PictureType.PNG, "Graph", Units.ToEMU(400),
                    Units.ToEMU(300));
            }
        }
        else
        {
            var calculationResult = doc.CreateParagraph();
            calculationResult.Alignment = ParagraphAlignment.LEFT;
            var calculationRun = calculationResult.CreateRun();
            calculationRun.SetText("No roots are found.");
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

        var imageStream = new MemoryStream();

        var exporter = new PngExporter { Width = 400, Height = 300 };
        exporter.Export(plotModel, imageStream);

        imageStream.Position = 0;

        return imageStream;
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

        var imageStream = new MemoryStream();

        var exporter = new PngExporter { Width = 400, Height = 300 };
        exporter.Export(plotModel, imageStream);

        imageStream.Position = 0;

        return imageStream;
    }
}