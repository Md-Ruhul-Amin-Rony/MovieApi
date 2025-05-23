using AutoMapper;
using MovieAPI.Models;
using MovieAPI.NewFolder;

namespace MovieAPI
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Movie, MovieListViewModel>().ReverseMap();
            CreateMap<CreateMovieViewModel, Movie>().ForMember(x => x.Actors, y=> y.Ignore());
            CreateMap<Movie, MovieDetailsViewModel>();
            CreateMap<Person, ActorViewModel>().ReverseMap();
            CreateMap<Person, ActorDetailsModel>();
            CreateMap<Person, ActorsViewModel>();
        }
    }
}
