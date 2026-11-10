using AquaGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AquaGuard.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AquaGuardDbContext))]
[Migration("202611090002_MeasurementReceipts")]
public sealed class MeasurementReceipts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("measurement_receipts", table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            SourceEventId = table.Column<Guid>(type: "uuid", nullable: false),
            ReservoirId = table.Column<Guid>(type: "uuid", nullable: false),
            Source = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
            RawPayload = table.Column<string>(type: "jsonb", nullable: false),
            Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
            NumericValue = table.Column<double>(type: "double precision", nullable: true),
            RawValue = table.Column<string>(type: "text", nullable: true),
            Unit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
            MeasuredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            AccumulationStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            AccumulationEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            ValidationStatus = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
            ValidationMessages = table.Column<string>(type: "jsonb", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_measurement_receipts", x => x.Id);
            table.ForeignKey("FK_measurement_receipts_reservoirs_ReservoirId", x => x.ReservoirId,
                "reservoirs", "Id", onDelete: ReferentialAction.Cascade);
        });
        migrationBuilder.CreateIndex("IX_measurement_receipts_SourceEventId", "measurement_receipts", "SourceEventId", unique: true);
        migrationBuilder.CreateIndex("IX_measurement_receipts_ReservoirId_Kind_MeasuredAt", "measurement_receipts",
            new[] { "ReservoirId", "Kind", "MeasuredAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("measurement_receipts");
}
