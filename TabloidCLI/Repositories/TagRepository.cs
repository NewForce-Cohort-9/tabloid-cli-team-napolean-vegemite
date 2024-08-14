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
                    //if (TagExists(tag.Name))
                    //{
                    //    throw new InvalidOperationException("A tag with this name already exists.");
                    //}




                    //Inserts a new row called Name into the Tag table with the @Name parameter's value.

                    cmd.CommandText = @"INSERT INTO Tag (Name)
                                                    VALUES (@Name)";
                    cmd.Parameters.AddWithValue("@Name",tag.Name);
             

                    cmd.ExecuteNonQuery();
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
            throw new NotImplementedException();
        }






        public SearchResults<Tag> SearchTags(string tagName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //t.Id AS TagId,
                    //                           t.Name,
                    //                        at.TagId AS AuthorTagId,
                    //                        at.AuthorId AS AuthorId,
                    //                        pt.TagId AS PostTagId,
                    //                        pt.PostId AS PostId,
                    //                        bt.TagId AS BlogTagId
                    //                        bt.BlogId AS BlogId,

                    //                     FROM Tag t 
                    //                     LEFT JOIN AuthorTag at ON t.Id = at.TagId
                    //                     LEFT JOIN PostTag pt ON t.Id = pt.TagId
                    //                     LEFT JOIN BlogTag bt ON t.Id = bt.TagId
                    //                     WHERE a.id = @id";
                    cmd.CommandText = @"SELECT Id, Name
                                    FROM Tag
                                    WHERE Name LIKE @name";
                    
                    cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
                    SqlDataReader reader = cmd.ExecuteReader();

                    SearchResults<Tag> results = new SearchResults<Tag>();
                    while (reader.Read())
                    {
                        Tag tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            
                        };
                        results.Add(tag);
                    }

                    reader.Close();

                    return results;
                }
            }
        }
    }
}
