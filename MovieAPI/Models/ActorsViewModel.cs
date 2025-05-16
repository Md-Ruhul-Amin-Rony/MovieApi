using MovieAPI.NewFolder;

namespace MovieAPI.Models
{
    public class ActorsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<int> MoviesId { get; set; }
      
       
    }
}
