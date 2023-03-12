string GetData(TimeSpan ts)
{
    return ts.TotalHours.ToString().Split('.')[0] + ":" + ts.Minutes + ":" + ts.Seconds;
}
TimeSpan TSAvg(IEnumerable<TimeSpan> spans)
{
    return TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
}
Dictionary<string, Dictionary<DateOnly, Dictionary<string, TimeSpan>>> dctTimesheet = new Dictionary<string, Dictionary<DateOnly, Dictionary<string, TimeSpan>>>();
using (StreamReader streamReader = new StreamReader("../../../Large_Input.txt"))
{
    int n = Convert.ToInt32(streamReader.ReadLine());
    while (n-- > 0)
    {
        string line = streamReader.ReadLine();
        if (line == null)
            continue;
        string[] data = line.Split(" ").Where(l => l.Trim().Length > 0).ToArray();
        if (!dctTimesheet.ContainsKey(data[0]))
            dctTimesheet.Add(data[0], new Dictionary<DateOnly, Dictionary<string, TimeSpan>>());
        if (!dctTimesheet[data[0]].ContainsKey(DateOnly.Parse(data[1])))
            dctTimesheet[data[0]].Add(DateOnly.Parse(data[1]), new Dictionary<string, TimeSpan>());
        dctTimesheet[data[0]][DateOnly.Parse(data[1])].Add(data[3], TimeSpan.Parse(data[2]));
    }
    List<Dictionary<string, Dictionary<DateOnly, Dictionary<string, TimeSpan>>>> lstGMonth = new List<Dictionary<string, Dictionary<DateOnly, Dictionary<string, TimeSpan>>>>();
    Dictionary<string, Dictionary<string, TimeSpan>> dctFinal = new Dictionary<string, Dictionary<string, TimeSpan>>();
    foreach (KeyValuePair<string, Dictionary<DateOnly, Dictionary<string, TimeSpan>>> emp in dctTimesheet)
    {
        TimeSpan tTime = TimeSpan.MinValue;
        foreach (KeyValuePair<DateOnly, Dictionary<string, TimeSpan>> time in emp.Value)
        {
            string MY = time.Key.ToString("MM/yyyy");
            if (!dctFinal.ContainsKey(MY))
                dctFinal.Add(MY, new Dictionary<string, TimeSpan>());
            TimeSpan lastTime;
            if (time.Value.ContainsKey("clock-out"))
                lastTime = time.Value["clock-out"];
            else
            {
                if (time.Value["clock-in"].Add(new TimeSpan(6, 0, 0)) < new TimeSpan(19, 30, 0))
                    lastTime = time.Value["clock-in"].Add(new TimeSpan(6, 0, 0));
                else
                    lastTime = new TimeSpan(19, 30, 0);
            }
            TimeSpan ts = lastTime - time.Value["clock-in"] - (time.Value["break-stop"] - time.Value["break-start"]);
            if (!dctFinal[MY].ContainsKey(emp.Key))
                dctFinal[MY][emp.Key] = ts;
            else
                dctFinal[MY][emp.Key] += ts;
        }
    }
    int Count = 1;
    foreach (KeyValuePair<string, Dictionary<string, TimeSpan>> kvp in dctFinal)
    {
        Dictionary<string, TimeSpan> dctMY = kvp.Value;
        Console.WriteLine("Case #" + Count++ + ": " + GetData(dctMY.Values.Max()) + " " + GetData(dctMY.Values.Min()) + " " + GetData(TSAvg(dctMY.Values)));
    }
}
Console.ReadLine();