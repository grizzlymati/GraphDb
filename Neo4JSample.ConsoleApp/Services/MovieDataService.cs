using Neo4JSample.Model;
using System.Collections.Generic;

namespace Neo4JSample.ConsoleApp.Services
{
    public interface IMovieDataService
    {
        IList<Genre> Genres { get; }

        IList<Person> Persons { get; }

        IList<Movie> Movies { get; }

        IList<MovieInformation> Metadatas { get; }

        void PrepareDataForDb();
    }

    public class MovieDataService : IMovieDataService
    {
        private static IList<Person> persons = new List<Person>();
        private static IList<Movie> movies = new List<Movie>();
        private static IList<Genre> genres = new List<Genre>();
        private static IList<MovieInformation> movieInformations = new List<MovieInformation>();

        public void PrepareDataForDb()
        {
            Person person1Movie1ForActionType = new Person() { Id = 1, Name = "Actor1" };
            Person person2Movie1ForActionType = new Person() { Id = 2, Name = "Actor2" };
            Person person3Movie1ForActionType = new Person() { Id = 3, Name = "Actor3" };
            Person person1Movie2ForActionType = new Person() { Id = 4, Name = "Actor4" };
            Person person2Movie2ForActionType = new Person() { Id = 5, Name = "Actor5" };
            Person person3Movie2ForActionType = new Person() { Id = 6, Name = "Actor6" };
            Person person1Movie3ForActionType = new Person() { Id = 7, Name = "Actor7" };
            Person person2Movie3ForActionType = new Person() { Id = 8, Name = "Actor8" };
            Person person3Movie3ForActionType = new Person() { Id = 9, Name = "Actor9" };
            Person directorForMovie1ForActionType = new Person() { Id = 10, Name = "Director1" };
            Person directorForMovie2ForActionType = new Person() { Id = 11, Name = "Director2" };
            Person directorForMovie3ForActionType = new Person() { Id = 12, Name = "Director3" };
            Movie movie1ActionType = new Movie() { Id = 1, Title = "Movie1"};
            Movie movie2ActionType = new Movie() { Id = 2, Title = "Movie2" };
            Movie movie3ActionType = new Movie() { Id = 3, Title = "Movie3" };
            Genre movieTypeAction = new Genre() { Name = "ActionType" };
            Genre movieTypeHorror = new Genre() { Name = "HorrorType" };

            MovieInformation relationForActionType = new MovieInformation() {
                Director = directorForMovie1ForActionType,
                Cast = new List<Person> { person1Movie1ForActionType, person2Movie1ForActionType, person3Movie1ForActionType },
                Movie = movie1ActionType,
                Genres = new List<Genre> { movieTypeAction, movieTypeHorror }
            };

            MovieInformation relationForActionType2 = new MovieInformation()
            {
                Director = directorForMovie2ForActionType,
                Cast = new List<Person> { person1Movie2ForActionType, person2Movie2ForActionType, person3Movie2ForActionType },
                Movie = movie2ActionType,
                Genres = new List<Genre> { movieTypeAction }
            };

            MovieInformation relationForActionType3 = new MovieInformation()
            {
                Director = directorForMovie3ForActionType,
                Cast = new List<Person> { person1Movie3ForActionType, person2Movie3ForActionType, person3Movie3ForActionType },
                Movie = movie3ActionType,
                Genres = new List<Genre> { movieTypeAction, movieTypeHorror }
            };

            persons.Add(person1Movie1ForActionType);
            persons.Add(person2Movie1ForActionType);
            persons.Add(person3Movie1ForActionType);
            persons.Add(person1Movie2ForActionType);
            persons.Add(person2Movie2ForActionType);
            persons.Add(person3Movie2ForActionType);
            persons.Add(person1Movie3ForActionType);
            persons.Add(person2Movie3ForActionType);
            persons.Add(person3Movie3ForActionType);
            persons.Add(directorForMovie1ForActionType);
            persons.Add(directorForMovie2ForActionType);
            persons.Add(directorForMovie3ForActionType);

            movies.Add(movie1ActionType);
            movies.Add(movie2ActionType);
            movies.Add(movie3ActionType);

            genres.Add(movieTypeAction);
            genres.Add(movieTypeHorror);

            movieInformations.Add(relationForActionType);
            movieInformations.Add(relationForActionType2);
            movieInformations.Add(relationForActionType3);
        }

        public IList<Person> Persons
        {
            get
            {
                return persons;
            }
        }
        
        public IList<Movie> Movies
        {
            get
            {
                return movies;
            }
        }

        public IList<Genre> Genres
        {
            get
            {
                return genres;
            }
        }

        public IList<MovieInformation> Metadatas
        {
            get
            {
                return movieInformations;
            }
        }
    }
}
