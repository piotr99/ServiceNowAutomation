namespace ServiceNowAutomation.Helpers;

public static class OutputPathBuilder
{
    public static string Build(string baseDir, string filePrefix)
    {
<<<<<<< HEAD
        //var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //return Path.Combine(baseDir, $"{filePrefix}_{ts}.xlsx");

        return Path.Combine(baseDir, $"{filePrefix}.xlsx");
=======
        var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(baseDir, $"{filePrefix}_{ts}.xlsx");
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
    }
}
