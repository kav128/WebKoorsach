namespace EduJournal.BLL.DTO
{
    public record LectureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
    }
}
