using System;
using System.Linq;
using FluentValidation;

namespace EduJournal.Presentation.Web.Models
{
    public record LecturerModel(int Id, string FullName, string? Email, int[] CourseIds)
    {
        public virtual bool Equals(LecturerModel? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && FullName == other.FullName && CourseIds.SequenceEqual(other.CourseIds);
        }

        public override int GetHashCode() => HashCode.Combine(Id, FullName, CourseIds);
    }

    public record LecturerAddModel(string FullName, string? Email);

    public record LecturerUpdateModel(int Id, string FullName, string? Email);
    
    /* --- */

    public class LecturerAddModelValidator : AbstractValidator<LecturerAddModel>
    {
        public LecturerAddModelValidator()
        {
            RuleFor(model => model.FullName).NotEmpty();
            RuleFor(model => model.Email)
                .EmailAddress()
                .When(model => model.Email is not null, ApplyConditionTo.CurrentValidator);
        }
    }

    public class LecturerUpdateModelValidator : AbstractValidator<LecturerUpdateModel>
    {
        public LecturerUpdateModelValidator()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.FullName).NotEmpty();
            RuleFor(model => model.Email)
                .EmailAddress()
                .When(model => model.Email is not null, ApplyConditionTo.CurrentValidator);
        }
    }
}
