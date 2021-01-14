using Microsoft.EntityFrameworkCore.Migrations;

namespace EduJournal.DAL.Migrations
{
    public partial class AddJournalRecordsConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JournalRecords_LectureId",
                table: "JournalRecords");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_JournalRecords_LectureId_StudentId",
                table: "JournalRecords",
                columns: new[] { "LectureId", "StudentId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords",
                sql: "[Attendance] = 0 AND [Score] = 0 OR [Attendance] = 1 AND [Score] BETWEEN 0 AND 5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_JournalRecords_LectureId_StudentId",
                table: "JournalRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_JournalRecords_Score",
                table: "JournalRecords");

            migrationBuilder.CreateIndex(
                name: "IX_JournalRecords_LectureId",
                table: "JournalRecords",
                column: "LectureId");
        }
    }
}
