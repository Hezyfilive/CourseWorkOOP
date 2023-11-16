using System.Collections.Generic;

namespace WorkFinal;

public interface IDocResult
{
    public void DocResult(List<double> results, string outPath, int degree, double eps, double minValue,
        double maxValue, double step);
}