using AquaGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AquaGuard.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AquaGuardDbContext))]
[Migration("202611090001_InitialPropertyAndReservoir")]
public sealed class InitialPropertyAndReservoir : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("properties", table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_properties", x => x.Id));

        migrationBuilder.CreateTable("reservoirs", table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
            Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
            LevelReferenceLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_reservoirs", x => x.Id);
            table.ForeignKey("FK_reservoirs_properties_PropertyId", x => x.PropertyId,
                "properties", "Id", onDelete: ReferentialAction.Cascade);
        });
        migrationBuilder.CreateIndex("IX_reservoirs_PropertyId", "reservoirs", "PropertyId", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("reservoirs");
        migrationBuilder.DropTable("properties");
    }
}
