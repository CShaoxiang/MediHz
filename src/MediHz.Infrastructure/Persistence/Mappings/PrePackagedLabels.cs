namespace MediHz.Infrastructure.Persistence.Mappings;

internal static class PrePackagedLabels
{
    public static readonly DateTimeOffset SeededAtUtc = DateTimeOffset.UnixEpoch;

    public static readonly Guid CirCorGroupId = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid HfLungGroupId = new("00000000-0000-0000-0000-000000000002");

    public static readonly Guid CirCorS1Id = new("00000000-0000-0000-0000-000000000101");
    public static readonly Guid CirCorSystoleId = new("00000000-0000-0000-0000-000000000102");
    public static readonly Guid CirCorS2Id = new("00000000-0000-0000-0000-000000000103");
    public static readonly Guid CirCorDiastoleId = new("00000000-0000-0000-0000-000000000104");
    public static readonly Guid CirCorUnknownId = new("00000000-0000-0000-0000-000000000105");
    public static readonly Guid CirCorSystolicMurmurId = new(
        "00000000-0000-0000-0000-000000000106"
    );
    public static readonly Guid CirCorDiastolicMurmurId = new(
        "00000000-0000-0000-0000-000000000107"
    );

    public static readonly Guid HfLungInhalationId = new("00000000-0000-0000-0000-000000000201");
    public static readonly Guid HfLungExhalationId = new("00000000-0000-0000-0000-000000000202");
    public static readonly Guid HfLungWheezeId = new("00000000-0000-0000-0000-000000000203");
    public static readonly Guid HfLungStridorId = new("00000000-0000-0000-0000-000000000204");
    public static readonly Guid HfLungRhonchiId = new("00000000-0000-0000-0000-000000000205");
    public static readonly Guid HfLungDasId = new("00000000-0000-0000-0000-000000000206");
}
