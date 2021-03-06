using LibraryManagementWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LibraryManagementWebApplication.Controllers
{
    public class BooksController : Controller
    {
        // GET: BooksController
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Search(Book book)
        {
            try {
                ViewData["books"] = JsonConvert.DeserializeObject<List<Book>>((string)TempData["book"]);
            } catch { 
            
            }
            
            if (ViewData.Count()>=1)
            {
                return View(ViewData["books"]);
            }
            else
            {
                return View();
            }
        }

        // GET: BooksController/Create
        public ActionResult AddBook()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBook(Book b)
        {
            Book book = new Book();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(b), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("https://localhost:44324/api/Books/", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        book = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
            }
            return RedirectToAction("GetListofBooks");
        }

        // GET: BooksController/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            Book book = new Book();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44351/api/Books/GetBookById/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    book = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return View(book);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Book book)
        {
            Book receivedBook = new Book();
            using (var httpClient = new HttpClient())
            {
                long id = book.BookId;
                StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:44324/api/Books/" + id, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    receivedBook = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return RedirectToAction("GetListofBooks");
        }

        // GET: BooksController/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            TempData["bookid"] = id;
            Book book = new Book();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44351/api/Books/GetBookById/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    book = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return View(book);
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            int bookId = Convert.ToInt32(TempData["bookid"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:44324/api/books/" + bookId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("GetListofBooks");
        }

        [HttpGet]
        public async Task<IActionResult> SearchBookById(int id)
        {
            Book book =await GetBook(id);
            List<Book> books = new List<Book>();
            books.Add(book);
            TempData["book"] = JsonConvert.SerializeObject(books);
            
            return RedirectToAction("Search");
        }
        
        [HttpGet]
        public async Task<Book> GetBook(int id)
        {
            Book book = new Book();
            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44351/";
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Books/GetBookById/" + id);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<Book>(Response);
                }
            }
            return book;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookById(int id)
        {
            Book book = await GetBook(id);
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> SearchBookByIsbn(string isbn)
        {
            List<Book> Books = new List<Book>();

            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44351/";

                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Books/GetBookByIsbn/" + HttpUtility.UrlEncode(isbn));
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    Books = JsonConvert.DeserializeObject<List<Book>>(Response);
                }
                TempData["book"] = JsonConvert.SerializeObject(Books);
                return RedirectToAction("Search");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchBooksByTitle(string title)
        {
            List<Book> Books = new List<Book>();

            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44351/";
                
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Books/GetBooksByTitle/" + HttpUtility.UrlEncode(title));
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    Books = JsonConvert.DeserializeObject<List<Book>>(Response);
                }
                TempData["book"] = JsonConvert.SerializeObject(Books);
                return RedirectToAction("Search");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetListofBooks()
        {
            List<Book> BooksInfo = new List<Book>();

            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44351/";
                
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Books");
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    BooksInfo = JsonConvert.DeserializeObject<List<Book>>(Response);
                }
                
                return View(BooksInfo);
            }

        }



    }
}
