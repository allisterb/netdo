namespace DigitalOcean.Cli;

using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Jint;
using Spectre.Console;
using NTokenizers.Extensions.Spectre.Console;
using NTokenizers.Extensions.Spectre.Console.Styles;
using System.Linq;
using System.Text;

public class JSInterp : Runtime
{    
    public static string[] ExtractJSFromMarkdown(string src)
    {
        var matches = Regex.Matches(src, @"```javascript\s+(.*?)\s+```", RegexOptions.Singleline);
        var list = new List<string>();
        foreach (Match match in matches)
        {
            list.Add(match.Groups[1].Value);
        }
        return list.ToArray();
    }

    public static void ConsoleInfo(object o)
    {
        Info(o?.ToString() ?? "");
        switch (o)
        {
            case string str:
                AnsiConsole.WriteLine(str);
                break;

            default:
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
                AnsiConsole.Console.WriteJson(json);
                break;
        }
        AnsiConsole.WriteLine(Environment.NewLine);
    }

    public static void ConsoleError(object o)
    {
        var s = o.ToString() + Environment.NewLine;
        Error(s);
        AnsiConsole.Write(new Markup(s, new Style(foreground: Color.Red)));
    }

    public static void ConsoleExecute(string src)
    {
        var jsoptions = new Jint.Options();        
        jsoptions.Host.StringCompilationAllowed = false;
        var engine = new Engine(jsoptions)
            
            .SetValue("log", ConsoleInfo)
            .SetValue("error", ConsoleError)
            .SetValue("confirm", ConsoleConfirm)
            .SetValue("ask", ConsoleAsk)
            .SetValue("select", ConsoleSelect)
            .SetValue("table", ConsoleDrawTable)
            .SetValue("api", new DonnaApi());
        engine.Execute(src);
    }

    public static string MCPExecute(string src)
    {
        var output = new StringBuilder();
        var jsoptions = new Jint.Options();
        jsoptions.Host.StringCompilationAllowed = false;
        var engine = new Engine(jsoptions)
            .SetValue("log", new Action<string>((s) => output.AppendLine(s)))
            .SetValue("error", new Action<string>((s) => output.AppendLine(s)))
            .SetValue("confirm", new Action(() => throw new NotSupportedException("User inputis not supported in the MCP server")))
            .SetValue("ask", new Action(() => throw new NotSupportedException("User input is not supported in the MCP server")))
            .SetValue("select", new Action(() => throw new NotSupportedException("User input function is not supported in the MCP server")))
            .SetValue("table", new Action<string[], object[][]>((headers, dataRows) =>
            {
                output.AppendLine(headers.ToString());

            }))
            .SetValue("api", new DonnaApi());
        engine.Execute(src);
        return output.ToString();
    }

    public static bool ConsoleConfirm(string msg) => AnsiConsole.Confirm(msg);

    public static string ConsoleAsk(string prompt) => AnsiConsole.Ask<string>(prompt);

    public static string ConsoleSelect(string title, params string[] choices) => AnsiConsole.Prompt(new SelectionPrompt<string>().Title(title).AddChoices(choices));

    public static void ConsoleDrawTable(string[] headers, object[][] dataRows)
    {
        var table = new Table();
        var headerStyle = new Style(foreground: Color.Aqua, decoration: Decoration.Bold | Decoration.Underline);
        foreach (var header in headers)
        {
            table.AddColumn(new TableColumn(new Markup(header, headerStyle)).Alignment(Justify.Center));
        }
        foreach (var row in dataRows)
        {            
            if (row.Length != headers.Length)
            {
                AnsiConsole.MarkupLine($"[yellow]Warning: Skipping a row due to column count mismatch.[/]");
                continue;
            }
            table.AddRow(row.Select(c => c?.ToString() ?? "").ToArray());
        }

        AnsiConsole.Write(table);
    }   
}
