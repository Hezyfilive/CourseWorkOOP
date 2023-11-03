using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Polynom;

namespace WorkFinal;

public class DataGrid
{
    private static DataGrid? _instance = null;
    private PolynomialInterpolation _polynomialInterpolation;

    public event EventHandler<PolynomialInterpolation>? GridEvent;

    public DataGrid()
    {
        _polynomialInterpolation = new PolynomialInterpolation();
    }

    public DataGrid(PolynomialInterpolation polynomialInterpolation)
    {
        _polynomialInterpolation = polynomialInterpolation;
    }

    public PolynomialInterpolation GetPolynomialInterpolation()
    {
        return _polynomialInterpolation;
    }

    public void SaveGrid(string path)
    {
        try
        {
            _polynomialInterpolation.SaveToXml(path);
        }
        finally
        {
            _polynomialInterpolation = new PolynomialInterpolation();
            GridEvent?.Invoke(this, _polynomialInterpolation);
        }
    }

    public void LoadGrid(string path)
    {
        try
        {
            _polynomialInterpolation = PolynomialInterpolation.LoadFromXml(path);
        }
        finally
        {
            GridEvent?.Invoke(this, _polynomialInterpolation);
        }
    }

    public void UpdateGrid(DataPoint point, int index)
    {
        _polynomialInterpolation.UpdateDataPoint(point, index);
        GridEvent?.Invoke(this, _polynomialInterpolation);
    }

    public void AddPoint()
    {
        _polynomialInterpolation.AddDataPoint(new DataPoint { X = 0, Y = 0 });
        GridEvent?.Invoke(this, _polynomialInterpolation);
    }

    public void AddPoint(DataPoint point)
    {
        _polynomialInterpolation.AddDataPoint(point);
        GridEvent?.Invoke(this, _polynomialInterpolation);
    }

    public void RemovePoint(int index)
    {
        _polynomialInterpolation.RemoveDataPoint(index);
        GridEvent?.Invoke(this, _polynomialInterpolation);
    }

    public List<DataPoint> GetCollection()
    {
        return _polynomialInterpolation.DataPoints;
    }

    public static DataGrid GetInstance()
    {
        if (_instance == null) _instance = new DataGrid();

        return _instance;
    }
}