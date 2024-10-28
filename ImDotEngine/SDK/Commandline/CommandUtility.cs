using System;
using System.Collections.Generic;
using System.Globalization;

class CommandUtility
{
    private readonly Dictionary<string, string> _arguments = new Dictionary<string, string>(); // i wonder if versions higher has that

    public CommandUtility(string[] args)
    {
        _arguments.Clear();

        for (int i = 0; i < args.Length; ++i)
        {
            string arg = args[i];

            if (arg.StartsWith("--"))
            {
                string key = arg.TrimStart('-').ToLower();

                if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                {
                    _arguments[key] = args[i+1];
                    ++i;
                }
                else _arguments[key] = null;
            }
        }
    }

    public bool HasFlag(string key) => _arguments.ContainsKey(key);

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (_arguments.TryGetValue(key.ToLower(), out string value))
            return value?.ToLower() == "true";

        return defaultValue;
    }

    public float GetNumber(string key, float defaultValue = -1)
    {
        if (_arguments.TryGetValue(key.ToLower(), out string value) &&
            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)
        )
            return result;

        return defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        if (_arguments.TryGetValue(key.ToLower(), out string value))
            return value;

        return defaultValue;
    }
}