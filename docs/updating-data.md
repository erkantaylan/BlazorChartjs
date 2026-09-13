# Updating data

## Add new values

When a graph is created, it means that the configuration is already defined and the datasets are passed to the chart engine. Without recreating the graph, it is possible to add a new value to a specific dataset and/or add a completely new dataset to the graph.

On your page, create a new chart by adding this code

```
<Chart Config="_config1" @ref="_chart1"></Chart>
```

In the code section you have to define the variables:

```csharp
private LineChartConfig _config1;
private Chart _chart1;
```

_chart1_ is the reference to the `Chart` component and from it, you can access all the functions and properties the component has to offer.

### Add a new value

In an existing graph, it is possible to add a single new value to a specific dataset calling `AddData` function that is available on the chart.

Now, the function `AddData` allows to add a new value in a specific existing dataset. The definition of `AddData` is the following

```csharp
Task AddData(List<string?>? labels, int datasetIndex, List<decimal?>? data)
```

For example, using __chart1_, the following code adds a new label `Test1` to the list of labels, and for the dataset _0_ adds a random number.

```csharp
await _chart1.AddData(new List<string?>() { "Test1" }, 0, new List<decimal?>() { rd.Next(0, 200) });
```

`AddData` returns a `Task` — await it. The whole batch is appended in one round trip and the chart is redrawn once, so passing several labels and values at a time is cheaper than calling it in a loop. Pass `null` for `labels` to append values without touching the labels. The call is a no-op if the chart has not rendered yet.

The result is visible in the following screenshot.

![chart-addnewdata](https://user-images.githubusercontent.com/9497415/229902251-8a2adf61-b37c-4fdc-a869-ca8eb1a7cd81.gif)

### Add a new dataset

It is also possible to add a completely new dataset to the graph. For that, there is the function `AddDataset`. This function requires a new dataset of the same format as the others already existing in the chart.

For example, this code adds a new dataset using `LineDataset` using some of the properties this dataset has.

```csharp
private async Task AddNewDataset()
{
    Random rd = new Random();
    List<decimal?> addDS = new List<decimal?>();
    for (int i = 0; i < 8; i++)
    {
        addDS.Add(rd.Next(i, 200));
    }

    var color = String.Format("#{0:X6}", rd.Next(0x1000000));

    await _chart1.AddDataset<LineDataset>(new LineDataset()
        {
            Label = $"Dataset {DateTime.Now}",
            Data = addDS,
            BorderColor = color,
            Fill = false
        });
}
```

The result of this code is the following screenshot.

![chart-addnewdataset](https://user-images.githubusercontent.com/9497415/229904537-22805b25-747f-4020-9eed-51533183324c.gif)

### Remove every value

`ClearData` empties the labels and the data of every dataset, keeping the datasets and the chart configuration in place.

```csharp
await _chart1.ClearData();
```

> `AddData`, `AddDataset<T>` and `ClearData` all return `Task`. In upstream they were `async void`, which meant an exception inside them could not be caught and the caller had no way to know when the chart had finished updating.
