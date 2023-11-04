using Polynom;

internal abstract class Program
{
    private static void Main()
    {
        var polynomialInterpolation = new PolynomialInterpolation();

        Console.WriteLine("Enter the number of data points:");
        var numDataPoints = int.Parse(Console.ReadLine());

        for (var i = 0; i < numDataPoints; i++)
        {
            Console.Write($"Enter data point {i + 1} (X Y): ");
            string[] data = Console.ReadLine().Split(' ');
            var x = double.Parse(data[0]);
            var y = double.Parse(data[1]);
            polynomialInterpolation.AddDataPoint(new DataPoint { X = x, Y = y });
        }

        Console.Write("Enter x value for Lagrange Interpolation: ");
        var xValue = double.Parse(Console.ReadLine());

        var interpolation = polynomialInterpolation.LagrangeInterpolation(xValue);
        Console.WriteLine($"Lagrange Interpolation Result at x={xValue}: {interpolation}");

        polynomialInterpolation.SaveToXml("test.xml");
        Console.WriteLine("Data points saved to test.xml.");

        var loadedInterpolation = PolynomialInterpolation.LoadFromXml("test.xml");
        Console.WriteLine("Data points loaded from test.xml:");

        foreach (var dataPoint in loadedInterpolation.DataPoints)
            Console.WriteLine($"X: {dataPoint.X}, Y: {dataPoint.Y}");

        Console.WriteLine("Specify the degree n of the polynomial f(x):");
        var degree = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the eps:");
        var eps = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter max iteration value:");
        var maxIteration = int.Parse(Console.ReadLine());

        var results = polynomialInterpolation.FindRoots(degree, eps, maxIteration);
        foreach (var result in results) Console.WriteLine($"Root found within x: {result}");
    }
}