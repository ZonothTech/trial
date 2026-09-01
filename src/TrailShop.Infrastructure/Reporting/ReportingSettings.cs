namespace TrailShop.Infrastructure.Reporting;

/// <summary>
/// Settings for the (fictional) reporting database. The host name uses the
/// reserved .invalid TLD, so nothing in this training app can ever connect to it.
/// </summary>
public sealed record ReportingSettings(string ConnectionString);
