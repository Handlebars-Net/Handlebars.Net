window.BENCHMARK_DATA = {
  "lastUpdate": 1640213958309,
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
      },
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
          "id": "01ef770e6843d6779fbd6f3a7b700a6bf677bbe9",
          "message": "Update readme with CI/Sonar info",
          "timestamp": "2020-10-11T20:01:59+03:00",
          "tree_id": "0d58d05bcbeb7d049204ab484a9705b703014d56",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/01ef770e6843d6779fbd6f3a7b700a6bf677bbe9"
        },
        "date": 1602436035124,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 44911432.87777778,
            "unit": "ns",
            "range": "± 2111707.8229665207"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 130.25045762062072,
            "unit": "ns",
            "range": "± 8.27294590189249"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 127.30757771219525,
            "unit": "ns",
            "range": "± 4.932222737379302"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 169.18157666524252,
            "unit": "ns",
            "range": "± 10.564743971908127"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 705.1620930989583,
            "unit": "ns",
            "range": "± 24.45441053526425"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 390.51982107162473,
            "unit": "ns",
            "range": "± 18.18713143375078"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 417.1917410850525,
            "unit": "ns",
            "range": "± 15.708929864034964"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 2221.6751643589564,
            "unit": "ns",
            "range": "± 66.20257217740578"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 2244.7922439575195,
            "unit": "ns",
            "range": "± 51.438884645987635"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 2173.730542755127,
            "unit": "ns",
            "range": "± 94.81969256205772"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 4160659.4972098214,
            "unit": "ns",
            "range": "± 116483.94963266149"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 4308724.955729167,
            "unit": "ns",
            "range": "± 182842.19893818576"
          }
        ]
      },
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
          "id": "f022ffc8c732c517f6f903d6590845691757211d",
          "message": "Update readme with CI/Sonar info",
          "timestamp": "2020-10-11T20:02:51+03:00",
          "tree_id": "acf30056d3508e7a6e093d363e72e5f34cf8c586",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/f022ffc8c732c517f6f903d6590845691757211d"
        },
        "date": 1602436120241,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 40804369.41666667,
            "unit": "ns",
            "range": "± 2602924.0087673147"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 113.7993238247358,
            "unit": "ns",
            "range": "± 3.347622495870392"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 124.92140010197957,
            "unit": "ns",
            "range": "± 8.830737597310188"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 158.6599274555842,
            "unit": "ns",
            "range": "± 7.2979234786732174"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 697.0572381337483,
            "unit": "ns",
            "range": "± 36.24614864536951"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 352.74170855113437,
            "unit": "ns",
            "range": "± 7.954253028397034"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 381.711395910808,
            "unit": "ns",
            "range": "± 10.575453550421964"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 2064.881165313721,
            "unit": "ns",
            "range": "± 123.77117323832637"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 1990.4557931082588,
            "unit": "ns",
            "range": "± 98.64030234598182"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 2279.1798769632974,
            "unit": "ns",
            "range": "± 114.73531419621355"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 4077116.7119791666,
            "unit": "ns",
            "range": "± 140014.32700488254"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 4180246.35625,
            "unit": "ns",
            "range": "± 276646.1472270508"
          }
        ]
      },
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
          "id": "a939d2fe61fe1a0a70e654a5869389a12c629e5c",
          "message": "Update readme with CI/Sonar info",
          "timestamp": "2020-10-11T20:04:12+03:00",
          "tree_id": "fdf84934e8452a532c667a2c4f082e3ceb23f47d",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/a939d2fe61fe1a0a70e654a5869389a12c629e5c"
        },
        "date": 1602436205796,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 37118763.79591836,
            "unit": "ns",
            "range": "± 515449.75404867914"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 106.65371568202973,
            "unit": "ns",
            "range": "± 1.1289635063276644"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 110.67338307698567,
            "unit": "ns",
            "range": "± 1.256084426869548"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 135.90883574883142,
            "unit": "ns",
            "range": "± 2.0203653095727074"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 586.8717856725057,
            "unit": "ns",
            "range": "± 10.26986619312994"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 320.95129915873207,
            "unit": "ns",
            "range": "± 5.241040708311511"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 335.15519295419966,
            "unit": "ns",
            "range": "± 3.1316305560372943"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 1757.17291532244,
            "unit": "ns",
            "range": "± 16.205919592472394"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 1802.3412659962971,
            "unit": "ns",
            "range": "± 30.03640421738016"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 1874.3060961405436,
            "unit": "ns",
            "range": "± 44.494029347397266"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 3528533.53984375,
            "unit": "ns",
            "range": "± 85787.68391815248"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 3618532.3546316964,
            "unit": "ns",
            "range": "± 36288.92254145676"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "323f4e5dd79fdfc38dc719b92eb08d99b5ba7f34",
          "message": "Merge pull request #359 from zjklee/master\n\nMerging fork into trunk",
          "timestamp": "2020-10-12T19:56:07+03:00",
          "tree_id": "10d48739d8169aca0856e5ccf06546493e922733",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/323f4e5dd79fdfc38dc719b92eb08d99b5ba7f34"
        },
        "date": 1602522083169,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 29379235.879166666,
            "unit": "ns",
            "range": "± 134482.92648796353"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 216.31903753961836,
            "unit": "ns",
            "range": "± 0.19764076777251852"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 235.75859769185385,
            "unit": "ns",
            "range": "± 0.1767348821859345"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 263.02979839765106,
            "unit": "ns",
            "range": "± 1.497327239665997"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 442.8253549337387,
            "unit": "ns",
            "range": "± 0.536525802234178"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 461.9604452337538,
            "unit": "ns",
            "range": "± 0.46792628386433666"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 499.6754415218647,
            "unit": "ns",
            "range": "± 0.8862103659667435"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 972.3469884055002,
            "unit": "ns",
            "range": "± 1.1127504199727698"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 980.8573020935058,
            "unit": "ns",
            "range": "± 3.9678847632194008"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 1013.1303179814265,
            "unit": "ns",
            "range": "± 2.4144359844213223"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 664496.8155048077,
            "unit": "ns",
            "range": "± 491.3268415421587"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 719144.0558035715,
            "unit": "ns",
            "range": "± 673.0116375834513"
          }
        ]
      },
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
          "id": "94197f44a9d446590eac780c327fbd371d18cee2",
          "message": "Disable benchmark failure on perf alert",
          "timestamp": "2020-10-12T20:07:31+03:00",
          "tree_id": "5fc93f126074ef061f8a666bec1d339004746339",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/94197f44a9d446590eac780c327fbd371d18cee2"
        },
        "date": 1602522827492,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 29109043.385416668,
            "unit": "ns",
            "range": "± 983081.4670666108"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 181.98750316301982,
            "unit": "ns",
            "range": "± 9.597813977536664"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 190.04774635632833,
            "unit": "ns",
            "range": "± 10.210988687531172"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 211.96112274328868,
            "unit": "ns",
            "range": "± 11.03150319879609"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 388.04920400891984,
            "unit": "ns",
            "range": "± 13.81765886085141"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 400.36630984942116,
            "unit": "ns",
            "range": "± 21.760113094617342"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 425.2765923500061,
            "unit": "ns",
            "range": "± 22.082496742726626"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 786.7405904134115,
            "unit": "ns",
            "range": "± 35.912005376594614"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 856.4691944122314,
            "unit": "ns",
            "range": "± 41.877452322184624"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 865.4222745895386,
            "unit": "ns",
            "range": "± 48.29804578313845"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 616384.9518880208,
            "unit": "ns",
            "range": "± 28846.44600256716"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 689526.9905598959,
            "unit": "ns",
            "range": "± 52685.59171396467"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b72cd8bad194c7a42d60d4df3c332e72372ad251",
          "message": "Merge pull request #371 from zjklee/merging/late-binding-improvement\n\nInfrastructure improvements",
          "timestamp": "2020-10-18T00:05:49+03:00",
          "tree_id": "7f2f7de4e870e109d64f126ef4b688c6fe8f71db",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/b72cd8bad194c7a42d60d4df3c332e72372ad251"
        },
        "date": 1602969073226,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20056724.420833334,
            "unit": "ns",
            "range": "± 509140.8268425644"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 324.4735985120138,
            "unit": "ns",
            "range": "± 9.576027282125526"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 319.6597719192505,
            "unit": "ns",
            "range": "± 7.938042151166329"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 346.2416379610697,
            "unit": "ns",
            "range": "± 8.2258841066088"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 423.6806919415792,
            "unit": "ns",
            "range": "± 10.87813235162832"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 328.0120960871379,
            "unit": "ns",
            "range": "± 11.388298933065512"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 361.0909435749054,
            "unit": "ns",
            "range": "± 13.168643024171637"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 689.2495915095011,
            "unit": "ns",
            "range": "± 12.883260266941784"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 689.7415002822876,
            "unit": "ns",
            "range": "± 15.478601454014107"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 737.3946743647258,
            "unit": "ns",
            "range": "± 24.107416228824807"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 246580.7543457031,
            "unit": "ns",
            "range": "± 8283.748259986047"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 258592.634375,
            "unit": "ns",
            "range": "± 9249.212626295168"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ec841d1c8a53548a3b8ec011eb9a5449c4544586",
          "message": "Merge pull request #376 from zjklee/feature/nuget-impr\n\nEnable CI build, add SourceLink",
          "timestamp": "2020-10-26T16:21:19+02:00",
          "tree_id": "f47a0efb0aef0854e350186f4d519d7c821c2e0e",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/ec841d1c8a53548a3b8ec011eb9a5449c4544586"
        },
        "date": 1603722401137,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19088174.595833335,
            "unit": "ns",
            "range": "± 243671.9057016972"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 308.9198212623596,
            "unit": "ns",
            "range": "± 3.7027354827757937"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 320.40050509997775,
            "unit": "ns",
            "range": "± 6.3023088105791265"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 362.47619773546853,
            "unit": "ns",
            "range": "± 5.988276022793533"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 416.12187836964927,
            "unit": "ns",
            "range": "± 8.816345095756885"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 326.2408130009969,
            "unit": "ns",
            "range": "± 5.316395037388925"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 347.47285102208457,
            "unit": "ns",
            "range": "± 5.513120970666432"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 670.5265794754029,
            "unit": "ns",
            "range": "± 15.93880332718411"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 686.3951638085501,
            "unit": "ns",
            "range": "± 7.931732950280785"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 755.470709323883,
            "unit": "ns",
            "range": "± 6.077093598617403"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 247534.29649135045,
            "unit": "ns",
            "range": "± 3515.2184140596873"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 268289.3352748326,
            "unit": "ns",
            "range": "± 2584.144495820786"
          }
        ]
      },
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
          "id": "29bc70852bcf870a4d328111a822aa8eaf50e5a5",
          "message": "Update .NET SDK version in CI",
          "timestamp": "2020-10-26T17:30:58+02:00",
          "tree_id": "3898925d0ca05194c5e1bb6c28b096d7e915cea7",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/29bc70852bcf870a4d328111a822aa8eaf50e5a5"
        },
        "date": 1603726599462,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 21042357.335416667,
            "unit": "ns",
            "range": "± 531085.2827676884"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 350.5379079500834,
            "unit": "ns",
            "range": "± 4.819909124757851"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 365.12415736062184,
            "unit": "ns",
            "range": "± 4.220984290495963"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 393.4304687182109,
            "unit": "ns",
            "range": "± 4.17637149551333"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 465.8464023590088,
            "unit": "ns",
            "range": "± 4.454888298978081"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 369.1967822313309,
            "unit": "ns",
            "range": "± 3.428673870722887"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 404.98468720118206,
            "unit": "ns",
            "range": "± 4.551448831184287"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 730.4528118133545,
            "unit": "ns",
            "range": "± 8.60131318572114"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 770.2660761560712,
            "unit": "ns",
            "range": "± 5.898472296932446"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 794.6733649798801,
            "unit": "ns",
            "range": "± 9.451069360244324"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 274905.81508091517,
            "unit": "ns",
            "range": "± 4966.648961771271"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 301672.7965611049,
            "unit": "ns",
            "range": "± 4116.886871330797"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "aceab78b3917fc291cbbde2c00602d0a03286c87",
          "message": "Update README.md\n\nDisplay `preview` package badge",
          "timestamp": "2020-10-26T17:38:54+02:00",
          "tree_id": "96035cc757fc7fcf6df9c3bda5222dd848e15298",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/aceab78b3917fc291cbbde2c00602d0a03286c87"
        },
        "date": 1603727063572,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 21122480.203125,
            "unit": "ns",
            "range": "± 407838.7859384311"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 332.4172096888224,
            "unit": "ns",
            "range": "± 7.003491880986063"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 356.9874269803365,
            "unit": "ns",
            "range": "± 5.343062575554579"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 386.17273708752225,
            "unit": "ns",
            "range": "± 4.641692266334682"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 445.0792442651895,
            "unit": "ns",
            "range": "± 4.6234884533718255"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 345.7754635810852,
            "unit": "ns",
            "range": "± 3.7649063073079234"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 378.59244625908985,
            "unit": "ns",
            "range": "± 3.4351829904176068"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 711.4379467964172,
            "unit": "ns",
            "range": "± 6.962606173478833"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 730.7679719289143,
            "unit": "ns",
            "range": "± 16.48717987761844"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 767.8266609632052,
            "unit": "ns",
            "range": "± 7.546792100766504"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 259621.69225260417,
            "unit": "ns",
            "range": "± 3104.4736589791405"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 284243.1380208333,
            "unit": "ns",
            "range": "± 4492.309673150321"
          }
        ]
      },
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
          "id": "cc33fce9d25e66bfc1169c53dade976c8416a7ab",
          "message": "Move `ContinuousIntegrationBuild` to workflow",
          "timestamp": "2020-10-26T18:54:37+02:00",
          "tree_id": "cd372f308ddf8d9ba786d70620b485add0aeae36",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/cc33fce9d25e66bfc1169c53dade976c8416a7ab"
        },
        "date": 1603731660627,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25231727.317307692,
            "unit": "ns",
            "range": "± 444494.8351145859"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 388.04394027392067,
            "unit": "ns",
            "range": "± 10.852916166415138"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 395.45101305643715,
            "unit": "ns",
            "range": "± 7.599001867889658"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 437.88441228866577,
            "unit": "ns",
            "range": "± 7.945919568325183"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 514.8643965039935,
            "unit": "ns",
            "range": "± 10.082059681490277"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 398.74636462529503,
            "unit": "ns",
            "range": "± 10.404071952496812"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 437.7166626612345,
            "unit": "ns",
            "range": "± 14.50638468969605"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 826.6779331843059,
            "unit": "ns",
            "range": "± 19.089578257607986"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 876.7336546352932,
            "unit": "ns",
            "range": "± 25.14226492505866"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 918.8457656860352,
            "unit": "ns",
            "range": "± 35.720133136161024"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 297505.7770298549,
            "unit": "ns",
            "range": "± 6822.089121432665"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 327155.5546875,
            "unit": "ns",
            "range": "± 9129.46581297929"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f6f6e561f0778e908763462f1040539e6dfbf7b7",
          "message": "Merge pull request #375 from zjklee/feature/data2\n\n`@data` and `noEscape`",
          "timestamp": "2020-10-27T10:33:12+02:00",
          "tree_id": "6a6155ccefed01997ee65d0d87619a413385a111",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/f6f6e561f0778e908763462f1040539e6dfbf7b7"
        },
        "date": 1603787918895,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 34028290.79523809,
            "unit": "ns",
            "range": "± 1265260.5055652475"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 357.6571513244084,
            "unit": "ns",
            "range": "± 0.0994648884908847"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 367.04995485941566,
            "unit": "ns",
            "range": "± 0.6793168463924086"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 371.1152695142306,
            "unit": "ns",
            "range": "± 5.895480755963987"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 546.4431444803873,
            "unit": "ns",
            "range": "± 12.79331417804332"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 369.2997646649679,
            "unit": "ns",
            "range": "± 0.7741897444180182"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 370.3078217873207,
            "unit": "ns",
            "range": "± 0.19367102678328396"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 595.3057911396027,
            "unit": "ns",
            "range": "± 0.18834202068664138"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 608.0273943680984,
            "unit": "ns",
            "range": "± 0.31857228484645445"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 640.8190136637006,
            "unit": "ns",
            "range": "± 10.067498576755902"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 210968.2376239483,
            "unit": "ns",
            "range": "± 427.8269097335108"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 230165.66662597656,
            "unit": "ns",
            "range": "± 613.636274876524"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "4f239ba10482c9aec20736ebc4205f3d79d2e3d1",
          "message": "Merge pull request #378 from zjklee/feature/helper-options\n\nIntroduce `HelperOptions` in Helper and ReturnHelper",
          "timestamp": "2020-11-01T02:41:44+02:00",
          "tree_id": "1bd84c0ad5421bf36e7cfb11d33aa14a6fa2c800",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/4f239ba10482c9aec20736ebc4205f3d79d2e3d1"
        },
        "date": 1604191661110,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 37123938.87142857,
            "unit": "ns",
            "range": "± 1613045.9086633173"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 411.7348847389221,
            "unit": "ns",
            "range": "± 6.068146320564097"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 409.96892093022666,
            "unit": "ns",
            "range": "± 15.965580484713643"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 414.5392350196838,
            "unit": "ns",
            "range": "± 5.502593982906607"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 574.3198080796462,
            "unit": "ns",
            "range": "± 5.964412447590354"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 417.63938630421956,
            "unit": "ns",
            "range": "± 6.613510478210782"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 400.63736292521156,
            "unit": "ns",
            "range": "± 8.75476946934173"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 661.6791969812833,
            "unit": "ns",
            "range": "± 14.01414976974492"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 651.2134197235107,
            "unit": "ns",
            "range": "± 24.79920038965599"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 677.8279979070028,
            "unit": "ns",
            "range": "± 6.737491809982804"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 212156.9918682392,
            "unit": "ns",
            "range": "± 2977.1376699148336"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 229510.01220703125,
            "unit": "ns",
            "range": "± 5568.1790712641005"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "575488d13e62dcbddbb03f48e50334412aef01f4",
          "message": "Merge pull request #382 from zjklee/issues/217\n\nFix whitespace support for partials",
          "timestamp": "2020-11-04T00:47:58+02:00",
          "tree_id": "3d9b725d7a22c331a0397815a9b5b3e501b77640",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/575488d13e62dcbddbb03f48e50334412aef01f4"
        },
        "date": 1604444022018,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 36552730.13333334,
            "unit": "ns",
            "range": "± 567127.2976141133"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 394.5375050385793,
            "unit": "ns",
            "range": "± 4.87674705757617"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 406.1298780759176,
            "unit": "ns",
            "range": "± 7.449846048844877"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 427.7546645800273,
            "unit": "ns",
            "range": "± 9.086424632599417"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 573.5755173819406,
            "unit": "ns",
            "range": "± 10.136062247311587"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 412.9993065765926,
            "unit": "ns",
            "range": "± 8.6141764916838"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 406.9154349395207,
            "unit": "ns",
            "range": "± 8.584512599545015"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 648.7821333067758,
            "unit": "ns",
            "range": "± 9.536225822359233"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 648.8216787746975,
            "unit": "ns",
            "range": "± 8.036522554192729"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 652.4849908192953,
            "unit": "ns",
            "range": "± 10.065910962258377"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 205189.05963134766,
            "unit": "ns",
            "range": "± 2120.004764019527"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 231404.65865885417,
            "unit": "ns",
            "range": "± 5934.813375092127"
          }
        ]
      },
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
          "id": "fcf7d5718c0288583c2dd40071367f379e328b79",
          "message": "Add DynamicPartial test\n\nCloses Handlebars-Net/Handlebars.Net/issues/324",
          "timestamp": "2020-11-04T01:27:24+02:00",
          "tree_id": "6cc7f466422358d54d54201cd2e9f8fcb1dc2706",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/fcf7d5718c0288583c2dd40071367f379e328b79"
        },
        "date": 1604446374080,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 34586852.48660714,
            "unit": "ns",
            "range": "± 481383.9885916075"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 364.7207196553548,
            "unit": "ns",
            "range": "± 2.824186553547778"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 370.4663365364075,
            "unit": "ns",
            "range": "± 3.40310275574329"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 378.21554202299853,
            "unit": "ns",
            "range": "± 1.5943492390427638"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 574.1827005239634,
            "unit": "ns",
            "range": "± 6.3569435360193385"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 370.65517743428546,
            "unit": "ns",
            "range": "± 2.5275950309350983"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 379.4534394900004,
            "unit": "ns",
            "range": "± 2.691831308139732"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 605.1295370688805,
            "unit": "ns",
            "range": "± 6.046740858029136"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 618.5915754318237,
            "unit": "ns",
            "range": "± 2.8950472374841887"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 620.4698373794556,
            "unit": "ns",
            "range": "± 14.92223485164165"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 214924.8247419085,
            "unit": "ns",
            "range": "± 1307.3105657917329"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 245065.77696940105,
            "unit": "ns",
            "range": "± 2512.6777003973725"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "93625bf2f8e18ba1af0a1f720c3ab2e1dad371f2",
          "message": "Merge pull request #381 from zjklee/feature/iterators\n\nIntroduce `IIterator` + structure reorganisation",
          "timestamp": "2020-11-06T19:19:46+02:00",
          "tree_id": "d859180d0e7a574707f97bb15b71af7daa43eb10",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/93625bf2f8e18ba1af0a1f720c3ab2e1dad371f2"
        },
        "date": 1604683497629,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 8001045.96875,
            "unit": "ns",
            "range": "± 59723.55483058003"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 365.046790940421,
            "unit": "ns",
            "range": "± 1.1589626450915"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 365.10151859692166,
            "unit": "ns",
            "range": "± 2.9094907005612654"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 379.09357522328696,
            "unit": "ns",
            "range": "± 4.75735520594715"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 537.8893058640616,
            "unit": "ns",
            "range": "± 2.714767956320843"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 366.0700233532832,
            "unit": "ns",
            "range": "± 2.279631248917551"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 370.9645134485685,
            "unit": "ns",
            "range": "± 1.7402318003198771"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 591.5990870793661,
            "unit": "ns",
            "range": "± 4.075673646791099"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 608.4044011189387,
            "unit": "ns",
            "range": "± 4.642973385318162"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 607.123424911499,
            "unit": "ns",
            "range": "± 3.9093984307506133"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 194676.30477469307,
            "unit": "ns",
            "range": "± 855.1184580608164"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 205912.28867885045,
            "unit": "ns",
            "range": "± 1067.8276426967147"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "41167595+probot-auto-merge[bot]@users.noreply.github.com",
            "name": "probot-auto-merge[bot]",
            "username": "probot-auto-merge[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d87dc59c067f7601fe5edfb5808582fe78e4d883",
          "message": "Merge pull request #358 from sferencik/fix-readme\n\nFix syntax for block helpers",
          "timestamp": "2020-11-07T15:26:44Z",
          "tree_id": "05a674e373a4c71ee233a7c3f6e0560450fbaa1c",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/d87dc59c067f7601fe5edfb5808582fe78e4d883"
        },
        "date": 1604763150812,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 8558416.080208333,
            "unit": "ns",
            "range": "± 204394.826918946"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 407.14831962585447,
            "unit": "ns",
            "range": "± 4.987448467401553"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 411.74183668409074,
            "unit": "ns",
            "range": "± 3.9287601854379766"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 449.2695032869066,
            "unit": "ns",
            "range": "± 8.296057833698596"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 593.3442877989548,
            "unit": "ns",
            "range": "± 4.078363704507277"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 411.5628914197286,
            "unit": "ns",
            "range": "± 5.840609496760153"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 418.24362325668335,
            "unit": "ns",
            "range": "± 4.622495515340131"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 684.7146427790324,
            "unit": "ns",
            "range": "± 9.093738664182084"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 690.7552483422415,
            "unit": "ns",
            "range": "± 13.003897459448561"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 703.679762395223,
            "unit": "ns",
            "range": "± 8.567426581912201"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 201957.86488560267,
            "unit": "ns",
            "range": "± 2016.429356869775"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 220175.95511067708,
            "unit": "ns",
            "range": "± 3199.6703835022977"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "271e165ac052464e269e4f3535b7d8eef82635cc",
          "message": "Merge pull request #384 from zjklee/feature/helper-ref\n\nReplace `StrongBox<T>` with `Ref<T>`",
          "timestamp": "2020-11-07T21:02:18+02:00",
          "tree_id": "3713fab848076f659a6fa949ed8ea19d9cf99c50",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/271e165ac052464e269e4f3535b7d8eef82635cc"
        },
        "date": 1604776061348,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 10158681.46986607,
            "unit": "ns",
            "range": "± 120107.09686455225"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 361.7967798028673,
            "unit": "ns",
            "range": "± 1.3204932198286172"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 381.2459104537964,
            "unit": "ns",
            "range": "± 5.096394211452154"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 388.7952182633536,
            "unit": "ns",
            "range": "± 7.327395865057832"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 570.9548582077026,
            "unit": "ns",
            "range": "± 2.5301205794119763"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 371.04702514012655,
            "unit": "ns",
            "range": "± 2.0081096773010456"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 381.05173250834144,
            "unit": "ns",
            "range": "± 1.6510987675937312"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 599.4635137411265,
            "unit": "ns",
            "range": "± 8.040178708093551"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 609.1257247243609,
            "unit": "ns",
            "range": "± 3.1036264942398715"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 678.9162092208862,
            "unit": "ns",
            "range": "± 4.393632527099495"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 200854.8007436899,
            "unit": "ns",
            "range": "± 2220.608711197849"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 217486.7310546875,
            "unit": "ns",
            "range": "± 3615.117634289309"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "41167595+probot-auto-merge[bot]@users.noreply.github.com",
            "name": "probot-auto-merge[bot]",
            "username": "probot-auto-merge[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "4c97ac918a20424f50da988f19ab0b23f655299a",
          "message": "Merge pull request #356 from perlun/patch-1\n\nREADME.md: Add explicit casting in Handlebars.RegisterHelper",
          "timestamp": "2020-11-07T19:35:06Z",
          "tree_id": "69631b4507d888f1bd1eb500f59ed84ba3592f58",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/4c97ac918a20424f50da988f19ab0b23f655299a"
        },
        "date": 1604778041760,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 11219588.760044644,
            "unit": "ns",
            "range": "± 185389.2554516825"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 381.6916502157847,
            "unit": "ns",
            "range": "± 6.551577105921435"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 399.36676082611086,
            "unit": "ns",
            "range": "± 18.198661171611285"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 404.35497873624166,
            "unit": "ns",
            "range": "± 6.108198886818831"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 564.7705254554749,
            "unit": "ns",
            "range": "± 6.619629014264427"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 431.48929195404054,
            "unit": "ns",
            "range": "± 18.04243524220333"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 399.9814504623413,
            "unit": "ns",
            "range": "± 4.370231951150221"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 616.7209432601928,
            "unit": "ns",
            "range": "± 10.48500078239668"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 662.2042280197144,
            "unit": "ns",
            "range": "± 18.81115829246476"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 674.5099058787029,
            "unit": "ns",
            "range": "± 12.021986599373003"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 193835.22934570312,
            "unit": "ns",
            "range": "± 3968.5312792365517"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 203127.39358723958,
            "unit": "ns",
            "range": "± 4539.193418747925"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3c3af033c875f1a2012f442bc3443ce77dba8364",
          "message": "Merge pull request #386 from zjklee/issue/383\n\nUse `ParentContext` when handling `@partial-block`",
          "timestamp": "2020-11-11T08:03:46-08:00",
          "tree_id": "9566c66a8564a9e6cb4991fcfd19eeac9ac27544",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/3c3af033c875f1a2012f442bc3443ce77dba8364"
        },
        "date": 1605110945429,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 9225062.63392857,
            "unit": "ns",
            "range": "± 16444.40203970865"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 318.0631761891501,
            "unit": "ns",
            "range": "± 0.06530864522922991"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 325.0798048239488,
            "unit": "ns",
            "range": "± 0.11233410717675368"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 323.316931327184,
            "unit": "ns",
            "range": "± 0.05770895122136288"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 465.1785354614258,
            "unit": "ns",
            "range": "± 0.46123228093287355"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 316.8643116950989,
            "unit": "ns",
            "range": "± 0.07784316484738771"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 323.7334528923035,
            "unit": "ns",
            "range": "± 0.10125537363600529"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 517.1704692840576,
            "unit": "ns",
            "range": "± 0.08434779148887725"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 528.9313157154963,
            "unit": "ns",
            "range": "± 0.3059875749550525"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 541.5098329271589,
            "unit": "ns",
            "range": "± 0.11416443241002715"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 167769.21407645088,
            "unit": "ns",
            "range": "± 632.0063340645966"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 174809.31548602766,
            "unit": "ns",
            "range": "± 60.65700218307799"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "5149d7a6806f780753f1a1c6edac55bd8c4a7c4e",
          "message": "Merge pull request #388 from zjklee/feature/improve-internal-collections\n\nExpose inner interfaces in configuration",
          "timestamp": "2020-11-16T11:11:31-08:00",
          "tree_id": "2fd7d5f30f149b5c9763e8ebb04c858a40b23f1a",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/5149d7a6806f780753f1a1c6edac55bd8c4a7c4e"
        },
        "date": 1605554228991,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 11165144.575,
            "unit": "ns",
            "range": "± 367155.8960666653"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 357.6046886444092,
            "unit": "ns",
            "range": "± 11.824370366076852"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 368.36316916147865,
            "unit": "ns",
            "range": "± 6.147843776972714"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 381.05272207260134,
            "unit": "ns",
            "range": "± 4.853656113558389"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 579.550513013204,
            "unit": "ns",
            "range": "± 8.529489218158943"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 369.5127893447876,
            "unit": "ns",
            "range": "± 4.987387033112359"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 383.4870189666748,
            "unit": "ns",
            "range": "± 8.265672047968632"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 601.3039999643962,
            "unit": "ns",
            "range": "± 27.860765690429268"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 587.8702471733093,
            "unit": "ns",
            "range": "± 23.538584454862008"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 598.3573600451151,
            "unit": "ns",
            "range": "± 16.268283346615423"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 206927.23648507256,
            "unit": "ns",
            "range": "± 5153.568579398805"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 209462.55659179686,
            "unit": "ns",
            "range": "± 7297.032644821255"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "0b00295a8ae9392229c0267a0cda44c26860b265",
          "message": "Merge pull request #389 from zjklee/feature/less-compiler-allocations\n\nReduce compiler allocations",
          "timestamp": "2020-11-16T14:00:19-08:00",
          "tree_id": "81a2bd8e49193b728c430a5e68df13114dbe07da",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/0b00295a8ae9392229c0267a0cda44c26860b265"
        },
        "date": 1605564349206,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 10789579.015625,
            "unit": "ns",
            "range": "± 305012.10868710285"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 363.4632857322693,
            "unit": "ns",
            "range": "± 3.2060180936653597"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 370.85585383006503,
            "unit": "ns",
            "range": "± 4.541327784662626"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 388.21718041102093,
            "unit": "ns",
            "range": "± 9.688594408740224"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 540.1633776346843,
            "unit": "ns",
            "range": "± 11.58804991293326"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 370.6034899439131,
            "unit": "ns",
            "range": "± 4.3034809357976345"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 380.02298221588137,
            "unit": "ns",
            "range": "± 5.085580811238182"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 586.1924699636606,
            "unit": "ns",
            "range": "± 8.70522624727927"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 598.0728092193604,
            "unit": "ns",
            "range": "± 6.837798785318802"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 601.3936422348022,
            "unit": "ns",
            "range": "± 6.34373277756443"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 186055.52059500557,
            "unit": "ns",
            "range": "± 2407.185641415902"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 197943.7073079427,
            "unit": "ns",
            "range": "± 2066.5456587481126"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b61e6a6b5dce977a2339a09a8cf09e21b1006c0d",
          "message": "Merge pull request #390 from zjklee/feature/formatters\n\nIntroduce value formatters",
          "timestamp": "2020-11-20T10:04:01-08:00",
          "tree_id": "626271f4879ba750c6f4c4ac89dda95d7c47a567",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/b61e6a6b5dce977a2339a09a8cf09e21b1006c0d"
        },
        "date": 1605895746833,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 18595602.620833334,
            "unit": "ns",
            "range": "± 395717.50483066146"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 323.97672281946456,
            "unit": "ns",
            "range": "± 6.616744348164495"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 334.72478669484457,
            "unit": "ns",
            "range": "± 6.622125641778866"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 343.09424273173016,
            "unit": "ns",
            "range": "± 7.347903411532653"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 495.77762333552045,
            "unit": "ns",
            "range": "± 12.623446362021973"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 339.46752338409425,
            "unit": "ns",
            "range": "± 6.0578263938368035"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 344.81287240982056,
            "unit": "ns",
            "range": "± 4.081466167313984"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 500.7069564819336,
            "unit": "ns",
            "range": "± 9.459149868686868"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 516.5110808372498,
            "unit": "ns",
            "range": "± 11.55994461643903"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 512.4229729516165,
            "unit": "ns",
            "range": "± 8.609715942460161"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 228273.940952846,
            "unit": "ns",
            "range": "± 3622.5611361299216"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 231431.02511160713,
            "unit": "ns",
            "range": "± 3801.193222827125"
          }
        ]
      },
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
          "id": "068d4bb08a50f49f9a56bb1a761003dae170d01d",
          "message": "Improve `IMemberAliasProvider`",
          "timestamp": "2020-12-02T01:13:08+02:00",
          "tree_id": "6e0e77614bea177f7616b1f0b274d36c207472e9",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/068d4bb08a50f49f9a56bb1a761003dae170d01d"
        },
        "date": 1606864729407,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20792540.64285714,
            "unit": "ns",
            "range": "± 372969.06350382755"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 392.029567305247,
            "unit": "ns",
            "range": "± 8.296584097130776"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 390.7293132373265,
            "unit": "ns",
            "range": "± 7.539305438477856"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 401.08768297831216,
            "unit": "ns",
            "range": "± 6.850640403870313"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 563.2656744321188,
            "unit": "ns",
            "range": "± 8.432613471375847"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 424.6053887367249,
            "unit": "ns",
            "range": "± 9.230422718684048"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 401.22712093989054,
            "unit": "ns",
            "range": "± 7.459067788249048"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 626.0335987726847,
            "unit": "ns",
            "range": "± 10.463012802597929"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 634.5836556979588,
            "unit": "ns",
            "range": "± 5.530564573181684"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 650.9406378609793,
            "unit": "ns",
            "range": "± 11.266435874162477"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 262765.9591238839,
            "unit": "ns",
            "range": "± 2851.664921706174"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 274808.28079659597,
            "unit": "ns",
            "range": "± 5230.602123340131"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d707cc94e2576f82ab3fbfc875fba4ac3a7a70db",
          "message": "Merge pull request #391 from zjklee/feature/ambient-context\n\nIntroduce `AmbientContext`",
          "timestamp": "2020-12-02T12:49:37-08:00",
          "tree_id": "89c2c1e991e3e56f28fdaba1f57bd24a7a873a1f",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/d707cc94e2576f82ab3fbfc875fba4ac3a7a70db"
        },
        "date": 1606942536756,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19448792.777083334,
            "unit": "ns",
            "range": "± 100708.03763503165"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 579.2479434331258,
            "unit": "ns",
            "range": "± 12.044357010422255"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 616.3534875869751,
            "unit": "ns",
            "range": "± 9.334838008353406"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 626.6492119471233,
            "unit": "ns",
            "range": "± 10.654763397090639"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 801.0533302852085,
            "unit": "ns",
            "range": "± 10.130500427455974"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 614.0642555872599,
            "unit": "ns",
            "range": "± 12.39078999415573"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 620.3208013534546,
            "unit": "ns",
            "range": "± 9.417451717233808"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 791.0527808507284,
            "unit": "ns",
            "range": "± 15.111350943953045"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 763.144511381785,
            "unit": "ns",
            "range": "± 21.47623795043646"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 749.5991733414786,
            "unit": "ns",
            "range": "± 17.0634876666429"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 274087.41751302086,
            "unit": "ns",
            "range": "± 2637.55038501885"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 266595.13430989586,
            "unit": "ns",
            "range": "± 4118.843194155332"
          }
        ]
      },
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
          "id": "27e79234bf02c303dacad3bc02b697ad8fe0accc",
          "message": "Fix subscription direction",
          "timestamp": "2020-12-02T23:53:36+02:00",
          "tree_id": "59a672999d2f69c08122bcfdd4e2fcdd58931567",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/27e79234bf02c303dacad3bc02b697ad8fe0accc"
        },
        "date": 1606946359167,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 18712269.26875,
            "unit": "ns",
            "range": "± 319513.8473076193"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 616.9380691392081,
            "unit": "ns",
            "range": "± 2.9361756509010593"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 659.354141108195,
            "unit": "ns",
            "range": "± 12.936846313398563"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 642.1979033606393,
            "unit": "ns",
            "range": "± 3.6727040691927306"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 858.9302401224772,
            "unit": "ns",
            "range": "± 3.9823411974525516"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 649.1606486002604,
            "unit": "ns",
            "range": "± 3.276963973478493"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 645.3251979534442,
            "unit": "ns",
            "range": "± 2.010007884727369"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 826.5489242076874,
            "unit": "ns",
            "range": "± 1.4770419686180432"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 843.6231086095174,
            "unit": "ns",
            "range": "± 1.4277972502693617"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 853.824528840872,
            "unit": "ns",
            "range": "± 1.9430884825003234"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 265340.3311686198,
            "unit": "ns",
            "range": "± 3417.6831831405275"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 287750.604422433,
            "unit": "ns",
            "range": "± 3280.352484062722"
          }
        ]
      },
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
          "id": "21dc530e3d9d9b9f5b4c317d9ee603619f386294",
          "message": "Fix support for `struct` enumerators in `ExtendedEnumerator`",
          "timestamp": "2020-12-03T00:38:08+02:00",
          "tree_id": "45331c78cbd68e1b55c68139fa10f25162e7d9ae",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/21dc530e3d9d9b9f5b4c317d9ee603619f386294"
        },
        "date": 1606949016222,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 18843272.981770832,
            "unit": "ns",
            "range": "± 239965.632597829"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 571.7412096659342,
            "unit": "ns",
            "range": "± 19.819213394735815"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 556.6350046157837,
            "unit": "ns",
            "range": "± 20.44911619358745"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 584.9741022109986,
            "unit": "ns",
            "range": "± 26.414601554771778"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 698.7664411251361,
            "unit": "ns",
            "range": "± 25.20151313692663"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 582.2911779085795,
            "unit": "ns",
            "range": "± 21.06034087920543"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 595.5773295720418,
            "unit": "ns",
            "range": "± 28.68041881653256"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 693.4368607447698,
            "unit": "ns",
            "range": "± 11.944361639952287"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 747.3066433270773,
            "unit": "ns",
            "range": "± 28.181232056732966"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 765.1574738184611,
            "unit": "ns",
            "range": "± 29.32850596345185"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 265262.27975260414,
            "unit": "ns",
            "range": "± 4388.708382557258"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 255343.8607421875,
            "unit": "ns",
            "range": "± 6526.261957370825"
          }
        ]
      },
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
          "id": "ea7adce814ada37427f1986330f4ad98d3823735",
          "message": "Alternative to JS's `this`\nCloses Handlebars-Net/Handlebars.Net/issues/105",
          "timestamp": "2020-12-06T20:20:21+02:00",
          "tree_id": "fe21face4e2614ee7c5d69aacfe1e3e3723c4726",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/ea7adce814ada37427f1986330f4ad98d3823735"
        },
        "date": 1607279180390,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20775851.395833332,
            "unit": "ns",
            "range": "± 79407.07577447344"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 632.99263215065,
            "unit": "ns",
            "range": "± 0.16798614164032627"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 657.4384433201382,
            "unit": "ns",
            "range": "± 0.5483107556539513"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 656.4403831775372,
            "unit": "ns",
            "range": "± 0.3573902701393598"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 825.762154170445,
            "unit": "ns",
            "range": "± 2.2622075968748443"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 700.2560450480535,
            "unit": "ns",
            "range": "± 1.2704671936570398"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 659.7789045061384,
            "unit": "ns",
            "range": "± 2.0386638184546997"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 843.1880687986102,
            "unit": "ns",
            "range": "± 3.7808666528241806"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 871.7750539779663,
            "unit": "ns",
            "range": "± 1.9984708263988076"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 879.268374647413,
            "unit": "ns",
            "range": "± 3.873252836991321"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 266832.2041829427,
            "unit": "ns",
            "range": "± 671.3004481676338"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 277503.4504231771,
            "unit": "ns",
            "range": "± 1117.3370114711333"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "46c80b2cdbe7ce30d7292d395e0414c1a21f596b",
          "message": "Fix assembly signing",
          "timestamp": "2020-12-07T10:33:10-08:00",
          "tree_id": "eb31e1da5ed9bbd07c7c39a80135760096ec3d5c",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/46c80b2cdbe7ce30d7292d395e0414c1a21f596b"
        },
        "date": 1607366314811,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20396919.98125,
            "unit": "ns",
            "range": "± 92925.02396876492"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 612.2651493365948,
            "unit": "ns",
            "range": "± 1.2271267298278827"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 637.7170386314392,
            "unit": "ns",
            "range": "± 1.5902812976782075"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 640.5784173965454,
            "unit": "ns",
            "range": "± 0.9656080200070701"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 867.9393510137286,
            "unit": "ns",
            "range": "± 2.6184832648404806"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 657.4755778993879,
            "unit": "ns",
            "range": "± 3.440024477649398"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 661.7328226225717,
            "unit": "ns",
            "range": "± 0.6781184545211024"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 861.987762705485,
            "unit": "ns",
            "range": "± 22.267373931644347"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 874.6616968427386,
            "unit": "ns",
            "range": "± 13.378927321051831"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 880.6332376797994,
            "unit": "ns",
            "range": "± 1.3902937012901102"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 268127.5564313616,
            "unit": "ns",
            "range": "± 635.5754214954125"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 278290.7150390625,
            "unit": "ns",
            "range": "± 700.559745356168"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f862782ceb4bff22197ff3ec8aac72e564219d74",
          "message": "Merge pull request #396 from zjklee/issues/393\n\nMake respective classes serialize for net451 and net452",
          "timestamp": "2020-12-07T16:38:32-08:00",
          "tree_id": "25182a1cb25f31e3fdf26368cb5e1f4ba8269f3f",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/f862782ceb4bff22197ff3ec8aac72e564219d74"
        },
        "date": 1607388212849,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 15932166.54326923,
            "unit": "ns",
            "range": "± 128452.39282401006"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 476.8127046312605,
            "unit": "ns",
            "range": "± 5.509368546162134"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 520.5659739630563,
            "unit": "ns",
            "range": "± 6.815602251976222"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 475.4572722752889,
            "unit": "ns",
            "range": "± 7.917478096494305"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 643.8925660678318,
            "unit": "ns",
            "range": "± 6.402947540499416"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 497.2176496432378,
            "unit": "ns",
            "range": "± 9.660676353635731"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 500.3052630106608,
            "unit": "ns",
            "range": "± 14.197554091578061"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 657.1000182151795,
            "unit": "ns",
            "range": "± 10.482576270893984"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 658.4966057997483,
            "unit": "ns",
            "range": "± 6.897817010549283"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 675.218183708191,
            "unit": "ns",
            "range": "± 15.511596930373774"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 208647.28090122767,
            "unit": "ns",
            "range": "± 3111.2863564037166"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 225193.75150240384,
            "unit": "ns",
            "range": "± 2949.3576112855376"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a4c8db094f9b0069b5c00eabcd09ba2c6e2cf57d",
          "message": "Merge pull request #397 from zjklee/issues/395\n\nFix case insensitive selection of WellKnownVariable",
          "timestamp": "2020-12-07T17:07:32-08:00",
          "tree_id": "2db6679e3231a49eeee9b7134f7bb8e6250431a6",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/a4c8db094f9b0069b5c00eabcd09ba2c6e2cf57d"
        },
        "date": 1607389989787,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19902883.698660713,
            "unit": "ns",
            "range": "± 394250.70334067446"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 578.6445392290751,
            "unit": "ns",
            "range": "± 5.549776082081189"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 590.5500820704868,
            "unit": "ns",
            "range": "± 8.962210873064143"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 636.0624630610148,
            "unit": "ns",
            "range": "± 14.126367349219255"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 794.2733682904925,
            "unit": "ns",
            "range": "± 10.31159531729149"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 647.4512135187784,
            "unit": "ns",
            "range": "± 14.431910766401977"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 642.0005065282186,
            "unit": "ns",
            "range": "± 20.650641077992915"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 770.3783132870992,
            "unit": "ns",
            "range": "± 14.989471213861538"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 816.6258433024088,
            "unit": "ns",
            "range": "± 8.294939722775338"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 866.825203259786,
            "unit": "ns",
            "range": "± 21.123037923500878"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 242900.08989606585,
            "unit": "ns",
            "range": "± 5342.074714294402"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 252222.74293619793,
            "unit": "ns",
            "range": "± 3834.202366412663"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "0a03914fba43728d6a38d5bde851fa7886348534",
          "message": "Merge pull request #398 from mhornbacher/master\n\nfix #394 - get first not single",
          "timestamp": "2020-12-08T02:00:50-08:00",
          "tree_id": "00637d6a7dcd695642bf8ac8d209c2437b248d0d",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/0a03914fba43728d6a38d5bde851fa7886348534"
        },
        "date": 1607421983221,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25401703.714583334,
            "unit": "ns",
            "range": "± 956093.1376778618"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 713.5640738805135,
            "unit": "ns",
            "range": "± 11.964235883485316"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 739.538239924113,
            "unit": "ns",
            "range": "± 11.60486698537758"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 757.9264465059553,
            "unit": "ns",
            "range": "± 12.945220054043235"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 998.7286193211874,
            "unit": "ns",
            "range": "± 19.581044277152934"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 731.1671495755513,
            "unit": "ns",
            "range": "± 16.239372927049136"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 743.8782307761056,
            "unit": "ns",
            "range": "± 11.538439433886467"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 980.891735458374,
            "unit": "ns",
            "range": "± 14.286285171894058"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 998.0223651885987,
            "unit": "ns",
            "range": "± 25.49718878773071"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 988.2659404754638,
            "unit": "ns",
            "range": "± 29.197039678268837"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 271093.4992675781,
            "unit": "ns",
            "range": "± 3910.6258228776856"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 279955.6073242187,
            "unit": "ns",
            "range": "± 5463.809281252959"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "0a03914fba43728d6a38d5bde851fa7886348534",
          "message": "Merge pull request #398 from mhornbacher/master\n\nfix #394 - get first not single",
          "timestamp": "2020-12-08T02:00:50-08:00",
          "tree_id": "00637d6a7dcd695642bf8ac8d209c2437b248d0d",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/0a03914fba43728d6a38d5bde851fa7886348534"
        },
        "date": 1607428083709,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 21132229.332589287,
            "unit": "ns",
            "range": "± 34829.71210319571"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 663.9921232632229,
            "unit": "ns",
            "range": "± 2.998996244183471"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 655.8344450632732,
            "unit": "ns",
            "range": "± 7.324708724770043"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 661.6671333312988,
            "unit": "ns",
            "range": "± 1.7764379759416005"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 863.9325842490563,
            "unit": "ns",
            "range": "± 1.3353450886233256"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 655.7784106181218,
            "unit": "ns",
            "range": "± 2.940256770090732"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 665.3854118347168,
            "unit": "ns",
            "range": "± 1.7849889366181189"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 856.5096039090838,
            "unit": "ns",
            "range": "± 1.81847741306885"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 864.4273406982422,
            "unit": "ns",
            "range": "± 2.919276257031738"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 887.8669743537903,
            "unit": "ns",
            "range": "± 1.6769304727946042"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 255572.21618652344,
            "unit": "ns",
            "range": "± 329.5165188787986"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 265062.31640625,
            "unit": "ns",
            "range": "± 532.0381066857292"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "99d92f4310287a255151acfa3deff314cc36af0f",
          "message": "Merge pull request #403 from zjklee/fix/path-resolvement\n\nMultiple fixes to path resolvement",
          "timestamp": "2020-12-11T07:21:22-08:00",
          "tree_id": "e08f457d9068b0f8a6893ce3a856c5e53d026677",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/99d92f4310287a255151acfa3deff314cc36af0f"
        },
        "date": 1607700422648,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20044424.91346154,
            "unit": "ns",
            "range": "± 179852.45840863488"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 590.0986453692118,
            "unit": "ns",
            "range": "± 17.5420933978147"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 554.9817361831665,
            "unit": "ns",
            "range": "± 13.806303311365538"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 605.446399752299,
            "unit": "ns",
            "range": "± 22.266039988090196"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 860.2651877403259,
            "unit": "ns",
            "range": "± 21.321159008823063"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 647.5056296030681,
            "unit": "ns",
            "range": "± 19.421940380411723"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 617.0153594017029,
            "unit": "ns",
            "range": "± 8.86066695171078"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 788.7986570504995,
            "unit": "ns",
            "range": "± 7.2361559273706195"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 937.9395475387573,
            "unit": "ns",
            "range": "± 8.407959374264093"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 877.3150574366251,
            "unit": "ns",
            "range": "± 66.13975641810083"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 247606.3129231771,
            "unit": "ns",
            "range": "± 7824.1355270150825"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 245738.5713936942,
            "unit": "ns",
            "range": "± 6366.316690785009"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "6ce87b65a8c13bc315895f010da227882ed52b3d",
          "message": "Merge pull request #405 from zjklee/issues/387\n\nFix `^if`, `^unless` and `^each`",
          "timestamp": "2020-12-11T08:30:03-08:00",
          "tree_id": "695dd9c96435fa9d1fcf989b38837ad5748cb277",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/6ce87b65a8c13bc315895f010da227882ed52b3d"
        },
        "date": 1607704543195,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 23330112.31919643,
            "unit": "ns",
            "range": "± 61282.60026231208"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 629.1869071324667,
            "unit": "ns",
            "range": "± 25.728067592641555"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 642.0420251259437,
            "unit": "ns",
            "range": "± 1.947251666574062"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 644.3838714872088,
            "unit": "ns",
            "range": "± 2.0430331669055226"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 833.6639590263367,
            "unit": "ns",
            "range": "± 0.8839913841036448"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 644.0049471173968,
            "unit": "ns",
            "range": "± 11.652438439614713"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 658.438824780782,
            "unit": "ns",
            "range": "± 4.199616389199829"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 877.5040862401327,
            "unit": "ns",
            "range": "± 3.288775530440582"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 894.1018781661987,
            "unit": "ns",
            "range": "± 1.1969304053582723"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 904.6948950449625,
            "unit": "ns",
            "range": "± 3.6080457417405576"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 267422.52968052455,
            "unit": "ns",
            "range": "± 1068.2012328373182"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 286325.4210205078,
            "unit": "ns",
            "range": "± 694.9608680589707"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "bfcffe065d50122722e70feb9e5bad4caddd1471",
          "message": "Merge pull request #406 from zjklee/chore/latest-sdk-versions\n\nUse latest SDK versions in GitHub Actions",
          "timestamp": "2020-12-11T12:03:17-08:00",
          "tree_id": "21896dc3304eeefda8359fc57c0bb6d41f1dd866",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/bfcffe065d50122722e70feb9e5bad4caddd1471"
        },
        "date": 1607717319390,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 24719586.285416666,
            "unit": "ns",
            "range": "± 367089.8089120192"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 644.3413440068563,
            "unit": "ns",
            "range": "± 11.94395114249085"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 658.9906843049185,
            "unit": "ns",
            "range": "± 10.127322958152579"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 669.0065199534098,
            "unit": "ns",
            "range": "± 9.537196114637311"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 892.9917309443156,
            "unit": "ns",
            "range": "± 10.9066126072791"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 639.9728421529134,
            "unit": "ns",
            "range": "± 7.560882677918951"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 669.7756440822894,
            "unit": "ns",
            "range": "± 4.7387279739139405"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 906.7974166870117,
            "unit": "ns",
            "range": "± 18.444126201515846"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 940.8192373003278,
            "unit": "ns",
            "range": "± 9.729529018795656"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 878.3804944356283,
            "unit": "ns",
            "range": "± 41.97265024484475"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 280406.722265625,
            "unit": "ns",
            "range": "± 3623.2600668181735"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 288020.07067871094,
            "unit": "ns",
            "range": "± 2379.888231427678"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c7d6ed8d445de33253443d52607dfc295ed59cd4",
          "message": "Update bug_report.md",
          "timestamp": "2020-12-17T17:23:40+02:00",
          "tree_id": "353f89e811cb8f6446f524ad3de9a27045745e6f",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/c7d6ed8d445de33253443d52607dfc295ed59cd4"
        },
        "date": 1608218940768,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 27630042.389583334,
            "unit": "ns",
            "range": "± 394665.8242991694"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 697.4604331970215,
            "unit": "ns",
            "range": "± 11.911791519291096"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 694.8299025535583,
            "unit": "ns",
            "range": "± 12.383021687035573"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 729.7156225204468,
            "unit": "ns",
            "range": "± 14.55925579595446"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 913.5729726382664,
            "unit": "ns",
            "range": "± 11.536394800881448"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 722.5646014531453,
            "unit": "ns",
            "range": "± 9.034681567353642"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 715.8841625213623,
            "unit": "ns",
            "range": "± 9.913607687287445"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 1021.0361151014056,
            "unit": "ns",
            "range": "± 20.51627827338589"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 1014.3028077345627,
            "unit": "ns",
            "range": "± 10.106055664772796"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 1037.5652513504028,
            "unit": "ns",
            "range": "± 17.424483200637987"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 312708.1337890625,
            "unit": "ns",
            "range": "± 3559.639449939236"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 297780.896484375,
            "unit": "ns",
            "range": "± 6110.96466694407"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ef732830fb05b70553bd39efa7da4e7c6db6d75f",
          "message": "Merge pull request #409 from zjklee/issues/408\n\nAdd boundaries check to MemberAccessor responsible for `IList*`",
          "timestamp": "2020-12-18T03:30:49-08:00",
          "tree_id": "813274e5e30bb75940dac1c4408ca747753085a3",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/ef732830fb05b70553bd39efa7da4e7c6db6d75f"
        },
        "date": 1608291415165,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 24381221.335416667,
            "unit": "ns",
            "range": "± 698943.943179266"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 622.7077430089315,
            "unit": "ns",
            "range": "± 26.832641171396748"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 634.7159806765043,
            "unit": "ns",
            "range": "± 14.630871096142517"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 621.5724114009312,
            "unit": "ns",
            "range": "± 23.834133732517696"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 793.5283640861511,
            "unit": "ns",
            "range": "± 25.33830358431176"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 608.4618665059407,
            "unit": "ns",
            "range": "± 29.915070046273367"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 635.9361822764079,
            "unit": "ns",
            "range": "± 25.65557435210037"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 900.941188621521,
            "unit": "ns",
            "range": "± 33.08526890925258"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 867.3158700806754,
            "unit": "ns",
            "range": "± 26.10022889260693"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 890.0073724746704,
            "unit": "ns",
            "range": "± 25.243321316629647"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 252734.80854492186,
            "unit": "ns",
            "range": "± 9916.439865418963"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 271556.4489908854,
            "unit": "ns",
            "range": "± 14702.8380945717"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3fe981a341e6d839619230b4ac1151d8870b0325",
          "message": "Update release.yml",
          "timestamp": "2020-12-19T17:57:48+02:00",
          "tree_id": "220d7122d8621c793f8a952034a6e06e0541d1eb",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/3fe981a341e6d839619230b4ac1151d8870b0325"
        },
        "date": 1608393788186,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 22585957.9375,
            "unit": "ns",
            "range": "± 176121.71697149792"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 607.1761294773647,
            "unit": "ns",
            "range": "± 5.376915971520828"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 643.2583979288737,
            "unit": "ns",
            "range": "± 4.640055454939555"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 616.2275805791219,
            "unit": "ns",
            "range": "± 5.301127695000525"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 817.2685340245565,
            "unit": "ns",
            "range": "± 11.796039682884835"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 633.466240755717,
            "unit": "ns",
            "range": "± 11.829908720269577"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 610.6829982121785,
            "unit": "ns",
            "range": "± 7.975407195538882"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 846.1224873406546,
            "unit": "ns",
            "range": "± 14.58005734394991"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 859.6712557928903,
            "unit": "ns",
            "range": "± 6.096040813158486"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 872.8708158220563,
            "unit": "ns",
            "range": "± 7.555233268571245"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 258871.71840122767,
            "unit": "ns",
            "range": "± 2403.923459520835"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 271984.1205078125,
            "unit": "ns",
            "range": "± 3045.825448951665"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3fe981a341e6d839619230b4ac1151d8870b0325",
          "message": "Update release.yml",
          "timestamp": "2020-12-19T17:57:48+02:00",
          "tree_id": "220d7122d8621c793f8a952034a6e06e0541d1eb",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/3fe981a341e6d839619230b4ac1151d8870b0325"
        },
        "date": 1608407474997,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19618655.660416666,
            "unit": "ns",
            "range": "± 104460.88368225351"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 540.6383539835612,
            "unit": "ns",
            "range": "± 1.4458772164300258"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 538.4788017954145,
            "unit": "ns",
            "range": "± 1.3697711911793629"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 540.8027908960978,
            "unit": "ns",
            "range": "± 1.396494699881405"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 720.1201652380137,
            "unit": "ns",
            "range": "± 0.8552677214137782"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 530.7050496510097,
            "unit": "ns",
            "range": "± 5.6849507655779865"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 534.6607624053955,
            "unit": "ns",
            "range": "± 0.8930635433088824"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 740.2815297933726,
            "unit": "ns",
            "range": "± 1.945313981363778"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 741.8996090888977,
            "unit": "ns",
            "range": "± 0.8305206161939156"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 763.7855617523194,
            "unit": "ns",
            "range": "± 2.4655601882250844"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 223825.37029157366,
            "unit": "ns",
            "range": "± 499.2904484809694"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 228740.15007672991,
            "unit": "ns",
            "range": "± 151.08326066371552"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3fe981a341e6d839619230b4ac1151d8870b0325",
          "message": "Update release.yml",
          "timestamp": "2020-12-19T17:57:48+02:00",
          "tree_id": "220d7122d8621c793f8a952034a6e06e0541d1eb",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/3fe981a341e6d839619230b4ac1151d8870b0325"
        },
        "date": 1608451090305,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 18632849.110416666,
            "unit": "ns",
            "range": "± 393777.5072580205"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 495.9787540435791,
            "unit": "ns",
            "range": "± 12.487589369890523"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 514.8657567977905,
            "unit": "ns",
            "range": "± 19.330284186845294"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 503.6026938878573,
            "unit": "ns",
            "range": "± 5.821539162245626"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 666.2948289235433,
            "unit": "ns",
            "range": "± 19.12281855578311"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 505.36432628631593,
            "unit": "ns",
            "range": "± 17.932502780362196"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 511.0393735249837,
            "unit": "ns",
            "range": "± 20.901764261856115"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 723.0256613951462,
            "unit": "ns",
            "range": "± 10.330229325086089"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 699.5324692726135,
            "unit": "ns",
            "range": "± 16.348663691102963"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 710.8852967534747,
            "unit": "ns",
            "range": "± 14.608500032881746"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 212083.73388671875,
            "unit": "ns",
            "range": "± 3073.4922966124527"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 221241.03404947917,
            "unit": "ns",
            "range": "± 4642.308282957146"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "oleh@formaniuk.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "distinct": true,
          "id": "90722fa5a4578cb0a47b2279221acd523f23ab60",
          "message": "Revert \"Update release.yml\"\n\nThis reverts commit 3fe981a341e6d839619230b4ac1151d8870b0325.",
          "timestamp": "2021-01-03T23:01:09+02:00",
          "tree_id": "813274e5e30bb75940dac1c4408ca747753085a3",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/90722fa5a4578cb0a47b2279221acd523f23ab60"
        },
        "date": 1609708007153,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 22895152.889583334,
            "unit": "ns",
            "range": "± 365370.4082243897"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 606.5296989440918,
            "unit": "ns",
            "range": "± 7.730601050085857"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 610.7510115305582,
            "unit": "ns",
            "range": "± 8.507403047588399"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 590.1685557683309,
            "unit": "ns",
            "range": "± 4.072740419870042"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 777.5408541134426,
            "unit": "ns",
            "range": "± 11.044871428463212"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 594.3350872039795,
            "unit": "ns",
            "range": "± 21.70608334104506"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 592.5695201237996,
            "unit": "ns",
            "range": "± 9.325381135273508"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 871.460604540507,
            "unit": "ns",
            "range": "± 36.945852699361765"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 839.9984431266785,
            "unit": "ns",
            "range": "± 8.933658554566438"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 843.031941986084,
            "unit": "ns",
            "range": "± 11.02307479162385"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 244101.65814678484,
            "unit": "ns",
            "range": "± 3084.4262160842422"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 253311.3000676082,
            "unit": "ns",
            "range": "± 1860.4590962566642"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c5b2ac3d08c363c4132fb838966ed5d189829870",
          "message": "Merge pull request #414 from zjklee/fix/github-actions\n\nFix Github Actions",
          "timestamp": "2021-01-07T22:32:27+02:00",
          "tree_id": "fe7dd248577f3286471e77b91121e38d56195dca",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/c5b2ac3d08c363c4132fb838966ed5d189829870"
        },
        "date": 1610051853263,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 26413997.89732143,
            "unit": "ns",
            "range": "± 410040.057265439"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 702.1513041178385,
            "unit": "ns",
            "range": "± 9.789571350702404"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 734.0460355622428,
            "unit": "ns",
            "range": "± 9.484332762468785"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 718.5955841200692,
            "unit": "ns",
            "range": "± 5.109612970214154"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 941.204044342041,
            "unit": "ns",
            "range": "± 12.19126488854675"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 711.8815449201144,
            "unit": "ns",
            "range": "± 3.8354127967135248"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 718.0915812810262,
            "unit": "ns",
            "range": "± 6.581796451834282"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 1003.5876337687174,
            "unit": "ns",
            "range": "± 14.884994329216363"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 1011.6163707460676,
            "unit": "ns",
            "range": "± 20.106336421205622"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 1014.8334919611613,
            "unit": "ns",
            "range": "± 10.145128086422538"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 279581.8723842076,
            "unit": "ns",
            "range": "± 2372.8424510131767"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 293305.27705891925,
            "unit": "ns",
            "range": "± 931.1808322416055"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "41167595+probot-auto-merge[bot]@users.noreply.github.com",
            "name": "probot-auto-merge[bot]",
            "username": "probot-auto-merge[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c82a3b09bf451be33d88f4477097b7f844332213",
          "message": "Merge pull request #413 from leniency/master\n\nFix for Issue #412",
          "timestamp": "2021-01-07T20:49:53Z",
          "tree_id": "bfd6262fa2ee359effc060b202ae7505ff86dd36",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/c82a3b09bf451be33d88f4477097b7f844332213"
        },
        "date": 1610052991880,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25728250.18973214,
            "unit": "ns",
            "range": "± 412444.6022538794"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 677.688476117452,
            "unit": "ns",
            "range": "± 8.127758780647131"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 706.7489074298313,
            "unit": "ns",
            "range": "± 11.785819981117235"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 702.7120621363322,
            "unit": "ns",
            "range": "± 20.91474618778907"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 891.1980419794719,
            "unit": "ns",
            "range": "± 16.887479027294248"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 702.0395933787028,
            "unit": "ns",
            "range": "± 8.277198047324285"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 706.5407567024231,
            "unit": "ns",
            "range": "± 9.098858704280918"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 993.0750362396241,
            "unit": "ns",
            "range": "± 18.138778913942115"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 973.2710618336995,
            "unit": "ns",
            "range": "± 16.201804085594144"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 993.6156587600708,
            "unit": "ns",
            "range": "± 20.383759672590685"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 309585.01028645836,
            "unit": "ns",
            "range": "± 8070.63743980842"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 278046.871875,
            "unit": "ns",
            "range": "± 6824.280999224916"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "320bc594d7c5b3a7ad29e1711c9ac7a95e5613e5",
          "message": "Merge pull request #427 from zjklee/issues/422",
          "timestamp": "2021-02-02T16:02:49+02:00",
          "tree_id": "fa0eb6766c78fcb992ca1d5e4b6b9c94a08f4e90",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/320bc594d7c5b3a7ad29e1711c9ac7a95e5613e5"
        },
        "date": 1612274887478,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25802784.402083334,
            "unit": "ns",
            "range": "± 273882.8982695511"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 685.1251846313477,
            "unit": "ns",
            "range": "± 6.141690853565967"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 691.7258261998494,
            "unit": "ns",
            "range": "± 8.727115559868459"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 682.1427980422974,
            "unit": "ns",
            "range": "± 6.408208168676356"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 907.0722662607828,
            "unit": "ns",
            "range": "± 7.028175077088352"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 677.6693011601766,
            "unit": "ns",
            "range": "± 8.796270488265721"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 708.847398349217,
            "unit": "ns",
            "range": "± 9.42799613861413"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 981.2475334167481,
            "unit": "ns",
            "range": "± 12.0161981244016"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 969.1957224332369,
            "unit": "ns",
            "range": "± 11.12078309127778"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 969.2789690835135,
            "unit": "ns",
            "range": "± 8.578826703962505"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 278396.9961983817,
            "unit": "ns",
            "range": "± 3144.826667764153"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 284678.2586588542,
            "unit": "ns",
            "range": "± 3154.8865325824154"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ce5a00f92c60543f217639775a74ffa357e970eb",
          "message": "Merge pull request #429 from nblumhardt/patch-1",
          "timestamp": "2021-02-18T00:54:52+02:00",
          "tree_id": "a663a6798b941732d275e89ae17f067956250cb3",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/ce5a00f92c60543f217639775a74ffa357e970eb"
        },
        "date": 1613603773556,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25581776.0875,
            "unit": "ns",
            "range": "± 265204.3152602532"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 660.3364789962768,
            "unit": "ns",
            "range": "± 4.470059503379937"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 671.7152695655823,
            "unit": "ns",
            "range": "± 7.2482753908929896"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 681.1928232737949,
            "unit": "ns",
            "range": "± 9.336505765512735"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 865.1585395812988,
            "unit": "ns",
            "range": "± 7.773836745379559"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 670.5874811319204,
            "unit": "ns",
            "range": "± 8.125694286911793"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 680.5063583510263,
            "unit": "ns",
            "range": "± 4.165145012476605"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 945.0359131949289,
            "unit": "ns",
            "range": "± 8.418095191052664"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 948.7847253254482,
            "unit": "ns",
            "range": "± 12.198359826698155"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 957.6361095428467,
            "unit": "ns",
            "range": "± 10.028906893140318"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 268228.034109933,
            "unit": "ns",
            "range": "± 2686.6752125778416"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 279841.34261067706,
            "unit": "ns",
            "range": "± 1574.1656795735403"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a2146e7b86017cbbbecfafd04ce1497e15753b0f",
          "message": "Merge pull request #433 from zjklee/issues/432\n\nFix child context creation when value is `null` when invoking partials",
          "timestamp": "2021-03-07T00:19:08+02:00",
          "tree_id": "9ebc68627fc23b842670f51b269154a089b69afd",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/a2146e7b86017cbbbecfafd04ce1497e15753b0f"
        },
        "date": 1615069451613,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 22478029.929166667,
            "unit": "ns",
            "range": "± 430545.4642381724"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 612.1351924260457,
            "unit": "ns",
            "range": "± 24.743112769915886"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 607.6808296203614,
            "unit": "ns",
            "range": "± 18.812965839276536"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 607.8265102386474,
            "unit": "ns",
            "range": "± 16.78499363448172"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 754.4605593045552,
            "unit": "ns",
            "range": "± 19.676483714415255"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 579.240347925822,
            "unit": "ns",
            "range": "± 20.10951077488145"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 596.8995667237502,
            "unit": "ns",
            "range": "± 10.367218466491948"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 852.4297406332834,
            "unit": "ns",
            "range": "± 18.228012797855698"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 836.5713718278067,
            "unit": "ns",
            "range": "± 15.32653172098234"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 856.9210077013288,
            "unit": "ns",
            "range": "± 21.44043666951781"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 274118.69306640624,
            "unit": "ns",
            "range": "± 8486.790616498409"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 241705.54321289062,
            "unit": "ns",
            "range": "± 6527.649606250094"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "95c787a185e54d2fb4eee25efd4014825eacdcaa",
          "message": "Merge pull request #436 from zjklee/fix/multiple-block-params\n\nFix multiple block params in the same template in different blocks",
          "timestamp": "2021-03-21T14:45:27+02:00",
          "tree_id": "4811f326d262df187b81bcb7212426306fc2a674",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/95c787a185e54d2fb4eee25efd4014825eacdcaa"
        },
        "date": 1616331041215,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19385319.435416665,
            "unit": "ns",
            "range": "± 828301.9064634563"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 518.4902978624616,
            "unit": "ns",
            "range": "± 12.660559537733983"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 543.0463656743367,
            "unit": "ns",
            "range": "± 40.19067460319065"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 579.3125210762024,
            "unit": "ns",
            "range": "± 32.14090770984754"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 704.0393073081971,
            "unit": "ns",
            "range": "± 26.666421957988405"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 536.1753165381296,
            "unit": "ns",
            "range": "± 15.169789344517634"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 527.9820722897847,
            "unit": "ns",
            "range": "± 23.77923349784145"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 768.2909335454304,
            "unit": "ns",
            "range": "± 40.04601161751011"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 744.3587540308635,
            "unit": "ns",
            "range": "± 20.467945875800922"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 784.7120342254639,
            "unit": "ns",
            "range": "± 35.7732457073661"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 229462.8174967448,
            "unit": "ns",
            "range": "± 12084.922313384053"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 240793.81708984374,
            "unit": "ns",
            "range": "± 17562.805992225203"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f2adea9b4a2561bdec0dd8bfea0bb72e2b9fc350",
          "message": "Merge pull request #438 from heemskerkerik/feature/fix-html-comment-as-layout",
          "timestamp": "2021-03-21T18:05:33+02:00",
          "tree_id": "473d6c4f5546ea8904d50fb2f6546adb8095ce8b",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/f2adea9b4a2561bdec0dd8bfea0bb72e2b9fc350"
        },
        "date": 1616343051962,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 23484932.535714287,
            "unit": "ns",
            "range": "± 60591.53424579331"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 650.1170993169148,
            "unit": "ns",
            "range": "± 32.7081064420912"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 628.2369129474347,
            "unit": "ns",
            "range": "± 1.1628733389344008"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 682.7584060668945,
            "unit": "ns",
            "range": "± 14.044562353088264"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 871.0431939760844,
            "unit": "ns",
            "range": "± 0.9837575635826136"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 643.0405593285194,
            "unit": "ns",
            "range": "± 1.3228179802209088"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 667.6105702718099,
            "unit": "ns",
            "range": "± 6.244951010134163"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 883.8502046267191,
            "unit": "ns",
            "range": "± 1.5162462603436888"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 870.9134222439358,
            "unit": "ns",
            "range": "± 1.9885242683269708"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 898.2625948832585,
            "unit": "ns",
            "range": "± 3.6151057105330398"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 273358.5104166667,
            "unit": "ns",
            "range": "± 7885.784947196799"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 286725.1134765625,
            "unit": "ns",
            "range": "± 7443.455660933802"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "2db65054feb25394770a849799c84bd5775216a4",
          "message": "Merge pull request #441 from zjklee/issues/439",
          "timestamp": "2021-03-23T23:56:07+02:00",
          "tree_id": "d00a90bac22bb52815786567d5aed9901c5059c3",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/2db65054feb25394770a849799c84bd5775216a4"
        },
        "date": 1616536901571,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 23671072.234375,
            "unit": "ns",
            "range": "± 61655.173237040995"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 640.2268013636271,
            "unit": "ns",
            "range": "± 20.42165758306436"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 658.7362235142634,
            "unit": "ns",
            "range": "± 3.084926082062226"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 632.783839566367,
            "unit": "ns",
            "range": "± 2.8086945327082127"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 864.9364878109524,
            "unit": "ns",
            "range": "± 1.418144973316534"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 633.8507867959829,
            "unit": "ns",
            "range": "± 1.0685233608318985"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 641.6952419916789,
            "unit": "ns",
            "range": "± 1.3254866761666644"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 875.6154758135477,
            "unit": "ns",
            "range": "± 0.5588793410314481"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 916.4152352469308,
            "unit": "ns",
            "range": "± 1.5525615185945263"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 906.8254563013712,
            "unit": "ns",
            "range": "± 2.17347802980352"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 277148.44862583705,
            "unit": "ns",
            "range": "± 866.5153371934418"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 288582.0443033854,
            "unit": "ns",
            "range": "± 537.4929548121985"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "41167595+probot-auto-merge[bot]@users.noreply.github.com",
            "name": "probot-auto-merge[bot]",
            "username": "probot-auto-merge[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "01ad0e69e56331371f85a1c59c008d7ecfd06f78",
          "message": "Merge pull request #443 from heemskerkerik/issues/440\n\nRender layouts using LayoutViewModel instead of DynamicViewModel",
          "timestamp": "2021-03-26T10:16:02Z",
          "tree_id": "64a432e27b021bfdc0bd58fa65697f1d23e5f7cf",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/01ad0e69e56331371f85a1c59c008d7ecfd06f78"
        },
        "date": 1616754072638,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 25556961.68125,
            "unit": "ns",
            "range": "± 205076.52343855725"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 632.9861302057902,
            "unit": "ns",
            "range": "± 6.654843313962429"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 671.7222473780314,
            "unit": "ns",
            "range": "± 13.181419842927808"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 666.0157056808472,
            "unit": "ns",
            "range": "± 4.848528211793209"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 854.8037636439005,
            "unit": "ns",
            "range": "± 8.20643861682191"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 670.1762642542521,
            "unit": "ns",
            "range": "± 11.93903660869173"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 659.2565800348917,
            "unit": "ns",
            "range": "± 7.629869766709978"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 944.8588298797607,
            "unit": "ns",
            "range": "± 6.404378944956234"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 994.8686173756918,
            "unit": "ns",
            "range": "± 7.911636688225971"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 951.8701061521258,
            "unit": "ns",
            "range": "± 8.829320716854035"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 271762.0411202567,
            "unit": "ns",
            "range": "± 2211.0210260090435"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 288416.9512765067,
            "unit": "ns",
            "range": "± 1978.5911169485275"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "1bc7e1ff9c14ae16524565a8edc9d4a590cf5ba2",
          "message": "Merge pull request #449 from bsagal/struct-collections",
          "timestamp": "2021-05-12T14:30:52-07:00",
          "tree_id": "c03342da9cdc3e362a44ccba663e33c9deb6cd8e",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/1bc7e1ff9c14ae16524565a8edc9d4a590cf5ba2"
        },
        "date": 1620855373126,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 23018075.7421875,
            "unit": "ns",
            "range": "± 206939.60543518516"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 618.9223837852478,
            "unit": "ns",
            "range": "± 5.925869454753022"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 627.6127208709717,
            "unit": "ns",
            "range": "± 3.082293294281394"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 652.1197736740112,
            "unit": "ns",
            "range": "± 13.973471492508759"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 829.9297097751072,
            "unit": "ns",
            "range": "± 4.1830646890556515"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 740.9697502576388,
            "unit": "ns",
            "range": "± 3.7185842991432168"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 624.90741964976,
            "unit": "ns",
            "range": "± 2.151686847869484"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 853.7259649912517,
            "unit": "ns",
            "range": "± 4.179508111503217"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 886.8207443873088,
            "unit": "ns",
            "range": "± 5.662128033802472"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 897.4654628208706,
            "unit": "ns",
            "range": "± 4.513641220970968"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 268388.2049386161,
            "unit": "ns",
            "range": "± 1701.0905767947922"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 274348.66813151044,
            "unit": "ns",
            "range": "± 1680.681399632635"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "9e9d60ed47ae2bbd6a94e8826a3f83b12f0cb22d",
          "message": "Merge pull request #451 from albertschulz/issues/448\n\nImprove performance for large arrays (10.000+)",
          "timestamp": "2021-06-02T11:30:09-07:00",
          "tree_id": "2fa161bddf88a0f7c43ff5a87778e6e86329d2e6",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/9e9d60ed47ae2bbd6a94e8826a3f83b12f0cb22d"
        },
        "date": 1622658994623,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 26039902.0125,
            "unit": "ns",
            "range": "± 456127.35676847183"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 693.1477076530457,
            "unit": "ns",
            "range": "± 14.003060748742298"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 683.5680577414377,
            "unit": "ns",
            "range": "± 9.272210897121976"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 694.1986987431844,
            "unit": "ns",
            "range": "± 10.670252413637176"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 885.2668676376343,
            "unit": "ns",
            "range": "± 12.728637865581621"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 700.7521623464731,
            "unit": "ns",
            "range": "± 7.689792738274014"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 706.70054063797,
            "unit": "ns",
            "range": "± 12.789954166904675"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 1018.8545162200928,
            "unit": "ns",
            "range": "± 23.93067515973045"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 995.9143802642823,
            "unit": "ns",
            "range": "± 14.291695698650406"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 974.8211306058444,
            "unit": "ns",
            "range": "± 13.224198637391487"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 283188.8631766183,
            "unit": "ns",
            "range": "± 5139.371655585329"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 287386.3008138021,
            "unit": "ns",
            "range": "± 7158.568762260367"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 4816202.972395834,
            "unit": "ns",
            "range": "± 89366.9213173841"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 10859860.230208334,
            "unit": "ns",
            "range": "± 184269.17794006865"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 24589073.166666668,
            "unit": "ns",
            "range": "± 318603.04950927664"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ad5c616c58397c6d52507c859cb9f1b397580e3a",
          "message": "Merge pull request #456 from zjklee/issues/452\n\nFix memory leak at Configuration subscribers",
          "timestamp": "2021-08-01T01:05:33-07:00",
          "tree_id": "1185bcf95ff342986fb3d9a14903875b8c7bff38",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/ad5c616c58397c6d52507c859cb9f1b397580e3a"
        },
        "date": 1627805508524,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 24776128.86875,
            "unit": "ns",
            "range": "± 620019.2806978031"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 602.4007506688436,
            "unit": "ns",
            "range": "± 23.92128298668459"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 627.8543393135071,
            "unit": "ns",
            "range": "± 36.50353314805767"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 660.590826511383,
            "unit": "ns",
            "range": "± 15.917219370278856"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 820.8048380851745,
            "unit": "ns",
            "range": "± 39.659182039799134"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 615.628271261851,
            "unit": "ns",
            "range": "± 18.88538378209421"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 602.3572552363078,
            "unit": "ns",
            "range": "± 15.402350239721148"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 865.7083689689637,
            "unit": "ns",
            "range": "± 23.320867522289042"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 918.642900126321,
            "unit": "ns",
            "range": "± 22.019532177021336"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 903.8873404775347,
            "unit": "ns",
            "range": "± 21.87438595516467"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 287239.06438802084,
            "unit": "ns",
            "range": "± 5875.953012674517"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 269608.2555013021,
            "unit": "ns",
            "range": "± 12082.2541158601"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 4340013.9328125,
            "unit": "ns",
            "range": "± 172504.9877832129"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 8417229.2734375,
            "unit": "ns",
            "range": "± 206459.1003383015"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 18898811.039583333,
            "unit": "ns",
            "range": "± 648715.0394387866"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8bf6990a3670aa8985a8b814b26adf7377df5d0b",
          "message": "Merge pull request #474 from jamesfarrugia96/master",
          "timestamp": "2021-12-07T11:18:10-08:00",
          "tree_id": "153b050d1947e8495cb1b2f92b9d388896f3f45d",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/8bf6990a3670aa8985a8b814b26adf7377df5d0b"
        },
        "date": 1638905063548,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 23001158.397916667,
            "unit": "ns",
            "range": "± 219388.13736539852"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 620.7968275887625,
            "unit": "ns",
            "range": "± 5.855380469292438"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 625.9471412499746,
            "unit": "ns",
            "range": "± 2.676933012329452"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 637.68997853597,
            "unit": "ns",
            "range": "± 5.566185131915716"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 841.2655670642853,
            "unit": "ns",
            "range": "± 6.174266056251172"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 634.4672972605779,
            "unit": "ns",
            "range": "± 6.036394741128606"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 632.2840429941813,
            "unit": "ns",
            "range": "± 1.6534177618631614"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 875.7609381357829,
            "unit": "ns",
            "range": "± 6.508111621526176"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 892.4298540115357,
            "unit": "ns",
            "range": "± 8.703065212799356"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 909.1568350110736,
            "unit": "ns",
            "range": "± 6.928526465156576"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 268744.8319963728,
            "unit": "ns",
            "range": "± 2140.220315445958"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 272298.6813151042,
            "unit": "ns",
            "range": "± 1299.9938716591323"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 4430355.66875,
            "unit": "ns",
            "range": "± 32692.40006379322"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 8937556.7375,
            "unit": "ns",
            "range": "± 13781.527490521526"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 17713808.59598214,
            "unit": "ns",
            "range": "± 57781.66724544543"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b32b3c0503935eddbabe78af54c47b7d6c53dc61",
          "message": "Merge pull request #477 from tommysor/UnencodedStatementVisitor_PreserveNoEscape\n\nUnencodedStatementVisitor resets value to previously",
          "timestamp": "2021-12-18T03:13:28-08:00",
          "tree_id": "2791b64ff1da8edf661047fa39b86bc24ecc00e6",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/b32b3c0503935eddbabe78af54c47b7d6c53dc61"
        },
        "date": 1639826384094,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 20207805.985416666,
            "unit": "ns",
            "range": "± 748668.6846829271"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 531.5835893630981,
            "unit": "ns",
            "range": "± 0.8084636684137286"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 531.0466078349522,
            "unit": "ns",
            "range": "± 2.3847865076951424"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 560.1091390337263,
            "unit": "ns",
            "range": "± 0.9299905481472306"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 698.4781952699026,
            "unit": "ns",
            "range": "± 21.87885547963943"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 474.22333695338324,
            "unit": "ns",
            "range": "± 0.5206821364956871"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 556.27607856478,
            "unit": "ns",
            "range": "± 0.9888181230509883"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 683.9082882563273,
            "unit": "ns",
            "range": "± 44.459258144516596"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 670.0170264561971,
            "unit": "ns",
            "range": "± 3.1671188766513145"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 722.0894463221232,
            "unit": "ns",
            "range": "± 1.8815183988550044"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 209220.15854492187,
            "unit": "ns",
            "range": "± 13566.092472607703"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 205255.7546183268,
            "unit": "ns",
            "range": "± 2791.3399281928887"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 3405374.0928485575,
            "unit": "ns",
            "range": "± 1367.1675390378357"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 7710308.213541667,
            "unit": "ns",
            "range": "± 7616.937877136009"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 13675242.864583334,
            "unit": "ns",
            "range": "± 47383.003249604495"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d30cdf32f92a5a6cf2b902e52012bc869fe98837",
          "message": "Merge pull request #478 from zjklee/issues/470\n\nFix path parsing when contains `[a/b]`",
          "timestamp": "2021-12-18T20:00:20-08:00",
          "tree_id": "a8a91f7b50a734d3fdd7069a249be5b9093a6b07",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/d30cdf32f92a5a6cf2b902e52012bc869fe98837"
        },
        "date": 1639886787737,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 22700575.425,
            "unit": "ns",
            "range": "± 422741.92448390165"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 652.3859461375645,
            "unit": "ns",
            "range": "± 7.183781403085773"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 654.203368695577,
            "unit": "ns",
            "range": "± 7.128845953411527"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 634.5694348335267,
            "unit": "ns",
            "range": "± 9.672827228529098"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 823.7998633702596,
            "unit": "ns",
            "range": "± 26.409863266146917"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 623.3598182678222,
            "unit": "ns",
            "range": "± 13.55997489713151"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 626.7600295884268,
            "unit": "ns",
            "range": "± 8.858387824346341"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 834.9024819056193,
            "unit": "ns",
            "range": "± 15.020666890980324"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 887.8180047353109,
            "unit": "ns",
            "range": "± 23.28185770960081"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 886.0183235168457,
            "unit": "ns",
            "range": "± 19.960666770820822"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 266832.1893554687,
            "unit": "ns",
            "range": "± 5489.833837491671"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 260850.81608072916,
            "unit": "ns",
            "range": "± 4428.43347854202"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 4427879.695833334,
            "unit": "ns",
            "range": "± 34545.0816050935"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 8568544.796875,
            "unit": "ns",
            "range": "± 90384.56989742737"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 17360282.14732143,
            "unit": "ns",
            "range": "± 173663.51384673337"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "41167595+probot-auto-merge[bot]@users.noreply.github.com",
            "name": "probot-auto-merge[bot]",
            "username": "probot-auto-merge[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "61c9580adb17532e278cddf6075e53dd9ce5d846",
          "message": "Merge pull request #481 from orgads/support-last\n\nSupport @last by default.",
          "timestamp": "2021-12-20T22:35:29Z",
          "tree_id": "18fd764efcd6a8917f23975754f1dfe74e06b604",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/61c9580adb17532e278cddf6075e53dd9ce5d846"
        },
        "date": 1640040105961,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19431210.59375,
            "unit": "ns",
            "range": "± 67784.80469919548"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 525.0244993845622,
            "unit": "ns",
            "range": "± 2.370281278345506"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 528.3503554207938,
            "unit": "ns",
            "range": "± 1.741276037789119"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 570.6520296096802,
            "unit": "ns",
            "range": "± 1.6130873039321043"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 709.5426307678223,
            "unit": "ns",
            "range": "± 4.174441066611344"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 556.326475756509,
            "unit": "ns",
            "range": "± 2.556822223219601"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 551.0131175994873,
            "unit": "ns",
            "range": "± 1.3464746367727018"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 747.8417216028486,
            "unit": "ns",
            "range": "± 2.7508000767380025"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 741.0382193156651,
            "unit": "ns",
            "range": "± 3.7010223207843254"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 768.356900024414,
            "unit": "ns",
            "range": "± 4.089300440095298"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 226166.3886343149,
            "unit": "ns",
            "range": "± 113.65085157069839"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 236405.5667255108,
            "unit": "ns",
            "range": "± 219.22510835812753"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 3760630.746651786,
            "unit": "ns",
            "range": "± 6083.350710902854"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 7494247.373325893,
            "unit": "ns",
            "range": "± 22839.30810168644"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 15239358.486607144,
            "unit": "ns",
            "range": "± 26665.400579813406"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "02e1794cf80a58f2f485d41a09253085b5d5c84c",
          "message": "Merge pull request #473 from tommysor/NoEscape_inconsistent_fix\n\nHtmlEncoding consistent with rules in handlebars.js",
          "timestamp": "2021-12-22T14:13:46-08:00",
          "tree_id": "58b0a9972441f9a6eb14448a17ca7e0ad45a6f45",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/02e1794cf80a58f2f485d41a09253085b5d5c84c"
        },
        "date": 1640211610877,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 17297807.71875,
            "unit": "ns",
            "range": "± 53848.16878692281"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 461.8115930897849,
            "unit": "ns",
            "range": "± 1.609707062148989"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 508.28649584452313,
            "unit": "ns",
            "range": "± 2.3453770433390813"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 484.48849391937256,
            "unit": "ns",
            "range": "± 2.523887851105392"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 626.0867013250079,
            "unit": "ns",
            "range": "± 1.3546699459109783"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 516.5042750676473,
            "unit": "ns",
            "range": "± 24.691909682088127"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 538.6686880405133,
            "unit": "ns",
            "range": "± 0.8164528261863887"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 653.6634817123413,
            "unit": "ns",
            "range": "± 1.3156046906717687"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 674.8964197452252,
            "unit": "ns",
            "range": "± 0.9540634143925492"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 671.7087674935659,
            "unit": "ns",
            "range": "± 1.0257698651239655"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 204549.65118815104,
            "unit": "ns",
            "range": "± 2270.880388759587"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 200264.4605102539,
            "unit": "ns",
            "range": "± 322.1721967317106"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 3727577.093489583,
            "unit": "ns",
            "range": "± 45827.20133529811"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 6813999.742708334,
            "unit": "ns",
            "range": "± 15882.907188204665"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 13483319.067708334,
            "unit": "ns",
            "range": "± 100472.95585688668"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "zjklee@gmail.com",
            "name": "Oleh Formaniuk",
            "username": "zjklee"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3b6c9c413a512c1c1c87b642229522a20b07fe5c",
          "message": "Publish package using ORG Nuget Secret",
          "timestamp": "2021-12-22T14:52:51-08:00",
          "tree_id": "307ab69d78c1dff31b3867d3080d3410eb5d70f5",
          "url": "https://github.com/Handlebars-Net/Handlebars.Net/commit/3b6c9c413a512c1c1c87b642229522a20b07fe5c"
        },
        "date": 1640213957799,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "HandlebarsNet.Benchmark.Compilation.Template",
            "value": 19759233.34375,
            "unit": "ns",
            "range": "± 90180.37062580055"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithoutParameters",
            "value": 528.087319056193,
            "unit": "ns",
            "range": "± 1.4442249504477167"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithOneParameter",
            "value": 533.2259363446917,
            "unit": "ns",
            "range": "± 1.0132464527067258"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallHelperWithTwoParameter",
            "value": 534.6112103462219,
            "unit": "ns",
            "range": "± 0.7542081434912336"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithoutParameters",
            "value": 700.5552235921224,
            "unit": "ns",
            "range": "± 1.1175379433325738"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithOneParameter",
            "value": 542.7686174099262,
            "unit": "ns",
            "range": "± 3.426295127367746"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.LateCallHelperWithTwoParameter",
            "value": 534.3428106307983,
            "unit": "ns",
            "range": "± 0.5494998300812075"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithoutParameters",
            "value": 742.7844414710999,
            "unit": "ns",
            "range": "± 0.6875598643733091"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithOneParameter",
            "value": 752.727730017442,
            "unit": "ns",
            "range": "± 1.2237252091648763"
          },
          {
            "name": "HandlebarsNet.Benchmark.Execution.CallBlockHelperWithTwoParameter",
            "value": 766.2073616186777,
            "unit": "ns",
            "range": "± 1.1493539338698293"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"dictionary\")",
            "value": 227142.21953473773,
            "unit": "ns",
            "range": "± 340.6236035610881"
          },
          {
            "name": "HandlebarsNet.Benchmark.EndToEnd.Default(N: 5, DataType: \"object\")",
            "value": 231904.52273995537,
            "unit": "ns",
            "range": "± 305.1483883625834"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 20000)",
            "value": 3359407.3878348214,
            "unit": "ns",
            "range": "± 781.5176966793434"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 40000)",
            "value": 7581326.9625,
            "unit": "ns",
            "range": "± 7545.715068595806"
          },
          {
            "name": "HandlebarsNet.Benchmark.LargeArray.Default(N: 80000)",
            "value": 15073261.494419644,
            "unit": "ns",
            "range": "± 25165.831709867285"
          }
        ]
      }
    ]
  }
}