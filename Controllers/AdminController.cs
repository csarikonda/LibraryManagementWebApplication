using LibraryManagementWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LibraryManagementWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISession session;
        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            this.session = httpContextAccessor.HttpContext.Session;
        }
        // GET: AdminController
        [HttpGet]
        public ActionResult Index(string loginError=null)
        {
            ViewBag.LoginError = loginError;
            return View();
        }

        // GET: AdminController/Details/5
        [HttpGet]
        public async  Task<IActionResult> Profile()
        {
            string email = HttpUtility.UrlEncode(HttpContext.Session.GetString("email"));
            List<Admin> admins = new List<Admin>();

            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44324/";

                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Admins/GetAdminByEmail/" + email );
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    admins = JsonConvert.DeserializeObject<List<Admin>>(Response);
                    if (admins.Count() == 1)
                    {
                        return View(admins[0]);
                    }
                }
            }
            return View();
        }

        // GET: AdminController/Create
        public ActionResult AddAdmin(string addAdminError=null)
        {
            ViewBag.addAdminError = addAdminError;
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAdmin(Admin a)
        {
            Admin admin = new Admin();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

                var mailResponse = await httpClient.GetAsync("https://localhost:44324/api/Admins/GetAdminByEmail/" + HttpUtility.UrlEncode(a.Mail) );

                if (mailResponse.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("AddAdmin", "Admin", new { addAdminError = "Email Id Already Exists" });
                }
                else
                {
                    ViewBag.addAdminError = null;
                    using (var response = await httpClient.PostAsync("https://localhost:44324/api/Admins/", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        admin = JsonConvert.DeserializeObject<Admin>(apiResponse);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // GET: AdminController/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            TempData["adminId"] = id;
            Admin admin = new Admin();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:44324/api/Admins/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                admin = JsonConvert.DeserializeObject<Admin>(apiResponse);
            }
            return View(admin);
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Admin admin)
        {
            int adminId = Convert.ToInt32(TempData["adminId"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:44324/api/Admins/" + adminId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            List<Admin> AdminsInfo= new List<Admin>();
            using (var client=new HttpClient())
            {
                string BaseUrl = "https://localhost:44324/";
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Admins/GetAdminsExcept/" + HttpUtility.UrlEncode(HttpContext.Session.GetString("email")));
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    AdminsInfo = JsonConvert.DeserializeObject<List<Admin>>(Response);
                }

                return View(AdminsInfo);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Login(string email, string password)
        {
            using (var client = new HttpClient())
            {
                string Baseurl = "https://localhost:44351/";
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Admins/AuthenticateAdmin/" + HttpUtility.UrlEncode(email) + "/" + HttpUtility.UrlEncode(password));
                if (Res.StatusCode==HttpStatusCode.OK)
                {
                    HttpContext.Session.SetString("email",email);
                    return RedirectToAction("Index","Home");
                }
            }
            
            return RedirectToAction("Index","Admin",new { loginError = "Enter Valid Credentials" });
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
