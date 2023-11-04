using System.Collections.Generic;

namespace WorkFinal;

public interface IDocResult
{
    public void DocResult(List<double> result, string outPath, int degree, double eps, int iterations);
}