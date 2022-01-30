using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestingFirebase.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Auth;//for Firebase authentication
using FirebaseAdmin.Auth;
using System.Security.Claims;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace TestingFirebase.Controllers;//Miguel upgraded this to scoped namespaces -a C# 10 new feature

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    FirebaseAuthProvider _auth;
    string _FirebaseAPIKey = "AIzaSyCBGtzohPw6nkvQub6Bj4LqAOUBcDz0TRQ";

    FirebaseClient firebaseClient;
    // FirebaseAdmin.Auth. firebaseAdmin;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _auth = new FirebaseAuthProvider(
                     new FirebaseConfig(this._FirebaseAPIKey));
        firebaseClient = new FirebaseClient("https://pemdata-2f1d1-default-rtdb.firebaseio.com/");

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


        IReadOnlyCollection<FirebaseObject<Subcategory>> dbLogins = null;
        //Retrieve data from Firebase
        try
        {
            dbLogins = await firebaseClient
            .Child("categories")
            .OrderByKey()
            //.LimitToFirst(2)
            .OnceAsync<Subcategory>();
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, "NO DATA FOUND!\n" +
                "Error while querying Firebase" + " " + e.Message);
        }

        var mainCategoriesList = new List<MainCategory>();

        var mainKatIds = new string[] { "c1", "c2", "c3", "c4", "c5" };
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

            //if Firebase subcategories is empty, then make count zero
            if (dbLogins == null)
            {
                subCategoryCount = 0;
            }
            else
                subCategoryCount = dbLogins.Count(x => x.Object.SubId.Equals(val));
        }

        //Here dbLogins can be null since the Firebase table can be empty, so the view handles nulls
        // and makes number of categories 0
        @ViewBag.subCatList = dbLogins;

        return View(mainCategoriesList);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(LoginModel userModel)
    {
        string token = null;
        UserSession.token = null;
        UserSession.userName = null;
        string adminKey = "No Uer Role Defined";

        try
        {
            //create the user
            await _auth.CreateUserWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            //***********************************************************************************
            ///**********************************************************************************
            //UserRecordArgs args = new UserRecordArgs()
            //{
            //    Email = "katy@kat.com",
            //    EmailVerified = false,
            //    PhoneNumber = "+13055608045",
            //    Password = "remamasota",
            //    DisplayName = "Katy Kat",
            //    PhotoUrl = "http://www.example.com/12345678/photo.png",

            //};
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile("pemdata-2f1d1-firebase-adminsdk-i7d8f-55aad4b3ee.json"),
            //});


            //UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(args);
            //// See the UserRecord reference doc for the contents of userRecord.
            //Console.WriteLine($"Successfully created new user: {userRecord.Uid}");
         
            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Role, "Admin" },
                { "admin", true },
            };

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("pemdata-2f1d1-firebase-adminsdk-i7d8f-55aad4b3ee.json"),
                });
            }

            //retrived user just created in order to access the userId
            UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userModel.Email);
            
            // This will call the Firebase Auth server and set the custom claim for the user
            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                .SetCustomUserClaimsAsync(userRecord.Uid, claims);


            //refresh user variable so the user claims that were just added, show up
            userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userModel.Email);
            Console.WriteLine($"Name for this user: {userRecord.DisplayName}");

            string admin = null;
            try
            {
                adminKey = userRecord.CustomClaims[ClaimTypes.Role].ToString();
                //admin = userRecord.CustomClaims[admin].ToString();
            }
            catch (Exception e)
            {
                adminKey = $"no admin key was found: {e.Message}";
            }
            Console.WriteLine($"Admin key for this user: {adminKey}");

            //***********************************************************************************
            ///**********************************************************************************

            //log in the new user

            var fbAuthLink = await _auth
                                .SignInWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            token = fbAuthLink.FirebaseToken;

            //userModel.Email = userRecord.DisplayName;
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, "Error while trying to register an account\n" + e.Message );
        }

        //saving the token in a session variable
        if (token != null)
        {
            HttpContext.Session.SetString("_UserToken", token);

            //set model to display user logged info on menu
            UserSession.token = token;


            UserSession.userRole = adminKey;

            UserSession.userName = $"{userModel.Email} | {UserSession.userRole}";
 
            return RedirectToAction("Index");
        }
        else
        {
            return View();
        }
    }

    public IActionResult RegisterAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAdmin(LoginModel userModel)
    {
        string token = null;
        UserSession.token = null;
        UserSession.userName = null;


        try
        {
            //create the user
            await _auth.CreateUserWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            //***********************************************************************************
            ///**********************************************************************************
            //UserRecordArgs args = new UserRecordArgs()
            //{
            //    Email = "katy@kat.com",
            //    EmailVerified = false,
            //    PhoneNumber = "+13055608045",
            //    Password = "remamasota",
            //    DisplayName = "Katy Kat",
            //    PhotoUrl = "http://www.example.com/12345678/photo.png",

            //};
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile("pemdata-2f1d1-firebase-adminsdk-i7d8f-55aad4b3ee.json"),
            //});


            //UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(args);
            //// See the UserRecord reference doc for the contents of userRecord.
            //Console.WriteLine($"Successfully created new user: {userRecord.Uid}");

            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Role, "Admin" }
            };
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("pemdata-2f1d1-firebase-adminsdk-i7d8f-55aad4b3ee.json"),
            });
            //retrived user just created in order to access the userId
            UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userModel.Email);
            // This will call the Firebase Auth server and set the custom claim for the user
            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
                .SetCustomUserClaimsAsync(userRecord.Uid, claims);

            //refresh user variable so the user claims that were just added, show up
            userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userModel.Email);
            Console.WriteLine($"Name for this user: {userRecord.DisplayName}");

            string adminKey;
            try
            {
                adminKey = userRecord.CustomClaims[ClaimTypes.Role].ToString();
            }
            catch (Exception e)
            {
                adminKey = $"no admin key was found: {e.Message}";
            }
            Console.WriteLine($"Admin key for this user: {adminKey}");

            //***********************************************************************************
            ///**********************************************************************************

            //log in the new user

            var fbAuthLink = await _auth
                                .SignInWithEmailAndPasswordAsync(userModel.Email, userModel.Password);
            token = fbAuthLink.FirebaseToken;

            //userModel.Email = userRecord.DisplayName;
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, "Error while trying to register an account\n" + e.Message);
        }

        //saving the token in a session variable
        if (token != null)
        {
            HttpContext.Session.SetString("_UserToken", token);

            //set model to display user logged info on menu
            UserSession.token = token;
            UserSession.userName = userModel.Email;

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
    [ValidateAntiForgeryToken]
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
            UserSession.token = token;
           
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("pemdata-2f1d1-firebase-adminsdk-i7d8f-55aad4b3ee.json"),
                    
                });
            }

            UserRecord user = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userModel.Email);
            string adminKey;
            try
            {

               adminKey = user.CustomClaims[ClaimTypes.Role].ToString();
            }
            catch(Exception) 
            {
               adminKey = "no admin key was found";
            }
            Console.WriteLine($"Admin key for this user: {adminKey}");


            UserSession.userRole = adminKey;

            UserSession.userName = $"{userModel.Email} | {UserSession.userRole}" ;



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

    public async Task<IActionResult> LoginDemoUser()
    {
        string token = null;
        UserSession.token = null;
        UserSession.userName = null;

//        FirebaseApp.Create(new AppOptions()
//        {
//            Credential = GoogleCredential.FromAccessToken(_FirebaseAPIKey),
//            a
            
//        });

//        // Create the custom user claim that has the role key
//        var claims = new Dictionary<string, object>
//{
//    { ClaimTypes.Role, "Administrator" }
//};

//        // This will call the Firebase Auth server and set the custom claim for the user
//        await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance
//            .SetCustomUserClaimsAsync("nVaIpgdDtcSFCY6HpwJ2ElqiM0S2", claims);


        //log in the user
        try
        {
            var fbAuthLink = await _auth.SignInAnonymouslyAsync();

            token = fbAuthLink.FirebaseToken;
        }
        catch (Exception e)
        {
            //ModelState.AddModelError(string.Empty, ex.Message);
            ModelState.AddModelError(string.Empty, "Wrong email or password!");
            Console.WriteLine(e.Message + " Miguel Here");
        }

        //saving the token in a session variable
        if (token != null)
        {
            HttpContext.Session.SetString("_UserToken", token);
            UserSession.token = token;
            UserSession.userName = "Admin Demo User";
            UserSession.userRole = "Admin";

            return RedirectToAction("Index");
        }
        else
        {
            return RedirectToAction("Login");
        }
    }

}


