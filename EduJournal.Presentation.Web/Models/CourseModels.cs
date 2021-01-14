using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;

namespace EduJournal.Presentation.Web.Models
{
    public record CourseModel(int Id, string Name, int LecturerId, int[] LectureIds)
    {
        public virtual bool Equals(CourseModel? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id &&
                   Name == other.Name &&
                   LecturerId == other.LecturerId &&
                   LectureIds.SequenceEqual(other.LectureIds);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name, LecturerId, LectureIds);
    }

    public record CourseAddModel(string Name, int LecturerId);

    public record CourseUpdateModel(int Id, string Name, int LecturerId);

    /* --- */

    public class CourseAddModelValidator : AbstractValidator<CourseAddModel>
    {
        public CourseAddModelValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.LecturerId).GreaterThan(0);
        }
    }

    public class CourseUpdateModelValidator : AbstractValidator<CourseUpdateModel>
    {
        public CourseUpdateModelValidator()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.LecturerId).GreaterThan(0);
        }
    }
}
