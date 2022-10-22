using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaPl.SKS.DataAccess.Sql.Migrations
{
    [ExcludeFromCodeCoverage]

    public partial class Howler : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hop",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HopType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessingDelayMins = table.Column<int>(type: "int", nullable: true),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hop", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Recipient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcel",
                columns: table => new
                {
                    TrackingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: true),
                    RecipientId = table.Column<int>(type: "int", nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    State = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcel", x => x.TrackingId);
                    table.ForeignKey(
                        name: "FK_Parcel_Recipient_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parcel_Recipient_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Recipient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HopArrival",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParcelTrackingId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParcelTrackingId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopArrival", x => x.Code);
                    table.ForeignKey(
                        name: "FK_HopArrival_Parcel_ParcelTrackingId",
                        column: x => x.ParcelTrackingId,
                        principalTable: "Parcel",
                        principalColumn: "TrackingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HopArrival_Parcel_ParcelTrackingId1",
                        column: x => x.ParcelTrackingId1,
                        principalTable: "Parcel",
                        principalColumn: "TrackingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HopArrival_ParcelTrackingId",
                table: "HopArrival",
                column: "ParcelTrackingId");

            migrationBuilder.CreateIndex(
                name: "IX_HopArrival_ParcelTrackingId1",
                table: "HopArrival",
                column: "ParcelTrackingId1");

            migrationBuilder.CreateIndex(
                name: "IX_Parcel_RecipientId",
                table: "Parcel",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcel_SenderId",
                table: "Parcel",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hop");

            migrationBuilder.DropTable(
                name: "HopArrival");

            migrationBuilder.DropTable(
                name: "Parcel");

            migrationBuilder.DropTable(
                name: "Recipient");
        }
    }
}
