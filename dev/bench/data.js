window.BENCHMARK_DATA = {
  "lastUpdate": 1603722401803,
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
      }
    ]
  }
}