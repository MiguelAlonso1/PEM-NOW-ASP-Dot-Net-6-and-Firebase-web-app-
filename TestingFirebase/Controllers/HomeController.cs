using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestingFirebase.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Auth;//for Firebase authentication
using Microsoft.AspNetCore.Http;//for set string on http context session line (119)

namespace TestingFirebase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FirebaseAuthProvider _auth;
        string _FirebaseAPIKey = "AIzaSyB9-PR4nCk8T6nNtqvnMhYFLyxr7ZLXJV8";
        FirebaseClient firebaseClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _auth = new FirebaseAuthProvider(
                         new FirebaseConfig(this._FirebaseAPIKey));
            firebaseClient = new FirebaseClient("https://med-app-519aa.firebaseio.com/");
        }


        public async Task<IActionResult> Index()
        {
            //code below checks for user authentication; redirects to login if user is not logged in
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return RedirectToAction("Login");
            }

            //var result = await firebaseClient
            //.Child("Users/" + userId + "/Logins")
            //.PostAsync(currentUserLogin);
        

            //Retrieve data from Firebase
            var dbLogins = await firebaseClient
            .Child("categories")
            .OrderByKey()
            //.LimitToFirst(2)
            .OnceAsync<Subcategory>();

            var mainCategoriesList = new List<MainCategory>();

            var mainKatIds  = new string []{"c1", "c2","c3","c4","c5"};
            var subCategoryCount = new int();
            /// Convert JSON da ta to original datatype, then make a MainCategory and add it to list

            //foreach (var mainCategory in dbLogins)
            //{
            //    mainCategoriesList.Add(new MainCategory
            //    {
            //      Key = Convert.ToString(mainCategory.Key),
            //      Title = Convert.ToString(mainCategory.Object.Title),
            //        CategoryDescription = Convert.ToString(mainCategory.Object.CategoryDescription),
            //      ImageUrl = @"\images\img.png"//Verbatim string for pic
            //    });

            //    Console.WriteLine($"{mainCategory.Key} is");
            //    Console.WriteLine($"Evaluation is {mainCategory.Object.CategoryDescription}");
            //    Console.WriteLine();
            //}
            foreach (var val in mainKatIds)
            {
                mainCategoriesList.Add(new MainCategory
                {
                    Key = val,//use to make asp-route-id for subcategories
                    //Switch expression, baby!
                    Title = val switch
                    {
                        "c1" => "Medical",
                        "c2" => "Surgical",
                        "c3" => "Trauma",
                        "c4" => "Toxicology",
                        "c5" => "Foreign Ingestion",
                        _ => "Medical",//default case
                    },
                    CategoryDescription = "Insert description",
                    ImageUrl = @"\images\" + val + ".jpg"//Verbatim string and konkat for pic
                });

           

                //var subCategoryObj = subCatList;
                // var subCategoryObj = dbLogins.Where(x => x.Object.SubId.Equals(id));
                subCategoryCount = dbLogins.Count(x => x.Object.SubId.Equals(val));
            }

            @ViewBag.subCatList = dbLogins;

                return View(mainCategoriesList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginModel userModel)
        {
            //create the user
            await _auth.CreateUserWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            //log in the new user
            var fbAuthLink = await _auth
                            .SignInWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            string token = fbAuthLink.FirebaseToken;
            //saving the token in a session variable
            if (token != null)
            {
                HttpContext.Session.SetString("_UserToken", token);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel userModel)
        {
            string token = null;
            UserSession.token = null;
            UserSession.userName = null;
      
            //log in the user
            try
            {
                var fbAuthLink = await _auth
                                .SignInWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
                token = fbAuthLink.FirebaseToken;
            }
            catch (Exception)
            {
                //ModelState.AddModelError(string.Empty, ex.Message);
                ModelState.AddModelError(string.Empty, "Wrong email or password!");
            }

            //saving the token in a session variable
            if (token != null)
            {
                HttpContext.Session.SetString("_UserToken", token);
                UserSession.token =  token;
                UserSession.userName = userModel.Email;
                //if (!ModelState.IsValid)
                //{
                //    return View();
                //}
                return RedirectToAction("Index");
            }
            else
            {
                    return View();
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            UserSession.token = null;
            UserSession.userName = null;
            return RedirectToAction("Login");
        }
    }
}
