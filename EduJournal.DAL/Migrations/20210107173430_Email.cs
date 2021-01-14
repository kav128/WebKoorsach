using Microsoft.EntityFrameworkCore.Migrations;

namespace EduJournal.DAL.Migrations
{
    public partial class Email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Lecturers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "testm10lec@mail.ru");

            migrationBuilder.UpdateData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "testm10lec@mail.ru");

            migrationBuilder.UpdateData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Email",
                value: "testm10lec@mail.ru");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "testm10stud@mail.ru");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "testm10stud@mail.ru");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                column: "Email",
                value: "testm10stud@mail.ru");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                column: "Email",
                value: "testm10stud@mail.ru");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                column: "Email",
                value: "testm10stud@mail.ru");

            migrationBuilder.AddCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords",
                sql: "[Attendance] = 0 AND [Score] = 0 OR [Attendance] = 1 AND [Score] BETWEEN 0 AND 5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Lecturers");

            migrationBuilder.AddCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords",
                sql: "[Attendance] = 0 AND [Score] = 0 OR [Attendance] = 1 AND [Score] BETWEEN 1 AND 5");
        }
    }
}
