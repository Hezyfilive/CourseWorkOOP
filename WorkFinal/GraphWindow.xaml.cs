using System.Collections.Generic;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WorkFinal;

public partial class GraphWindow : Window
{
    public GraphWindow(List<Polynom.DataPoint> dataPoints)
    {
        InitializeComponent();
        var plotModel = new PlotModel();

        var series = new LineSeries();
        foreach (var dataPoint in dataPoints) series.Points.Add(new DataPoint(dataPoint.X, dataPoint.Y));

        plotModel.Series.Add(series);

        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

        interpolationPlot.Model = plotModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}