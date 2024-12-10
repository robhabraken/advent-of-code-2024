void measure()
{
    var stopwatch = new Stopwatch();

    var measurements = new List<double>();
    for (var i = 0; i < 100; i++)
    {
        stopwatch.Start();
        solve();
        stopwatch.Stop();
        measurements.Add(stopwatch.Elapsed.TotalMilliseconds);
        stopwatch.Reset();
    }

    measurements.Sort();
    measurements.RemoveAt(0);
    measurements.RemoveAt(measurements.Count - 1);

    Console.WriteLine($"Trimmed mean time to find the solution: {Math.Round(measurements.Average(), 4)} ms");
}

void solve()
{

}