using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CefGlue.Tests.Performance;

[TestFixture]
public class PerformanceTests : TestBase
{
    private BindingTestClass _dotNetObject;

    protected override async Task ExtraSetup()
    {
        _dotNetObject = new BindingTestClass();
        Browser.RegisterJavascriptObject(_dotNetObject, "dotNetObject");
        await Browser.LoadContent("<script></script>");
        await base.ExtraSetup();
    }

    [TestCase(1_000, 1.0d)]
    [TestCase(10_000, 2.0d)]
    [TestCase(100_000, 8.5d)]
    [TestCase(1_000_000, 65.0d)]
    public async Task PerformanceTest(int numberOfValues, double expectedValue)
    {
        var script = $$"""
                       var startTime = 0;
                       var endTime = 0;
                       var counter = 0;
                       var duration = 0;
                       var testArray = [];
                       function ExecutePerformanceTest() {
                            startTime = performance.now();
                            dotNetObject.getDataFromDotNetObject({{numberOfValues}}).then(array => {
                                endTime = performance.now();
                                duration = endTime - startTime;
                                if (array.length === {{numberOfValues}})
                                {
                                    testArray.push(duration);
                                }

                                startTime = 0;
                                endTime = 0;
                                duration = 0;
                                counter++;

                                if (counter < 100) {
                                    ExecutePerformanceTest();
                                }
                                else {
                                    dotNetObject.setResult(testArray);
                                }
                            });
                       }
                       """;

        Browser.ExecuteJavaScript($"{script}");
        Browser.ExecuteJavaScript("ExecutePerformanceTest();");
        var result = await _dotNetObject.ResultTask;

        double mean = (result is object[] objectArray) ? objectArray.OfType<double>().Sum() / 100.0 : double.MaxValue;
        Console.WriteLine($"{mean} (ms)");

        const double TolerancePercentage = 0.03; // 3% tolerance
        double tolerance = expectedValue * TolerancePercentage;

        Assert.Less(mean, expectedValue + tolerance, 
            $"The method took too long to execute. Mean: {mean}, Expected: {expectedValue} with {TolerancePercentage * 100}% tolerance.");
    }
}

public class BindingTestClass
{
    private static readonly double[] Values1000 = new double[1_000];
    private static readonly double[] Values10000 = new double[10_000];
    private static readonly double[] Values100000 = new double[100_000];
    private static readonly double[] Values1000000 = new double[1_000_000];
    private TaskCompletionSource<object> _tcs = new ();

    public BindingTestClass()
    {
        for (var j = 0; j < 1_000_000; j++)
        {
            var randomValue = Random.Shared.NextDouble() * Random.Shared.Next(0, 1000);
            if (j < 1_000)
            {
                Values1000[j] = randomValue;
            }

            if (j < 10_000)
            {
                Values10000[j] = randomValue;
            }

            if (j < 100_000)
            {
                Values100000[j] = randomValue;
            }

            Values1000000[j] = randomValue;
        }
    }

    public Task<object> ResultTask => _tcs.Task;

    public void SetResult(object result) => _tcs.SetResult(result);

    public void ResetTask() => _tcs = new TaskCompletionSource<object>();

    public double[] GetDataFromDotNetObject(int numberOfItems) => SimulateDataTransfer(numberOfItems);

    private double[] SimulateDataTransfer(int numberOfItems) =>
        numberOfItems switch
        {
            1_000 => Values1000,
            10_000 => Values10000,
            100_000 => Values100000,
            1_000_000 => Values1000000,
            _ => Array.Empty<double>()
        };
}