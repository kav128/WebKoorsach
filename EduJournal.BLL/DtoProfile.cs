using System;
using System.Linq;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.DAL.Entities;

namespace EduJournal.BLL
{
    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dto => dto.LectureIds, 
                    expression => expression.MapFrom(course => course.Lectures.Select(lecture => lecture.Id)))
                .ReverseMap()
                .ForMember(course => course.Lectures,
                    expression => expression.MapFrom(dto => dto.LectureIds.Select(id => new Course { Id = id })));
            
            CreateMap<Lecturer, LecturerDto>()
                .ForMember(dto => dto.CourseIds,
                    expression => expression.MapFrom(lecturer => lecturer.Courses.Select(course => course.Id)))
                .ReverseMap()
                .ForMember(lecturer => lecturer.Courses,
                    expression => expression.MapFrom(dto => dto.CourseIds.Select(id => new Course { Id = id })));
            
            CreateMap<Lecture, LectureDto>().ReverseMap();
            
            CreateMap<Student, StudentDto>()
                .ReverseMap()
                .ForMember(student => student.JournalRecords,
                    expression => expression.MapFrom(dto => Array.Empty<JournalRecord>()));
            
            CreateMap<JournalRecord, JournalRecordDto>().ReverseMap();
        }
    }
}
