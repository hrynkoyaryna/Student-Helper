using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubjectsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Subjects data
            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Name", "ShortName", "Description", "DefaultColor" },
                values: new object[,]
                {
                    { "Математика", "Мат", "Математика як основна наука", "#FF5733" },
                    { "Українська мова", "УМ", "Українська мова як предмет", "#33FF57" },
                    { "Англійська мова", "АМ", "Англійська мова як іноземна", "#3357FF" },
                    { "Історія", "Іст", "Історія як предмет", "#FF33A8" },
                    { "Географія", "Геог", "Географія України", "#FFD700" },
                    { "Біологія", "Біо", "Біологія людини", "#00BFFF" },
                    { "Фізика", "Физ", "Фізика як науковий предмет", "#9370DB" },
                    { "Хімія", "Хім", "Хімія", "#32CD32" },
                    { "Фізкультура", "ФК", "Фізкультура", "#4682B4" },
                    { "Мистецтво", "Мист", "Образотворче мистецтво", "#FFB6C1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seed data
            migrationBuilder.Sql("DELETE FROM \"Subjects\" WHERE \"Name\" IN ('??????????', '??????', '?????????????', '?????', '???????', '??????????', '??????????', '????????', '?????????', '?????????')");
        }
    }
}
