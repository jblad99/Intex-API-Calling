using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INTEX_API_Calling.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace INTEX_API_Calling.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(float depth, string sex, string adultchild, string goods, string wrapping)
    {
        // Set the API endpoint URL
        string apiUrl = "http://52.70.13.136/predict";

        int adultsubadult_C;
        int goods_Other;
        int wrapping_H;
        int wrapping_W;

        // Create an input object from the user-provided data
        if (adultchild == "Adult")
        {
            adultsubadult_C = 0;
        }
        else
        {
            adultsubadult_C = 1;

        }

        if (goods == "Yes")
        {
            goods_Other = 1;
        }
        else
        {
            goods_Other = 0;

        }

        if (wrapping == "Whole")
        {
            wrapping_H = 0;
            wrapping_W = 1;
        }
        else
        {
            wrapping_H = 1;
            wrapping_W = 0;
        }

        var inputData = new { depth = depth, adultsubadult_C = adultsubadult_C, goods_Other = goods_Other, wrapping_H = wrapping_H, wrapping_W = wrapping_W };

        // Serialize the input object to JSON
        string inputDataJson = JsonConvert.SerializeObject(inputData);

        // Set the content type and the input data in the request body
        StringContent content = new StringContent(inputDataJson, System.Text.Encoding.UTF8, "application/json");

        // Send the POST request to the API endpoint and wait for the response
        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
        string responseContent = await response.Content.ReadAsStringAsync();

        // Check if the request was successful (status code 200)
        if (response.IsSuccessStatusCode)
        {
            // Deserialize the JSON response into a result object
            ModelResult ResultFromApi = JsonConvert.DeserializeObject<ModelResult>(responseContent);

            // Pass the prediction result to the view and render it

            //TempData["Result"] = Result;

            return RedirectToAction("MyResult", ResultFromApi);

        }
        else
        {
            // Pass the error message to the view and render it
            string errorMessage = $"Prediction API error ({response.StatusCode}): {responseContent}";
            return View("PredictionError", errorMessage);
        }
    }

    [HttpGet]
    public ActionResult MyResult(int ResultFromApi)
    {
        string ResultString = "";

        if (ResultFromApi == 1)
        {
            ResultString = "West";
        }
        else
        {
            ResultString = "East";
        }

        PredictionResult result = new PredictionResult { Result = ResultString };
        return View(result);
    }

    //[HttpGet]
    //public ActionResult MyResult()
    //{
    //    // Retrieve the result from the session
    //    var resultFromTempData = (string)TempData["Result"];

    //    // Create a new instance of MyViewModel and set its properties
    //    var myMummyResult = new MummyResult
    //    {
    //        Result = resultFromTempData
    //    };

    //    // Pass the view model to the view
    //    return View(myMummyResult);
    //}

    //Define a class to deserialize the JSON response into
    public class ModelResult
    {
        public float Prediction { get; set; }
    }
}