namespace ServiceNowAutomation.Helpers;

public static class OutputPathBuilder
{
    public static string Build(string baseDir, string filePrefix)
    {
        //var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //return Path.Combine(baseDir, $"{filePrefix}_{ts}.xlsx");

        return Path.Combine(baseDir, $"{filePrefix}.xlsx");
    }
}
