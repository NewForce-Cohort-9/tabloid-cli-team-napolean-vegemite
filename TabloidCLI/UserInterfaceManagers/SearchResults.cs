using System;
using System.Collections.Generic;
using TabloidCLI.Models;

namespace TabloidCLI.UserInterfaceManagers
{
    public class SearchResults<T>
    {
        private List<T> _results = new List<T>();

        public string Title { get; set; } = "Search Results";

        public bool NoResultsFound
        {
            get
            {
                return _results.Count == 0;
            }
        }

        public void Add(T result)
        {
            _results.Add(result);
        }

        public void Display(string sectionTitle = null)
        {
            if (!string.IsNullOrEmpty(sectionTitle))
            {
                Console.WriteLine(sectionTitle);
            }

            if (NoResultsFound)
            {
                Console.WriteLine($"No results found.");
            }
            else
            {
                foreach (T result in _results)
                {
                    Console.WriteLine(" " + result);
                }
            }

            Console.WriteLine();
        }
    }
}

