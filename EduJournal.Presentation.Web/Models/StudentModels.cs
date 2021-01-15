using FluentValidation;
using FluentValidation.Validators;

namespace EduJournal.Presentation.Web.Models
{
    public record StudentModel(int Id, string FullName, string? Email);
    
    public record StudentAddModel(string FullName, string? Email);

    public record StudentUpdateModel(int Id, string FullName, string? Email);
    
    /* --- */

    public class StudentAddModelValidator : AbstractValidator<StudentAddModel>
    {
        public StudentAddModelValidator()
        {
            RuleFor(model => model.FullName).NotEmpty();
            RuleFor(model => model.Email)
                .EmailAddress()
                .When(model => model.Email is not null, ApplyConditionTo.CurrentValidator);
        }
    }

    public class StudentUpdateModelValidator : AbstractValidator<StudentUpdateModel>
    {
        public StudentUpdateModelValidator()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.FullName).NotEmpty();
            RuleFor(model => model.Email)
                .EmailAddress()
                .When(model => model.Email is not null, ApplyConditionTo.CurrentValidator);
        }
    }
}
