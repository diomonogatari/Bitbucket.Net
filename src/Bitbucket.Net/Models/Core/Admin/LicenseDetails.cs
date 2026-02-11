using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Admin;

public class LicenseDetails : LicenseInfo
{
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreationDate { get; init; }
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? PurchaseDate { get; init; }
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? ExpiryDate { get; init; }
    public int NumberOfDaysBeforeExpiry { get; init; }
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? MaintenanceExpiryDate { get; init; }
    public int NumberOfDaysBeforeMaintenanceExpiry { get; init; }
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? GracePeriodEndDate { get; init; }
    public int NumberOfDaysBeforeGracePeriodExpiry { get; init; }
    public int MaximumNumberOfUsers { get; init; }
    public bool UnlimitedNumberOfUsers { get; init; }
    public string? ServerId { get; init; }
    public string? SupportEntitlementNumber { get; init; }
    public LicenseStatus? Status { get; init; }
}