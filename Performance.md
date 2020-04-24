#Performance

### Setup

#### Template

```handlebars
childCount={{level1.Count}}
childCount2={{level1.Count}}
{{#each level1}}
    id={{id}}
    childCount={{level2.Count}}
    childCount2={{level2.Count}}
    index=[{{@../../index}}:{{@../index}}:{{@index}}]
    first=[{{@../../first}}:{{@../first}}:{{@first}}]
    last=[{{@../../last}}:{{@../last}}:{{@last}}]
    {{#each level2}}
        id={{id}}
        childCount={{level3.Count}}
        childCount2={{level3.Count}}
        index=[{{@../../index}}:{{@../index}}:{{@index}}]
        first=[{{@../../first}}:{{@../first}}:{{@first}}]
        last=[{{@../../last}}:{{@../last}}:{{@last}}]
        {{#each level3}}
            id={{id}}
            index=[{{@../../index}}:{{@../index}}:{{@index}}]
            first=[{{@../../first}}:{{@../first}}:{{@first}}]
            last=[{{@../../last}}:{{@../last}}:{{@last}}]
        {{/each}}
    {{/each}}    
{{/each}}
```

#### Input example

```json
{
  "level1": [
    {
      "id": "0",
      "level2": [
        {
          "id": "0-0",
          "level3": [
            {
              "id": "0-0-0"
            }
          ]
        }
      ]
    }
  ]
}
```

#### Environment

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.973 (1809/October2018Update/Redstone5), VM=Hyper-V
Intel Xeon CPU E5-2640 v3 2.60GHz, 1 CPU, 16 logical and 16 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), X64 RyuJIT
  Job-GBTPJU : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT

Runtime=.NET Core 3.1  

