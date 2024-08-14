using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.Repositories;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI
{
    public class TagRepository : DatabaseConnector, IRepository<Tag>
    {
        public TagRepository(string connectionString) : base(connectionString) { }

        public List<Tag> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Tag"; //check Tag.cs for property names
                    List<Tag> tags = new List<Tag>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tag tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                        tags.Add(tag);
                    }

                    reader.Close();

                    return tags;
                }
            }
        }

        public Tag Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                         FROM Tag  
                                         WHERE Id = @id"; //int id

                    cmd.Parameters.AddWithValue("@id", id);

                    Tag tag = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {

                        tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")), //"Id" from SELECT Id
                            Name = reader.GetString(reader.GetOrdinal("Name")),

                        };
                    }

                    reader.Close();

                    return tag;
                }
            }
        }

        public void Insert(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    // Check if the tag already exists
                    //WHERE Name = @Name; 

                    cmd.CommandText = @"
                    IF EXISTS (
                    SELECT 1
                    FROM Tag
                    WHERE Name = @Name; 
                    )

                    BEGIN
                        THROW 50000, 'Tag already exists';
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Tag (Name)
                        OUTPUT INSERTED.Id
                        VALUES(@Name)
                    END";
                    cmd.Parameters.AddWithValue("@Name", tag.Name);

                    int id = (int)cmd.ExecuteScalar();
                    tag.Id = id; 
                }
            }
        }




 



        public void Update(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Tag 
                                           SET Name = @Name 
                                         WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Name", tag.Name);
                    cmd.Parameters.AddWithValue("@Id", tag.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //IF EXISTS..SELECT 1..UNION to check if the tag is in use in any related tables
                    // SELECT 1 checks eixstence, if any rows match the condition, dont care about actual data
                    //THROW >=50000 for custom error messages. 
                    cmd.CommandText = @"
                IF EXISTS (
                SELECT 1 FROM AuthorTag WHERE TagId = @id 
                UNION
                SELECT 1 PostTag WHERE TagId = @id 
                UNION
                SELECT 1 BlogTag WHERE TagId = @id) 

                BEGIN
                    THROW 50000, 'Cannot delete tag because it is in use';
                END
                ELSE
                BEGIN
                    DELETE FROM Tag WHERE Id = @id;
                END";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }




        public SearchResults<Author> SearchAuthors(string tagName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT a.id,
                                               a.FirstName,
                                               a.LastName,
                                               a.Bio
                                          FROM Author a
                                               LEFT JOIN AuthorTag at on a.Id = at.AuthorId
                                               LEFT JOIN Tag t on t.Id = at.TagId
                                         WHERE t.Name LIKE @name";
                    cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
                    SqlDataReader reader = cmd.ExecuteReader();

                    SearchResults<Author> results = new SearchResults<Author>();
                    while (reader.Read())
                    {
                        Author author = new Author()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Bio = reader.GetString(reader.GetOrdinal("Bio")),
                        };
                        results.Add(author);
                    }

                    reader.Close();

                    return results;
                }
            }
        }

        //public SearchResults<Tag> SearchTags(string tagName)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            //t.Id AS TagId,
        //            //                           t.Name,
        //            //                        at.TagId AS AuthorTagId,
        //            //                        at.AuthorId AS AuthorId,
        //            //                        pt.TagId AS PostTagId,
        //            //                        pt.PostId AS PostId,
        //            //                        bt.TagId AS BlogTagId
        //            //                        bt.BlogId AS BlogId,

        //            //                     FROM Tag t 
        //            //                     LEFT JOIN AuthorTag at ON t.Id = at.TagId
        //            //                     LEFT JOIN PostTag pt ON t.Id = pt.TagId
        //            //                     LEFT JOIN BlogTag bt ON t.Id = bt.TagId
        //            //                     WHERE a.id = @id";
        //            cmd.CommandText = @"SELECT Id, Name
        //                            FROM Tag
        //                            WHERE Name LIKE @name";

        //            cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            SearchResults<Tag> results = new SearchResults<Tag>();
        //            while (reader.Read())
        //            {
        //                Tag tag = new Tag()
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Name = reader.GetString(reader.GetOrdinal("Name")),

        //                };
        //                results.Add(tag);
        //            }

        //            reader.Close();

        //            return results;
        //        }
        //    }
        //}
    }
}
