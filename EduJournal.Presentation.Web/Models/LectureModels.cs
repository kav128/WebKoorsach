using FluentValidation;

namespace EduJournal.Presentation.Web.Models
{
    public record LectureModel(int Id, string Name, int CourseId);
    
    public record LectureAddModel(string Name, int CourseId);

    public record LectureUpdateModel(int Id, string Name);
    
    /* --- */

    public class LectureAddModelValidator : AbstractValidator<LectureAddModel>
    {
        public LectureAddModelValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.CourseId).GreaterThan(0);
        }
    }
    
    public class LectureUpdateModelValidator : AbstractValidator<LectureUpdateModel>
    {
        public LectureUpdateModelValidator()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
        }
    }
}
