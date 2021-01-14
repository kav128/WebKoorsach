using System;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.Presentation.Web.Models;

namespace EduJournal.Presentation.Web
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<LecturerDto, LecturerModel>();
            CreateMap<LecturerAddModel, LecturerDto>()
                .ForMember(dto => dto.CourseIds,
                    expression => expression.MapFrom(model => Array.Empty<int>()));
            CreateMap<LecturerUpdateModel, LecturerDto>()
                .ForMember(dto => dto.CourseIds,
                    expression => expression.MapFrom(model => Array.Empty<int>()));

            CreateMap<CourseDto, CourseModel>();
            CreateMap<CourseAddModel, CourseDto>()
                .ForMember(dto => dto.LectureIds,
                    expression => expression.MapFrom(model => Array.Empty<int>()));
            CreateMap<CourseUpdateModel, CourseDto>()
                .ForMember(dto => dto.LectureIds,
                    expression => expression.MapFrom(model => Array.Empty<int>()));

            CreateMap<StudentDto, StudentModel>();
            CreateMap<StudentAddModel, StudentDto>();
            CreateMap<StudentUpdateModel, StudentDto>();

            CreateMap<LectureDto, LectureModel>();
            CreateMap<LectureAddModel, LectureDto>();
            CreateMap<LectureUpdateModel, LectureDto>();

            CreateMap<JournalRecordDto, JournalRecordModel>().ReverseMap();
        }
    }
}
