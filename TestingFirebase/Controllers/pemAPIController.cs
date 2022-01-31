//using Firebase.Database;
//using Firebase.Database.Query;
//using Microsoft.AspNetCore.Mvc;
//using TestingFirebase.Models;

//For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace TestingFirebase.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class pemAPIController : ControllerBase
//{

//    FirebaseClient firebaseClient = new FirebaseClient("https://pemdata-2f1d1-default-rtdb.firebaseio.com/");

//    GET: api/<pemAPIController>
//        [HttpGet]
//    public async Task<List<String>> Get()
//    {
//        IReadOnlyCollection<FirebaseObject<Subcategory>> dbLogins = null;
//        Retrieve data from Firebase
//            try
//        {
//            dbLogins = await firebaseClient
//            .Child("categories")
//            .OrderByKey()
//            .LimitToFirst(2)
//            .OnceAsync<Subcategory>();
//        }
//        catch (Exception e)
//        {
//            ModelState.AddModelError(string.Empty, "NO DATA FOUND!\n" +
//                "Error while querying Firebase" + " " + e.Message);
//        }

//        var subList = new List<String>();
//        foreach (var dbLog in dbLogins)
//        {
//            subList.Add(dbLog.Object.Title);

//        }

//        return subList;
//        return new string[] { "Katy Perry Ta reguena, gon", "Katy kat las tiene enormes!" };
//    }

//    GET api/<pemAPIController>/5
//        [HttpGet("{id}")]
//    public string Get(int id)
//    {
//        return "value";
//    }

//    POST api/<pemAPIController>
//        [HttpPost]
//    public void Post([FromBody] string value)
//    {
//    }

//    PUT api/<pemAPIController>/5
//        [HttpPut("{id}")]
//    public void Put(int id, [FromBody] string value)
//    {
//    }

//    DELETE api/<pemAPIController>/5
//        [HttpDelete("{id}")]
//    public void Delete(int id)
//    {
//    }
//}
