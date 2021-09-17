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
        public ActionResult SearchById(Book book)
        {
            var url = (TempData["book"] as Book)?.ThumbnailUrl;
            if (book.ThumbnailUrl != null)
            {
                return View(book);
            }
            else
            {
                return View();
            }
        }
        
        // GET: BooksController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BooksController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Book book = new Book();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44351/api/Books/" + id))
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
                using (var response = await httpClient.PutAsync("https://localhost:44351/api/Books/" + id, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    receivedBook = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return RedirectToAction("GetListofBooks");
        }

        // GET: BooksController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchBookById(int id)
        {
            Book book =await GetBook(id);
            book.Authors = null;
            book.Categories = null;
            book.Isbn = null;
            book.LongDescription = null;
            book.ShortDescription = null;
            book.PublishedDate = null;

            TempData["book"] = book;

            return RedirectToAction("SearchById");
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
        public async Task<IActionResult> GetBooksByTitle(string title)
        {
            List<Book> BooksInfo = new List<Book>();

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
                    BooksInfo = JsonConvert.DeserializeObject<List<Book>>(Response);
                }

                return RedirectToAction("SearchById", BooksInfo);
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
