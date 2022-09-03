using CarpetMigrator.Models;
using CommandLine;

Parser
    .Default
    .ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if (options.Host == null)
        {
            throw new ArgumentNullException(nameof(options.Host));
        }

        if (options.Database == null)
        {
            throw new ArgumentNullException(nameof(options.Database));
        }

        if (options.User == null)
        {
            throw new ArgumentNullException(nameof(options.User));
        }

        if (options.Password == null)
        {
            throw new ArgumentNullException(nameof(options.Password));
        }

        // TODO: TEST IF SOURCE CONNECTION WORKS
        using var sourceContext = new MySqlCarpetDataContext(options.Host, options.Database, options.User, options.Password);

        var fullTargetPath = Path.IsPathFullyQualified(options.Target)
            ? options.Target
            : Path.Combine(Environment.CurrentDirectory, options.Target);

        if (!File.Exists(fullTargetPath))
        {
            throw new InvalidOperationException($"Target file ({fullTargetPath}) doesn't exist.");
        }

        using var targetContext = new SqliteCarpetDataContext(fullTargetPath);

        if (targetContext.Carpets.Any() || targetContext.Colors.Any() || targetContext.Stripes.Any())
        {
            throw new InvalidOperationException("Target has existing entities!");
        }

        var aliases = targetContext
            .Aliases
            .ToDictionary(alias => alias.Alias, alias => alias.ObjectId);

        foreach (var color in sourceContext.Colors)
        {
            targetContext.Colors.Add(color);
        }

        foreach (var carpet in sourceContext.Carpets)
        {
            targetContext.Add(new SqliteCarpetEntity
            {
                Id = carpet.Id,
                Name = carpet.Name,
                Owner = aliases.TryGetValue(carpet.Username, out var objectId)
                    ? objectId
                    : carpet.Username,
                Removed = carpet.Removed,
                Width = carpet.Width,
                StripeSeparator = carpet.StripeSeparator
            });
        }

        foreach (var stripe in sourceContext.Stripes)
        {
            targetContext.Add(stripe);
        }

        targetContext.SaveChanges();
    });

public class Options
{
    [Option('h', "host", Default = "localhost", Required = false, HelpText = "Source MySQL host address")]
    public string? Host { get; set; }

    [Option('d', "database", Default = "CarpetSQL", Required = false, HelpText = "Source MySQL database name")]
    public string? Database { get; set; }

    [Option('u', "user", Default = "", Required = false, HelpText = "Source MySQL username")]
    public string? User { get; set; }

    [Option('p', "password", Required = true, HelpText = "Source MySQL user password")]
    public string? Password { get; set; }

    [Option('t', "target", Required = true, HelpText = "Target SQLite filename or path")]
    public string? Target { get; set; }

    [Option("onlyColors", Required = false, HelpText = "Whether to migrate only Colors table content")]
    public bool OnlyColors { get; set; }
}
