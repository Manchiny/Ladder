using System;
using System.Collections.Generic;

public class WindowsHolder
{
    public static readonly Dictionary<Type, string> Windows = new Dictionary<Type, string>
    {
        [typeof(HoldAndReleaseWindow)] = "Windows/HoldAndReleaseWindow",
        [typeof(LevelStartWindow)] = "Windows/LevelStartWindow",
        [typeof(TapToCatchWindow)] = "Windows/TapToCatchWindow",
        [typeof(LevelCompleteWindow)] = "Windows/LevelCompleteWindow",
        [typeof(YouAreTiredWindow)] = "Windows/YouAreTiredWindow",
    };
}

