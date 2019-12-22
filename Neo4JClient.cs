// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Neo4j.Driver.V1;
using Neo4JSample.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4JSample.Serializer;
using Neo4JSample.Settings;
using System.Linq;

namespace Neo4JSample
{
    public class Neo4JClient : IDisposable
    {
        private readonly IDriver driver;

        public Neo4JClient(IConnectionSettings settings)
        {
            this.driver = GraphDatabase.Driver(settings.Uri, settings.AuthToken);
        }

        public async Task CreateIndices()
        {
            string[] queries = {
                "CREATE INDEX ON :Movie(title)",
                "CREATE INDEX ON :Movie(id)",
                "CREATE INDEX ON :Person(id)",
                "CREATE INDEX ON :Person(name)",
                "CREATE INDEX ON :Genre(name)"
            };

            using (var session = driver.Session())
            {
                foreach (var query in queries)
                {
                    await session.RunAsync(query);
                }
            }
        }

        public async Task CreatePersons(IList<Person> persons)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {persons} AS person")
                .AppendLine("MERGE (p:Person {name: person.name})")
                .AppendLine("SET p = person")
                .ToString();

            using (var session = driver.Session())
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "persons", ParameterSerializer.ToDictionary(persons) } });
            }
        }

        public async Task CreateGenres(IList<Genre> genres)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {genres} AS genre")
                .AppendLine("MERGE (g:Genre {name: genre.name})")
                .AppendLine("SET g = genre")
                .ToString();

            using (var session = driver.Session())
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "genres", ParameterSerializer.ToDictionary(genres) } });
            }
        }

        public async Task CreateMovies(IList<Movie> movies)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {movies} AS movie")
                .AppendLine("MERGE (m:Movie {id: movie.id})")
                .AppendLine("SET m = movie")
                .ToString();

            using (var session = driver.Session())
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "movies", ParameterSerializer.ToDictionary(movies) } });
            }
        }

        public async Task CreateRelationships(IList<MovieInformation> metadatas)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {metadatas} AS metadata")
                 // Find the Movie:
                 .AppendLine("MATCH (m:Movie { title: metadata.movie.title })")
                 // Create Cast Relationships:
                 .AppendLine("UNWIND metadata.cast AS actor")
                 .AppendLine("MATCH (a:Person { name: actor.name })")
                 .AppendLine("MERGE (a)-[r:ACTED_IN]->(m)")
                 // Create Director Relationship:
                 .AppendLine("WITH metadata, m")
                 .AppendLine("MATCH (d:Person { name: metadata.director.name })")
                 .AppendLine("MERGE (d)-[r:DIRECTED]->(m)")
                // Add Genres:
                .AppendLine("WITH metadata, m")
                .AppendLine("UNWIND metadata.genres AS genre")
                .AppendLine("MATCH (g:Genre { name: genre.name})")
                .AppendLine("MERGE (m)-[r:GENRE]->(g)")
                .ToString();


            using (var session = driver.Session())
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "metadatas", ParameterSerializer.ToDictionary(metadatas) } });
            }
        }

        public IList<Person> GetSpecificPersons(string movieType, string movieName, string personType)
        {
            List<Person> persons = new List<Person>();
            string query = string.Empty;

            if (!string.IsNullOrEmpty(movieType) && !string.IsNullOrEmpty(movieName) && !string.IsNullOrEmpty(personType))
            {
                query = "MATCH ( {name: '" + movieType + "'})--(m:Movie) MATCH(m: Movie { title: '" + movieName + "'}) - [:" + personType + "] - (n: Person) RETURN n.name";
            }

            if (!string.IsNullOrEmpty(movieType) && string.IsNullOrEmpty(movieName) && !string.IsNullOrEmpty(personType))
            {
                query = "MATCH ( {name: '" + movieType + "'})--(m:Movie) MATCH(m: Movie) - [:" + personType + "] - (n: Person) RETURN n.name";
            }

            if (!string.IsNullOrEmpty(movieType) && !string.IsNullOrEmpty(movieName) && string.IsNullOrEmpty(personType))
            {
                query = "MATCH ( {name: '" + movieType + "'})--(m:Movie) MATCH(m: Movie { title: '" + movieName + "'}) -- (n: Person) RETURN n.name";
            }

            if (!string.IsNullOrEmpty(movieType) && string.IsNullOrEmpty(movieName) && string.IsNullOrEmpty(personType))
            {
                query = "MATCH ( {name: '" + movieType + "'})--(m:Movie) MATCH (m:Movie) -- (n: Person) RETURN n.name";
            }

            if (string.IsNullOrEmpty(movieType) && !string.IsNullOrEmpty(movieName) && !string.IsNullOrEmpty(personType))
            {
                query = "MATCH(m: Movie { title: '" + movieName + "'}) - [:" + personType + "] - (n: Person) RETURN n.name";
            }
            
            if (string.IsNullOrEmpty(movieType) && !string.IsNullOrEmpty(movieName) && string.IsNullOrEmpty(personType))
            {
                query = "MATCH(m: Movie { title: '" + movieName + "'}) -- (n: Person) RETURN n.name";
            }
            
            if (string.IsNullOrEmpty(movieType) && string.IsNullOrEmpty(movieName) && !string.IsNullOrEmpty(personType))
            {
                query = "MATCH (m: Movie) - [:" + personType + "] - (n: Person) RETURN n.name";
            }

            if (string.IsNullOrEmpty(movieType) && string.IsNullOrEmpty(movieName) && string.IsNullOrEmpty(personType))
            {
                query = "MATCH (n: Person) RETURN n.name";
            }

            using (var session = driver.Session())
            {
                var temp = session.Run(query);

                foreach (var record in temp)
                {
                    persons.Add(new Person { Name = record["n.name"].ToString() });
                }
            }

            return persons;
        }

        public IList<Movie> GetSpecificMovies(string movieType)
        {
            string query;
            List<Movie> movies = new List<Movie>();

            if (!string.IsNullOrEmpty(movieType))
            {
                query = "MATCH ( {name: '" + movieType + "'})--(m:Movie) RETURN m.title";
            }
            else
            {
                query = "MATCH (m:Movie) RETURN m.title";
            }

            using (var session = driver.Session())
            {
                var temp = session.Run(query);

                foreach (var record in temp)
                {
                    movies.Add(new Movie { Title = record["m.title"].ToString() });
                }
            }

            return movies;
        }


        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}