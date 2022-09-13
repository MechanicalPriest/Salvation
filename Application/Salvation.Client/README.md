## Application Insights

Application Insights support is added using https://github.com/IvanJosipovic/BlazorApplicationInsights.

It's a wrapper for https://github.com/microsoft/ApplicationInsights-JS.

You can send custom data to the `customEvents` stream using:

```
await _appInsights.TrackEvent( ... );
```

Exceptions to the `exceptions` stream can be sent with:

```
Error error = new()
{
    Message = ex.Message,
    Stack = ex.StackTrace
};

await _appInsights.TrackException(error);
```