```

#### Legend

##### Version
`1.10.1` - Handlebars.Net version

`current` - current handlebars.csharp version

`current-cache` - current handlebars.csharp version with aggressive caching (`Default`) 

`current-fast` - current handlebars.csharp version with `CompileFast` feature

`current-fast-cache` - current handlebars.csharp version with `CompileFast` feature and aggressive caching

##### N

Number of items on each level of the input.

### Results

#### Compilation

|   Method |      Version |      Mean |     Error |    StdDev |    Gen 0 |    Gen 1 | Gen 2 | Allocated |
|--------- |------------- |----------:|----------:|----------:|---------:|---------:|------:|----------:|
| **Template** |       **1.10.1** | **26.270 ms** | **0.4970 ms** | **0.4406 ms** | **375.0000** | **187.5000** |     **-** | **3900.8 KB** |
| **Template** |      **current** | **13.723 ms** | **0.2681 ms** | **0.2238 ms** |  **62.5000** |  **31.2500** |     **-** | **735.62 KB** |
| **Template** | **current-fast** |  **3.078 ms** | **0.0615 ms** | **0.0862 ms** |  **78.1250** |  **39.0625** |     **-** | **804.66 KB** |

#### Execution

|          Method |  N |            Version |   DataType |         Mean |      Error |     StdDev |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|---------------- |--- |------------------- |----------- |-------------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
| **WithParentIndex** |  **2** |             **1.10.1** | **dictionary** |    **385.33 us** |   **6.026 us** |   **5.342 us** |  **11.7188** |   **0.4883** |        **-** |  **120.58 KB** |
| **WithParentIndex** |  **2** |             **1.10.1** |     **object** |    **363.27 us** |   **7.111 us** |   **6.984 us** |   **9.7656** |   **0.4883** |        **-** |  **102.74 KB** |
| **WithParentIndex** |  **2** |            **current** | **dictionary** |     **49.35 us** |   **0.973 us** |   **1.396 us** |   **1.0376** |        **-** |        **-** |   **11.19 KB** |
| **WithParentIndex** |  **2** |            **current** |     **object** |     **63.92 us** |   **1.221 us** |   **1.454 us** |   **1.0986** |        **-** |        **-** |   **11.61 KB** |
| **WithParentIndex** |  **2** |      **current-cache** | **dictionary** |     **45.84 us** |   **0.810 us** |   **0.718 us** |   **1.0376** |        **-** |        **-** |   **11.19 KB** |
| **WithParentIndex** |  **2** |      **current-cache** |     **object** |     **54.90 us** |   **1.509 us** |   **1.338 us** |   **1.0986** |        **-** |        **-** |   **11.61 KB** |
| **WithParentIndex** |  **2** |       **current-fast** | **dictionary** |     **52.31 us** |   **1.005 us** |   **1.307 us** |   **1.0376** |        **-** |        **-** |   **11.19 KB** |
| **WithParentIndex** |  **2** |       **current-fast** |     **object** |     **64.17 us** |   **1.238 us** |   **1.325 us** |   **1.0986** |        **-** |        **-** |   **11.61 KB** |
| **WithParentIndex** |  **2** | **current-fast-cache** | **dictionary** |     **43.94 us** |   **0.863 us** |   **0.765 us** |   **1.0376** |        **-** |        **-** |   **11.19 KB** |
| **WithParentIndex** |  **2** | **current-fast-cache** |     **object** |     **53.77 us** |   **0.840 us** |   **0.744 us** |   **1.0986** |        **-** |        **-** |   **11.61 KB** |


|          Method |  N |            Version |   DataType |         Mean |      Error |     StdDev |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|---------------- |--- |------------------- |----------- |-------------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
| **WithParentIndex** |  **5** |             **1.10.1** | **dictionary** |  **3,436.11 us** |  **68.128 us** |  **88.585 us** | **101.5625** |  **23.4375** |        **-** | **1111.08 KB** |
| **WithParentIndex** |  **5** |             **1.10.1** |     **object** |  **3,492.48 us** |  **68.605 us** |  **93.907 us** |  **93.7500** |  **23.4375** |        **-** |  **1002.9 KB** |
| **WithParentIndex** |  **5** |            **current** | **dictionary** |    **369.02 us** |   **7.016 us** |   **6.563 us** |   **9.7656** |   **1.4648** |        **-** |  **103.43 KB** |
| **WithParentIndex** |  **5** |            **current** |     **object** |    **496.41 us** |   **9.794 us** |  **14.659 us** |   **9.7656** |   **0.9766** |        **-** |  **105.54 KB** |
| **WithParentIndex** |  **5** |      **current-cache** | **dictionary** |    **356.12 us** |   **6.967 us** |   **7.744 us** |   **9.7656** |   **1.4648** |        **-** |  **103.43 KB** |
| **WithParentIndex** |  **5** |      **current-cache** |     **object** |    **418.22 us** |   **7.863 us** |   **8.075 us** |  **10.2539** |   **1.4648** |        **-** |  **105.54 KB** |
| **WithParentIndex** |  **5** |       **current-fast** | **dictionary** |    **375.19 us** |   **6.064 us** |   **5.672 us** |   **9.7656** |   **1.4648** |        **-** |  **103.43 KB** |
| **WithParentIndex** |  **5** |       **current-fast** |     **object** |    **479.95 us** |   **9.304 us** |   **8.248 us** |   **9.7656** |   **0.9766** |        **-** |  **105.54 KB** |
| **WithParentIndex** |  **5** | **current-fast-cache** | **dictionary** |    **350.70 us** |   **7.537 us** |   **7.050 us** |   **9.7656** |   **1.4648** |        **-** |  **103.43 KB** |
| **WithParentIndex** |  **5** | **current-fast-cache** |     **object** |    **403.11 us** |   **6.821 us** |   **6.381 us** |  **10.2539** |   **1.4648** |        **-** |  **105.54 KB** |


|          Method |  N |            Version |   DataType |         Mean |      Error |     StdDev |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|---------------- |--- |------------------- |----------- |-------------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
| **WithParentIndex** | **10** |             **1.10.1** | **dictionary** | **25,667.09 us** | **572.214 us** | **723.667 us** | **687.5000** | **250.0000** |  **93.7500** | **7482.11 KB** |
| **WithParentIndex** | **10** |             **1.10.1** |     **object** | **22,741.41 us** | **437.277 us** | **486.033 us** | **656.2500** | **218.7500** | **125.0000** | **6919.99 KB** |
| **WithParentIndex** | **10** |            **current** | **dictionary** |  **2,513.83 us** |  **50.250 us** |  **53.767 us** | **136.7188** | **109.3750** | **109.3750** |  **701.46 KB** |
| **WithParentIndex** | **10** |            **current** |     **object** |  **2,827.85 us** |  **55.088 us** |  **77.226 us** | **136.7188** | **109.3750** | **109.3750** |  **709.18 KB** |
| **WithParentIndex** | **10** |      **current-cache** | **dictionary** |  **2,203.95 us** |  **43.770 us** |  **48.650 us** | **136.7188** | **109.3750** | **109.3750** |  **701.43 KB** |
| **WithParentIndex** | **10** |      **current-cache** |     **object** |  **2,530.40 us** |  **49.594 us** |  **81.485 us** | **136.7188** | **109.3750** | **109.3750** |  **709.24 KB** |
| **WithParentIndex** | **10** |       **current-fast** | **dictionary** |  **2,264.14 us** |  **45.091 us** |  **83.578 us** | **136.7188** | **109.3750** | **109.3750** |  **701.53 KB** |
| **WithParentIndex** | **10** |       **current-fast** |     **object** |  **2,906.42 us** |  **57.859 us** |  **91.770 us** | **136.7188** | **109.3750** | **109.3750** |  **709.22 KB** |
| **WithParentIndex** | **10** | **current-fast-cache** | **dictionary** |  **2,164.81 us** |  **40.893 us** |  **45.452 us** | **136.7188** | **109.3750** | **109.3750** |  **701.49 KB** |
| **WithParentIndex** | **10** | **current-fast-cache** |     **object** |  **2,497.93 us** |  **49.739 us** |  **88.412 us** | **136.7188** | **109.3750** | **109.3750** |  **709.18 KB** |