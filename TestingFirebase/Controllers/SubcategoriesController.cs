﻿using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestingFirebase.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TestingFirebase.Controllers;//Miguel upgraded this to scoped namespaces -a C# 10 new feature

public class SubcategoriesController : Controller
    {
        FirebaseClient firebaseClient;

        public SubcategoriesController()
        {
            firebaseClient = new FirebaseClient("https://pemdata-2f1d1-default-rtdb.firebaseio.com/");
        }
        public async Task<IActionResult> Index(string id)
        {
            //code below checks for user authentication; redirects to login if user is not logged in
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return RedirectToAction("Login", "home");
            }

            //var result = await firebaseClient
            //.Child("Users/" + userId + "/Logins")
            //.PostAsync(currentUserLogin);

            //Retrieve data from Firebase

            IReadOnlyCollection<FirebaseObject<Subcategory>> subCatList = null;

            try
            {
               subCatList = await firebaseClient
           .Child("categories")
           .OrderByKey()
           .OnceAsync<Subcategory>();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "NO DATA FOUND!\n" +
                    "Error while querying Firebase" + " " + e.Message);
            }

            //var subCategoryObj = subCatList;
            //filter result query to only the choosen main medical kategory (id)


            IEnumerable<FirebaseObject<Subcategory>> subCategoryObj = null;

            if (subCatList != null)
           
                subCategoryObj = subCatList.Where(x => x.Object.SubId.Equals(id));
           

            var subCategoryList = new List<Subcategory>();

            //******************************
            string[] picURL = null;
            picURL = new string[] { @"\images\img.png" };//gets default pic in case no pics were uploaded on the app

            //this is because the Firebase database can be empty
            if (subCategoryObj != null)
            {
                /// Convert JSON data to original datatype
                foreach (var subCategory in subCategoryObj)
                {

                    //process pictures first
                    if (subCategory.Object.Image != null)//image here is and array of JSON pictures!!
                    {
                        if (!subCategory.Object.Image.ToString().Equals("image"))//if equals to "image" means no pic,
                        {
                            //below converts JSON array into string array- firebaseObj.Object.Image references a JSON array
                            picURL = JsonSerializer.Deserialize<string[]>(Convert.ToString(subCategory.Object.Image));
                        }
                        //viewbag gets reference to the string array of Base 64 enconded pics
                        //this is a super long string of characters that are put on the "scr" attributes of img tags
                        //the browser knows these are encoded pics and renders them as images on the websites

                    }

                    subCategoryList.Add(new Subcategory
                    {
                        Key = Convert.ToString(subCategory.Key),
                        Title = Convert.ToString(subCategory.Object.Title),
                        SubId = Convert.ToString(subCategory.Object.SubId),
                        Image = picURL[0]//only a pic will be required for datatables so element 0 of array must be dereferenced
                    });

                    //PicURL needs to be reset to the placeholder picture; otherwise the image from the previous element
                    //can be mistakenly assigned to a next object that may not be supposed to have one!!!
                    picURL = new string[] { @"\images\img.png" };
                }
            }


           
            @ViewBag.MainCategoryName = id switch//switch expression
            {
                "c1" => "Medical",
                "c2" => "Surgical",
                "c3" => "Trauma",
                "c4" => "Toxicology",
                "c5" => "Foreign Ingestion",
                _ => "Medical",//default case
            };
            //subcaterolyList can be empty since Firebase can be empty
            return View(subCategoryList);
        }

        public async Task<IActionResult> Details(string id)
        {
            //code below checks for user authentication; redirects to login if user is not logged in
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return RedirectToAction("Login", "home");
            }

            //var result = await firebaseClient
            //.Child("Users/" + userId + "/Logins")
            //.PostAsync(currentUserLogin);

            //Retrieve ALL Kategories from Firebase
            var subCatList = await firebaseClient
            .Child("categories")
            .OrderByKey()
            .OnceAsync<Subcategory>();

            var firebaseObj = subCatList.FirstOrDefault(x => x.Key.Equals(id));
            string[] picURL = null;
            ViewBag.picURL = new string[] { @"\images\img.png" };

            /// Convert JSON data to original datatype
            if (firebaseObj.Object.Image != null)
            {
                if (!firebaseObj.Object.Image.ToString().Equals("image"))
                {
                    //below converts JSON array into string array- firebaseObj.Object.Image references a JSON array
                    picURL = JsonSerializer.Deserialize<string[]>(Convert.ToString(firebaseObj.Object.Image));
                }
                if (picURL != null)
                    ViewBag.picURL = picURL;//viewbag gets reference to the string array
            }

            Subcategory obj = new Subcategory
            {
                Key = Convert.ToString(firebaseObj.Key),
                SubId = Convert.ToString(firebaseObj.Object.SubId),
                Title = Convert.ToString(firebaseObj.Object.Title),
             
                Evaluation = Convert.ToString(firebaseObj.Object.Evaluation),
                Management = Convert.ToString(firebaseObj.Object.Management),
                Medications = Convert.ToString(firebaseObj.Object.Medications),
                Signs = Convert.ToString(firebaseObj.Object.Signs),
                References = Convert.ToString(firebaseObj.Object.References)
            };

            @ViewBag.subCatList = 0;

            @ViewBag.MainCategoryName = firebaseObj.Object.SubId switch//switch expression
            {
                "c1" => "Medical",
                "c2" => "Surgical",
                "c3" => "Trauma",
                "c4" => "Toxicology",
                "c5" => "Foreign Ingestion",
                _ => "Medical",//default case
            };
            return View(obj);
        }

        //GET-Create
        public IActionResult Create()
        {
            #region :::Authentication validation:::
            //code below checks for user authentication; redirects to login if user is not logged in
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return RedirectToAction("Login", "home");
            }
            #endregion

            var mainKatArray = new string[] { "c1", "c2", "c3", "c4", "c5" };

            IEnumerable<SelectListItem> mainKategoryNameValues =
                mainKatArray.Select(x => new SelectListItem
                {
                    Text = x switch
                    {
                        "c1" => "Medical",
                        "c2" => "Surgical",
                        "c3" => "Trauma",
                        "c4" => "Toxicology",
                        "c5" => "Foreign Ingestion",
                        _ => "Medical",//default case notation
                    },
                    Value = x
                });

            //ViewBag is automatically passed to the view
            ViewBag.MainKategoryListOfNames = mainKategoryNameValues;

            return View();//returns the HTML form
        }

        #region ::Http is Web access. Web access benefits from asynchronous programming::
        // need to research this
        #endregion
        //POST-Create-always use POST for delete update and authentication for security reasons!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subcategory obj)//obj comes form the HTML form
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            //process pic*******************************
            /*
           Essentially, the image needs to be interpreted as an array of bytes
           */
            var ms = new MemoryStream();

            await obj.Pictures.CopyToAsync(ms);
            var fileBytes = ms.ToArray();//var here is a byte[]

            #region ::: Data URLs :::
            //line below sets the raw image as a Data URL, which web browsers understand
            //Data URLs are composed of four parts: a prefix (data:), a MIME type indicating the type of data,
            //an optional base64 token if non-textual, and the data itself:
            //data:[<mediatype>][;base64],<data>
            //final encoded pic will look something like this:
            //data:image/jpeg;base64,\"/9j/4QAYRXhpZgAASUkqAAgAA... a very long string of characters follow...
            #endregion
            string s = $"data:{obj.Pictures.ContentType};base64,{Convert.ToBase64String(fileBytes)}";

            //encoded image needs to be put in a string array since the image object in Firebase
            //represents an array, not and image itself!
            string[] newImgsArray = { s };

            //****************assign processed picture to new object in parameter
            obj.Image = newImgsArray;// Fireabase Image represents an array of pics not a single pic!
            //******************************************

            //*********************************!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          //!!!!!!!!!!!!!!!!!!!!  WARNING!!!!!!!!!!!!!!!!
          //Creating new object in Firebase runs the risk of breaking the whole application
          //      There's a risk of introducing a new field or updating a data type or removing a variable
          //      the problem is Thart firebase takes the new object and it DOES NOT check for data integryty
          //      it just takes the object as it switch, but then the app gets tottaly crippled cuz teh read operations
          //      clashes with the new or missing fields, rendering the whole app totally useless
          
                  //new object has to be build on the fly***************
                  //cuz I needed to remove the pictures variable

           
            //****************************************************
            try
            {
                var result = await firebaseClient
           .Child("categories")
            .PostAsync(new Subcategory
            {
                //Key = Convert.ToString(firebaseObj.Key),
                SubId = Convert.ToString(obj.SubId),
                Title = Convert.ToString(obj.Title),
                Evaluation = Convert.ToString(obj.Evaluation),
                Management = Convert.ToString(obj.Management),
                Medications = Convert.ToString(obj.Medications),
                Signs = Convert.ToString(obj.Signs),
                References = Convert.ToString(obj.References),
                Image = newImgsArray// Fireabase Image represents an array of pics not a single pic!
            });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Issue updating!\n" +
                    "Error while inserting new record Firebase" + " " + e.Message);
            }

            //to the Index in Categories since this controller is in Categories
            return RedirectToAction("Index", new { Id = obj.SubId });
        }

        //GET-Update
        public async Task<IActionResult> Update(string id)//obj comes form the HTML form
        {
            //code below checks for user authentication; redirects to login if user is not logged in
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return RedirectToAction("Login", "home");
            }

            if (id == null)
            {
                return NotFound();
            }

            //Retrieve ALL Kategories from Firebase
            var subCatList = await firebaseClient
            .Child("categories")
            .OrderByKey()
            .OnceAsync<Subcategory>();

            //Try to find object in database by string id/Key from incomming parameter
            var firebaseObj = subCatList.FirstOrDefault(x => x.Key.Equals(id));

            if (firebaseObj == null)
            {
                return NotFound();
            }

            /// Convert JSON data to original datatype
            Subcategory obj = new Subcategory
            {
                Key = Convert.ToString(firebaseObj.Key),
                SubId = Convert.ToString(firebaseObj.Object.SubId),
                Title = Convert.ToString(firebaseObj.Object.Title),
             
                Evaluation = Convert.ToString(firebaseObj.Object.Evaluation),
                Management = Convert.ToString(firebaseObj.Object.Management),
                Medications = Convert.ToString(firebaseObj.Object.Medications),
                Signs = Convert.ToString(firebaseObj.Object.Signs),
                References = Convert.ToString(firebaseObj.Object.References)
            };

            //In case obj from Firebase doesn't have a subKat
            if (string.IsNullOrEmpty(obj.SubId))
            {
                obj.SubId = "c1";
            }

            var mainKatArray = new string[] { obj.SubId };

            IEnumerable<SelectListItem> mainKategoryNameValues =
                mainKatArray.Select(x => new SelectListItem
                {
                    Text = x switch
                    {
                        "c1" => "Medical",
                        "c2" => "Surgical",
                        "c3" => "Trauma",
                        "c4" => "Toxicology",
                        "c5" => "Foreign Ingestion",
                        _ => "Medical",//default case
                    },
                    Value = x
                });

            //ViewBag is automatically passed to the view
            ViewBag.MainKategoryListOfNames = mainKategoryNameValues;
            return View(obj);
        }

        #region :::Http is Web access. Web access benefits from asynchronous programming:::
        // need to research this
        #endregion

        //POST-Update-always use POST for delete update and authentication for security reasons!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Subcategory obj)//obj comes form the HTML form
        {
            if (obj == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            #region ::: Warning on Updating records on Firebase :::
            //For the update below, the object needed to be rebuild on the fly. The primary Key: obj.Key
            //was being inserted as a new column in Firebase! Firebase didn't enforce the original structure of Categories,
            //ie, it'd allow for any new field to be inserted or deleted. So, the re-created object below is just the same
            //as the obj passed in as the argument for this update sans the Key!
            #endregion

            /*
             Essentially, the image needs to be interpreted as an array of bytes
             */
            var ms = new MemoryStream();

            await obj.Pictures.CopyToAsync(ms);
            var fileBytes = ms.ToArray();//var here is a byte[]

            #region ::: Data URLs :::
            //line below sets the raw image as a Data URL, which web browsers understand
            //Data URLs are composed of four parts: a prefix (data:), a MIME type indicating the type of data,
            //an optional base64 token if non-textual, and the data itself:
            //data:[<mediatype>][;base64],<data>
            //final encoded pic will look something like this:
            //data:image/jpeg;base64,\"/9j/4QAYRXhpZgAASUkqAAgAA... a very long string of characters follow...
            #endregion
            string s = $"data:{obj.Pictures.ContentType};base64,{Convert.ToBase64String(fileBytes)}";

            //encoded image needs to be put in a string array since the image object in Firebase
            //represents an array, not and image itself!
            string[] newImgsArray = { s };

            // act on the Base64 data

            //Update in Firebase returns void/nothing?
            await firebaseClient
           .Child("categories")
           .Child(obj.Key)
            .PutAsync(new Subcategory
            {
                //Key = Convert.ToString(firebaseObj.Key),
                SubId = Convert.ToString(obj.SubId),
                Title = Convert.ToString(obj.Title),
         
                Evaluation = Convert.ToString(obj.Evaluation),
                Management = Convert.ToString(obj.Management),
                Medications = Convert.ToString(obj.Medications),
                Signs = Convert.ToString(obj.Signs),
                References = Convert.ToString(obj.References),
                Image = newImgsArray// Fireabase Image represents an array of pics not a single pic!
            });

            #region ::The second parameter is just part...::
            /*
            the second parameter is just part of an object called route values. Id is a part of it
            would look something like this
            return RedirectToAction( "Main", new RouteValueDictionary( 
            new { controller = controllerName, action = "Main", Id = Id } ) );
            */
            #endregion

            //to the Index in Categories since this controller is in Categories
            return RedirectToAction("Details", new { Id = obj.Key });
        }

        #region :::GET Delete:::
        //GET-Delete
        //public async Task<IActionResult> Delete(string id)//obj comes form the HTML form
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    //Retrieve ALL Kategories from Firebase
        //    var subCatList = await firebaseClient
        //    .Child("categories")
        //    .OrderByKey()
        //    .OnceAsync<Subcategory>();


        //    //Try to find object in database by string id/Key from incomming parameter
        //    var firebaseObj = subCatList.FirstOrDefault(x => x.Key.Equals(id));

        //    if (firebaseObj == null)
        //    {
        //        return NotFound();
        //    }
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    MainCategory mainKategoryList =
        //        _db.MainCategories.FirstOrDefault(x => x.Id == obj.MainCategoryId);
        //    if (mainKategoryList == null)
        //    {
        //        return NotFound();
        //    }

        //    SelectListItem mainKategorySelectList =
        //         new SelectListItem
        //         {
        //             Text = mainKategoryList.CategoryName,
        //             Value = mainKategoryList.Id.ToString()
        //         };

        //    #region ::The below list is needed since...::
        //    /*The below list is needed since the HTML select list asp-items control
        //    wants an object that resolves as an IEnumerable so it can iterate throgh it
        //   to render the option HMTL tags for some reason creating a IEnumerable variable
        //   to assign it the list item didn't work. Maybe cuz it's an abstract object
        //   The list below resolves as an IEnumerable so this worked
        //    */
        //    #endregion
        //    List<SelectListItem> sl = new List<SelectListItem>();
        //    sl.Add(mainKategorySelectList);

        //    //ViewBag is automatically passed to the view
        //    ViewBag.MainKategorySelectList = sl;

        //    return View(obj);
        //}

        //POST-Delete-always use POST for delete update and authentication for security reasons!
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)//obj comes form the HTML form
        {
            if (id == null)
            {
                return NotFound();
            }

            //Retrieve ALL Kategories from Firebase
            var subCatList = await firebaseClient
            .Child("categories")
            .OrderByKey()
            .OnceAsync<Subcategory>();

            //Try to find object in database by string id/Key from incomming parameter
            var firebaseObj = subCatList.FirstOrDefault(x => x.Key.Equals(id));

            if (firebaseObj == null)
            {
                return NotFound();
            }

            //save foreing key ID before deleting so we can redirect back to index for the current subcategory index
            string MainCategoryKeyID = firebaseObj.Object.SubId;

            //Delete in Firebase returns void/nothing? 
            await firebaseClient
           .Child("categories")
           .Child(firebaseObj.Key)
           .DeleteAsync();

            //to the Index in Categories since this controller is in Categories
            return RedirectToAction("Index", new { id = MainCategoryKeyID });
        }
    }