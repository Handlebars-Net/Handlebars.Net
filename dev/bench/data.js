window.BENCHMARK_DATA = {
  "lastUpdate": 1602361014110,
  "repoUrl": "https://github.com/Handlebars-Net/Handlebars.Net",
  "entries": {
    "Benchmark.Net Benchmark": [
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "distinct": true,
          "id": "06a7c2f61fe876024060043b6e5db0045d4a0cb4",
          "message": "Fix CI flow",
          "timestamp": "2020-10-10T23:11:17+03:00",
          "tree_id": "62fcb2381e90720e0a477584054cb0227a642b11",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/06a7c2f61fe876024060043b6e5db0045d4a0cb4"
        },
        "date": 1602361013423,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 38094313.87912088,
            "unit": "ns",
            "range": "± 988604.685991903"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 111.80140233039856,
            "unit": "ns",
            "range": "± 2.0101465280550364"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 123.24848914146423,
            "unit": "ns",
            "range": "± 3.416504309553798"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 144.14054804581863,
            "unit": "ns",
            "range": "± 1.7928230874804232"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 645.4823929786683,
            "unit": "ns",
            "range": "± 31.454346614199324"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 331.9645745754242,
            "unit": "ns",
            "range": "± 7.792124989761809"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 358.569322347641,
            "unit": "ns",
            "range": "± 7.811808432307189"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 2002.5345984141031,
            "unit": "ns",
            "range": "± 34.701871899428575"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 1983.610055287679,
            "unit": "ns",
            "range": "± 47.44529123178334"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 2245.8239687601726,
            "unit": "ns",
            "range": "± 51.75968128967539"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 3874438.269230769,
            "unit": "ns",
            "range": "± 39584.2726897274"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 3860625.307198661,
            "unit": "ns",
            "range": "± 115303.22133554166"
          }
        ]
      }
    ]
  }
}