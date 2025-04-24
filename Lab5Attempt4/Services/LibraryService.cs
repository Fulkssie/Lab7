using Lab5Attempt4.Models;

namespace Lab5Attempt4.Services
{
    public class LibraryService
    {
        private List<Book> books = new List<Book>();
        private List<User> users = new List<User>();
        private Dictionary<int, int> borrowedBooks = new Dictionary<int, int>();

        private string booksFile = "./Data/Books.csv";
        private string usersFile = "./Data/Users.csv";

        private bool isTesting;

        public LibraryService() : this(false) { }

        public LibraryService(bool testMode = false)
        {
            isTesting = testMode;

            if (!isTesting)
            {
                LoadBooks();
                LoadUsers();
            }
        }

        private void LoadBooks()
        {
            if (File.Exists(booksFile))
            {
                foreach (var line in File.ReadLines(booksFile))
                {
                    var fields = line.Split(',');

                    if (fields.Length == 4)
                    {
                        books.Add(new Book
                        {
                            Id = int.Parse(fields[0]),
                            Title = fields[1],
                            Author = fields[2],
                            ISBN = fields[3]
                        });
                    }
                }
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFile))
            {
                foreach (var line in File.ReadLines(usersFile))
                {
                    var fields = line.Split(',');

                    if (fields.Length == 3)
                    {
                        users.Add(new User
                        {
                            Id = int.Parse(fields[0]),
                            Name = fields[1],
                            Email = fields[2]
                        });
                    }
                }
            }
        }

        public List<Book> GetBooks() => books;
        public List<User> GetUsers() => users;

        public Book GetBookById(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine($"Book with ID {id} not found.");
            }
            return book;
        }

        public User GetUserById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);
            SaveBooks();
        }

        public void UpdateBook(Book updatedBook)
        {
            var book = books.FirstOrDefault(b => b.Id == updatedBook.Id);
            if (book != null)
            {
                book.Title = updatedBook.Title;
                book.Author = updatedBook.Author;
                book.ISBN = updatedBook.ISBN;
                SaveBooks();
            }
        }

        public void DeleteBook(int bookId)
        {
            books.RemoveAll(b => b.Id == bookId);
            SaveBooks();
        }

        private void SaveBooks()
        {
            if (!isTesting)
                File.WriteAllLines(booksFile, books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}"));
        }

        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveUsers();
        }

        public void UpdateUser(User updatedUser)
        {
            var user = users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                SaveUsers();
            }
        }

        public void DeleteUser(int userId)
        {
            users.RemoveAll(u => u.Id == userId);
            SaveUsers();
        }

        private void SaveUsers()
        {
            if (!isTesting)
                File.WriteAllLines(usersFile, users.Select(u => $"{u.Id},{u.Name},{u.Email}"));
        }

        public void BorrowBook(int bookId, int userId)
        {
            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                Console.WriteLine($"Book with ID {bookId} not found.");
                return;
            }

            if (borrowedBooks.ContainsKey(bookId))
            {
                Console.WriteLine($"Book '{book.Title}' is already borrowed by another user.");
                return;
            }

            borrowedBooks[bookId] = userId;
            Console.WriteLine($"Book '{book.Title}' successfully borrowed by user {userId}.");
        }

        public void ReturnBook(int userId, int bookId)
        {
            if (!borrowedBooks.ContainsKey(bookId))
            {
                Console.WriteLine($"Book with ID {bookId} is not currently borrowed.");
                return;
            }

            var borrowerId = borrowedBooks[bookId];
            if (borrowerId != userId)
            {
                Console.WriteLine($"Book with ID {bookId} was not borrowed by user {userId}. It was borrowed by user {borrowerId}.");
                return;
            }

            borrowedBooks.Remove(bookId);
            Console.WriteLine($"Book with ID {bookId} successfully returned by user {userId}.");
        }

        public List<Book> GetBorrowedBooksByUser(int userId)
        {
            var borrowedBooksList = new List<Book>();

            foreach (var entry in borrowedBooks)
            {
                if (entry.Value == userId)
                {
                    var book = books.FirstOrDefault(b => b.Id == entry.Key);
                    if (book != null)
                    {
                        borrowedBooksList.Add(book);
                    }
                }
            }

            return borrowedBooksList;
        }
    }
}