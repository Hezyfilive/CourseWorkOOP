using System.Collections.Generic;
using System.IO;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using DataPoint = Polynom.DataPoint;

namespace WorkFinal;

public interface IDocResult
{
    public void DocResult(double result, string outPath);
}