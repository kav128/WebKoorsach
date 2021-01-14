using FluentValidation;

namespace EduJournal.Presentation.Web.Models
{
    public record JournalRecordModel(bool Attendance, int Score, int StudentId, int LectureId);
    
    /* --- */

    public class JournalRecordModelValidator : AbstractValidator<JournalRecordModel>
    {
        public JournalRecordModelValidator()
        {
            RuleFor(model => model.Score)
                .Equal(0)
                .When(model => !model.Attendance, ApplyConditionTo.CurrentValidator)
                .InclusiveBetween(0, 5)
                .When(model => model.Attendance, ApplyConditionTo.CurrentValidator);
            RuleFor(model => model.StudentId).GreaterThan(0);
            RuleFor(model => model.LectureId).GreaterThan(0);
        }
    }
}