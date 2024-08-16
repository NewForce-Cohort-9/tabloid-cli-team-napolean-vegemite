using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class NoteRepository : DatabaseConnector
    {
        public NoteRepository(string connectionString) : base(connectionString) { }

        public List<Note> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            n.Id AS NoteId,
                            n.Title AS NoteTitle,
                            n.TextContext,
                            n.CreationDateTime,
                            n.PostId,
                            p.Title AS PostTitle,
                            p.Url AS PostUrl,
                            p.PublishDateTime AS PostPublishDateTime
                        FROM Note n
                        LEFT JOIN Post p ON n.PostId = p.Id";

                    List<Note> notes = new List<Note>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Note note = new Note()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("NoteId")),
                            Title = reader.GetString(reader.GetOrdinal("NoteTitle")),
                            TextContext = reader.GetString(reader.GetOrdinal("TextContext")),
                            CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime")),
                            Post = new Post
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                                Url = reader.GetString(reader.GetOrdinal("PostUrl")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PostPublishDateTime"))
                            }
                        };
                        notes.Add(note);
                    }

                    reader.Close();

                    return notes;
                }
            }
        }

        public void Insert(Note note)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Note (Title, TextContext, CreationDateTime, PostId)
                        VALUES (@Title, @TextContext, @CreationDateTime, @PostId)";

                    cmd.Parameters.AddWithValue("@Title", note.Title);
                    cmd.Parameters.AddWithValue("@TextContext", note.TextContext);
                    cmd.Parameters.AddWithValue("@CreationDateTime", note.CreationDateTime);
                    cmd.Parameters.AddWithValue("@PostId", note.Post.Id); // set Post.Id 

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
                    cmd.CommandText = @"DELETE FROM Note WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}


