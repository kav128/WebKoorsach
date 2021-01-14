namespace EduJournal.BLL.DTO
{
    public record JournalRecordDto
    {
        public bool Attendance { get; set; }
        public int Score { get; set; }
        public int StudentId { get; set; }
        public int LectureId { get; set; }
    }
}
