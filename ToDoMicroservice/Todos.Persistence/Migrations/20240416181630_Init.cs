using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Todos.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnerRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerRoles", x => x.RoleId);
                });
            
            migrationBuilder.InsertData(
                table: "OwnerRoles",
                columns: new[] { "RoleId", "Name" },
                values: new object[,]
                {
                    { 1, "Client" },
                    { 2, "Admin" },
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "OwnerOwnerRole",
                columns: table => new
                {
                    OwnersOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerOwnerRole", x => new { x.OwnersOwnerId, x.RolesRoleId });
                    table.ForeignKey(
                        name: "FK_OwnerOwnerRole_OwnerRoles_RolesRoleId",
                        column: x => x.RolesRoleId,
                        principalTable: "OwnerRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnerOwnerRole_Owners_OwnersOwnerId",
                        column: x => x.OwnersOwnerId,
                        principalTable: "Owners",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    TodoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.TodoId);
                    table.ForeignKey(
                        name: "FK_Todos_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerOwnerRole_RolesRoleId",
                table: "OwnerOwnerRole",
                column: "RolesRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_OwnerId",
                table: "Todos",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnerOwnerRole");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "OwnerRoles");

            migrationBuilder.DropTable(
                name: "Owners");
        }
    }
}
