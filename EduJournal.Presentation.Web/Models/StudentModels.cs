using FluentValidation;
using FluentValidation.Validators;

namespace EduJournal.Presentation.Web.Models
{
    public record StudentModel(int Id, string FullName);
    
    public record StudentAddModel(string FullName);

    public record StudentUpdateModel(int Id, string FullName);
    
    /* --- */

    public class StudentAddModelValidator : AbstractValidator<StudentAddModel>
    {
        public StudentAddModelValidator()
        {
            RuleFor(model => model.FullName).NotEmpty();
        }
    }

    public class StudentUpdateModelValidator : AbstractValidator<StudentUpdateModel>
    {
        public StudentUpdateModelValidator()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.FullName).NotEmpty();
        }
    }
}
