namespace EduJournal.BLL.DTO
{
    public record StudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; init; }
    }
}
