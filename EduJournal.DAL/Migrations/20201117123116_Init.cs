using Microsoft.EntityFrameworkCore.Migrations;

namespace EduJournal.DAL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LecturerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Lecturers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lectures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lectures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lectures_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attendance = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    LectureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalRecords_Lectures_LectureId",
                        column: x => x.LectureId,
                        principalTable: "Lectures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Patricia James" },
                    { 2, "Phillip Ross" },
                    { 3, "Paul Baker" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Eric Jenkins" },
                    { 2, "Brian White" },
                    { 3, "Virginia Martinez" },
                    { 4, "Amy Johnson" },
                    { 5, "Terry Parker" }
                });


            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "LecturerId", "Name" },
                values: new object[,]
                {
                    { 1, 2, "Chemistry" },
                    { 2, 3, "Math" },
                    { 3, 3, "English" },
                    { 4, 3, "Physics" },
                    { 5, 1, "Computer Science" }
                });
            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "Id", "CourseId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Chemistry, lecture 1" },
                    { 2, 1, "Chemistry, lecture 2" },
                    { 3, 1, "Chemistry, lecture 3" },
                    { 4, 1, "Chemistry, lecture 4" },
                    { 5, 2, "Math, lecture 1" },
                    { 6, 2, "Math, lecture 2" },
                    { 7, 2, "Math, lecture 3" },
                    { 8, 3, "English, lecture 1" },
                    { 9, 3, "English, lecture 2" },
                    { 10, 3, "English, lecture 3" },
                    { 11, 4, "Physics, lecture 1" },
                    { 12, 4, "Physics, lecture 2" },
                    { 13, 4, "Physics, lecture 3" },
                    { 14, 5, "Computer Science, lecture 1" },
                    { 15, 5, "Computer Science, lecture 2" },
                    { 16, 5, "Computer Science, lecture 3" },
                    { 17, 5, "Computer Science, lecture 4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LecturerId",
                table: "Courses",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalRecords_LectureId",
                table: "JournalRecords",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalRecords_StudentId",
                table: "JournalRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_CourseId",
                table: "Lectures",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalRecords");

            migrationBuilder.DropTable(
                name: "Lectures");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Lecturers");
        }
    }
}
