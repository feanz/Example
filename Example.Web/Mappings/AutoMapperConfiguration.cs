using AutoMapper;
using Example.Core.Model;
using Utilities.Extensions;
using Web.Areas.Api.Models;
using Web.ViewModels;

namespace Web.Mappings
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<Bookmark, BookmarkViewModel>()
                .ForMember(d => d.Tags, s => s.MapFrom(o => o.Tags.ToCommaSeparatedString()));
            
            Mapper.CreateMap<Bookmark, BookmarkDTO>()
                .ForMember(d => d.Tags, s => s.MapFrom(o => o.Tags.ToCommaSeparatedString()));

            Mapper.CreateMap<BookmarkViewModel, Bookmark>()
                .ForMember(d => d.Tags, s => s.MapFrom(o => o.Tags.ToList()))
                .ForMember(d => d.IsFavourite,s => s.Ignore())
                .ForMember(d => d.BookmarkType, s => s.MapFrom(o => new BookmarkType() { Id = o.BookmarkTypeId, TypeName = o.BookmarkTypeTypeName }));

            Mapper.CreateMap<BookmarkDTO, Bookmark>()
                .ForMember(d => d.Tags, s => s.MapFrom(o => o.Tags.ToList()))
                .ForMember(d => d.IsFavourite, s => s.Ignore())
                .ForMember(d => d.BookmarkType, s => s.MapFrom(o => new BookmarkType() { Id = o.BookmarkTypeId, TypeName = o.BookmarkTypeTypeName }));
        }
    }
}