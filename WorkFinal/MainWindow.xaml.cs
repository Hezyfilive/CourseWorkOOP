using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using Polynom;

namespace WorkFinal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly DataGrid _dataGrid = DataGrid.GetInstance();

    public MainWindow()
    {
        InitializeComponent();
        _dataGrid.GridEvent += OnGridDataChange;
        _dataGrid.SettingsEvent += OnSettingsLoad;
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filePath = openFileDialog.FileName;
            _dataGrid.LoadGrid(filePath);
        }
    }

    private void ShowChart_Click(object sender, RoutedEventArgs e)
    {
        var graphWindow = new GraphWindow(_dataGrid.GetCollection());
    }

    private void CalculateResult_Click(object sender, RoutedEventArgs e)
    {
        GenerateResult(generateFile: false);
    }


    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
            try
            {
                var filePath = saveFileDialog.FileName;
                _dataGrid.SaveGrid(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
    }

    private void PdfResult_Click(object sender, RoutedEventArgs e)
    {
        GenerateResult(pdfResult: true);
    }

    private void DocxResult_Click(object sender, RoutedEventArgs e)
    {
        GenerateResult();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataPointGrid.SelectedItem is DataPoint dataPoint)
        {
            var index = _dataGrid.GetCollection().IndexOf(dataPoint);

            var copiedPoint = new DataPoint { X = dataPoint.X, Y = dataPoint.Y };

            var editDataPointWindow = new EditDataPointWindow(copiedPoint);

            if (editDataPointWindow.ShowDialog() == true)
            {
                var editedDataPoint = editDataPointWindow.DataPoint;
                _dataGrid.UpdateGrid(editedDataPoint, index);
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataPointGrid.SelectedItem is DataPoint dataPoint)
        {
            var index = _dataGrid.GetCollection().IndexOf(dataPoint);
            _dataGrid.RemovePoint(index);
        }
    }

    private void AddPoint_Click(object sender, RoutedEventArgs e)
    {
        var editDataPointWindow = new EditDataPointWindow();
        if (editDataPointWindow.ShowDialog() == true)
        {
            var newDataPoint = editDataPointWindow.DataPoint;
            _dataGrid.AddPoint(newDataPoint);
        }
    }

    private void GenerateResult(bool pdfResult = false, bool generateFile = true)
    {
        var degreeText = DegreeSelect.Text;

        if (int.TryParse(degreeText, out var degree) &&
            double.TryParse(TxtSearch.Text, out var parsedEps) &&
            double.TryParse(MinValueText.Text, out var parsedMinValue) &&
            double.TryParse(MaxValueText.Text, out var parsedMaxValue) &&
            double.TryParse(StepText.Text, out var parsedStep))
        {
            List<double> result = new List<double>();
            try
            {
                result = GetResult(degree, parsedEps, parsedMinValue, parsedMaxValue, parsedStep);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (generateFile)
                {
                    var saveFileDialog = new SaveFileDialog()
                    {
                        Filter = pdfResult
                            ? "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
                            : "Word Files (*.docx)|*.docx|All Files (*.*)|*.*"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var filePath = saveFileDialog.FileName;

                        if (pdfResult)
                        {
                            GenerateResultDoc(new DocumentResult(), result, filePath, degree, parsedEps, parsedMinValue, parsedMaxValue, parsedStep);
                        }
                        else
                        {
                            GenerateResultDoc(new DocxResult(), result, filePath, degree, parsedEps, parsedMinValue, parsedMaxValue, parsedStep);
                        }
                    }
                }
                else
                {
                    var graphWindow = new GraphWindow(result);
                }
            }
        }
        else
        {
            MessageBox.Show("Invalid double value entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void GenerateResultDoc(IDocResult docResult,List<double> result, string filePath, int degree, double eps, double minValue, double maxValue, double step)
    {
        docResult.DocResult(result, filePath, degree, eps, minValue, maxValue, step);
    }

    private void OnGridDataChange(object? sender, PolynomialInterpolation interpolation)
    {
        var dataPoints = interpolation.DataPoints;
        ObservableCollection<DataPoint> collection = new(dataPoints);
        DataPointGrid.ItemsSource = collection;
    }
    private List<double> GetResult(int degree, double eps, double minValue, double maxValue, double step)
    { 
        var interpolation = _dataGrid.GetPolynomialInterpolation();
        
        var setting = new InterpolationSettings
        {
            Degree = degree,
            Epsilon = eps,
            MinValue = minValue,
            MaxValue = maxValue,
            Step = step
        };
        
        return interpolation.FindRoots(setting);
    }

    private void OnSettingsLoad(object? sender, InterpolationSettings settings)
    {
        DegreeSelect.Text = settings.Degree.ToString();
        Console.WriteLine(DegreeSelect.Text);
        TxtSearch.Text = settings.Epsilon.ToString(CultureInfo.CurrentCulture);
        MinValueText.Text = settings.MinValue.ToString(CultureInfo.CurrentCulture);
        MaxValueText.Text = settings.MaxValue.ToString(CultureInfo.CurrentCulture);
        StepText.Text = settings.Step.ToString(CultureInfo.CurrentCulture);
    }
}