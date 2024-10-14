using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace CefGlue.Tests.Performance;

[TestFixture]
public class PerformanceTests : TestBase
{
    private BindingTestClass _dotNetObject;
    private BindingTestClass _dotNetObjectMsgPack;

    protected override async Task ExtraSetup()
    {
        _dotNetObject = new BindingTestClass();
        Browser.RegisterJavascriptObject(_dotNetObject, "dotNetObject", null, Messaging.Json);

        _dotNetObjectMsgPack = new BindingTestClass();
        Browser.RegisterJavascriptObject(_dotNetObjectMsgPack, "dotNetObjectMsgPack", null, Messaging.MsgPack);

        await Browser.LoadContent("<script></script>");
        await base.ExtraSetup();
    }

    [TestCase(1_000, 1.25)]
    [TestCase(10_000, 7.0)]
    [TestCase(100_000, 9.0)]
    [TestCase(250_000, 10.0)]
    [TestCase(1_000_000, 10.0)]
    public async Task ComparePerformanceJsonVsMsgPack(int numberOfValues, double expectedResult)
    {
        // Perform json performance test
        Stopwatch stopwatchJson = Stopwatch.StartNew();
        await PerformanceTestJson(numberOfValues);
        stopwatchJson.Stop();
        double jsonTime = stopwatchJson.ElapsedMilliseconds;

        // Perform msgpack performance test
        Stopwatch stopwatchMsgPack = Stopwatch.StartNew();
        await PerformanceTestMsgPack(numberOfValues);
        stopwatchMsgPack.Stop();
        double msgPackTime = stopwatchMsgPack.ElapsedMilliseconds;

        double difference = jsonTime / msgPackTime;

        TestContext.WriteLine($"Number of values: {numberOfValues}");
        TestContext.WriteLine($"Json time: {jsonTime} ms");
        TestContext.WriteLine($"MsgPack time: {msgPackTime} ms");
        TestContext.WriteLine($"MsgPack is {difference} faster than Json.");

        Assert.IsTrue(difference >= expectedResult, $"MsgPack should be about {expectedResult} times faster.");
    }

    public async Task PerformanceTestMsgPack(int numberOfValues)
    {
        var script = $$"""
                       var startTime = 0;
                       var endTime = 0;
                       var counter = 0;
                       var duration = 0;
                       var testArray = [];
                       function ExecutePerformanceTest() {
                            startTime = performance.now();
                            dotNetObjectMsgPack.getDataFromDotNetObject({{numberOfValues}}).then(array => {
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
                                    dotNetObjectMsgPack.setResult(testArray);
                                }
                            });
                       }
                       """;

        Browser.ExecuteJavaScript($"{script}");
        Browser.ExecuteJavaScript("ExecutePerformanceTest();");
        await _dotNetObjectMsgPack.ResultTask;
    }

    public async Task PerformanceTestJson(int numberOfValues)
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
        await _dotNetObject.ResultTask;
    }
}

public class BindingTestClass
{
    private static readonly double[] Values1000 = new double[1_000];
    private static readonly double[] Values10000 = new double[10_000];
    private static readonly double[] Values100000 = new double[100_000];
    private static readonly double[] Values250000 = new double[250_000];
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

            if (j < 250_000)
            {
                Values250000[j] = randomValue;
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
            250_000 => Values250000,
            1_000_000 => Values1000000,
            _ => Array.Empty<double>()
        };
}