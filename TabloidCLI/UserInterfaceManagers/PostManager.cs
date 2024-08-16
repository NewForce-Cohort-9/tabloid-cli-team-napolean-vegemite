using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class PostManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private PostRepository _postRepository;
        private BlogRepository _blogRepository;
        private string _connectionString;

        public PostManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _postRepository = new PostRepository(connectionString);
            _blogRepository = new BlogRepository(connectionString);
            _connectionString = connectionString;
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Post Menu");
            Console.WriteLine(" 1) List Post Entries");
            Console.WriteLine(" 2) Add Post Entry");
            Console.WriteLine(" 3) Remove Post Entry");
            Console.WriteLine(" 4) Edit Post Entry");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        List();
                        return this;
                    case 2:
                        Add();
                        return this;
                    case 3:
                        Remove();
                        return this;
                    case 4:
                        Edit();
                        return this;
                    case 0:
                        return _parentUI;
                    default:
                        Console.WriteLine("Invalid Selection");
                        return this;
                }
            }
            else
            {
                Console.WriteLine("Invalid Selection");
                return this;
            }
        }

        private void Add()
        {
            Console.WriteLine("New Post");
            Post post = new Post();

            Console.Write("Title: ");
            post.Title = Console.ReadLine();

            Console.Write("Url: ");
            post.Url = Console.ReadLine();

            post.PublishDateTime = DateTime.Now;
            post.Author = ChooseAuthor();
            post.Blog = ChooseBlog();

            _postRepository.Insert(post);
        }

        private Post Choose(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a Post:";
            }
            Console.WriteLine(prompt);

            List<Post> posts = _postRepository.GetAll();

            for (int i = 0; i < posts.Count; i++)
            {
                Post post = posts[i];
                Console.WriteLine($"{i + 1} - {post.Title} - Published on {post.PublishDateTime} - {post.Url}");
            }

            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return posts[choice - 1];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private void Edit()
        {
            Post postToEdit = Choose("Which post would you like to edit?");
            if (postToEdit == null)
            {
                return;
            }

            Console.WriteLine();
            Console.Write("New title (blank to leave unchanged): ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                postToEdit.Title = title;
            }

            Console.Write("New URL (blank to leave unchanged): ");
            string url = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
            {
                postToEdit.Url = url;
            }

            _postRepository.Update(postToEdit);
        }

        private void List()
        {
            List<Post> posts = _postRepository.GetAll();
            foreach (Post post in posts)
            {
                Console.WriteLine($"{post.Title} - Published On: {post.PublishDateTime} - {post.Url} - Author: {post.Author.FirstName} {post.Author.LastName} - Blog: {post.Blog.Title}");
            }
        }

        private void Remove()
        {
            Post postToDelete = Choose("Which post entry would you like to remove?");
            if (postToDelete != null)
            {
                _postRepository.Delete(postToDelete.Id);
            }
        }

        private Author ChooseAuthor()
        {
            return new Author { FirstName = "Default", LastName = "Author" };
        }

        private Blog ChooseBlog()
        {
            Console.WriteLine("Select a blog:");
            List<Blog> blogs = _blogRepository.GetAll();
            if (blogs.Count > 0)
            {
                for (int i = 0; i < blogs.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {blogs[i].Title} ({blogs[i].Url})");
                }
                Console.Write("Select blog (enter number): ");
                int choice = Convert.ToInt32(Console.ReadLine()) - 1;
                return blogs[choice];
            }
            else
            {
                Console.WriteLine("No blogs available to select.");
                return null;  // Handle this case appropriately in calling method
            }
        }

    }
}
