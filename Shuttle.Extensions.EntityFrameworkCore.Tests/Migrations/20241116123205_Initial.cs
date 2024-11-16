using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Shuttle.Core.Contract;

#nullable disable

namespace Shuttle.Extensions.EntityFrameworkCore.Tests.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        private readonly IDbContextSchema _dbContextSchema;

        public Initial(IDbContextSchema dbContextSchema)
        {
            _dbContextSchema = Guard.AgainstNull(dbContextSchema);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: _dbContextSchema.Schema);

            migrationBuilder.CreateTable(
                name: "FixtureEntity",
                schema: _dbContextSchema.Schema,
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(65)", maxLength: 65, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixtureEntity", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FixtureEntity",
                schema: _dbContextSchema.Schema);
        }
    }
}
