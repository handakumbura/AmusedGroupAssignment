namespace AmusedGroupTechnicalAssignment;

using Microsoft.Playwright;
using System.Text.Json;

[TestClass]
public class TestGitHubAPI
{

    private IAPIRequestContext Request = null!;
    private static string objectId;
    

    [TestInitialize]
    public async Task SetUpAPITesting()
    {
        await CreateAPIRequestContext();
    }

    [TestCleanup]
    public async Task TearDownAPITesting()
    {
        await Request.DisposeAsync();
    }


    [TestMethod]
    public async Task Test1()
    {
        var resonse = await Request.GetAsync("objects");
        JsonElement responseContent = (JsonElement)await resonse.JsonAsync();

        //asserting the response status and the availability of a content body. 
        Assert.AreEqual(resonse.Status, 200, "The expected response status was not received.");
        Assert.IsTrue(responseContent.GetArrayLength()>3, "The expected response body content was not found.");

    }

    [TestMethod]
    public async Task Test2()
    {
        var payload = new Dictionary<string, object>();
        payload.Add("name", "book");
        payload.Add("data", new Dictionary<string, object> { { "author", "Frank Herbert" }, { "title", "Dune" }, {"price", 9.99} });
        var resonse = await Request.PostAsync("objects", new() { DataObject = payload });
        JsonElement responseContent = (JsonElement)await resonse.JsonAsync();

        //asserting the response
        Assert.AreEqual(resonse.Status, 200, "The expected response status was not received.");
        Assert.AreEqual("book", responseContent.GetProperty("name").ToString(), "The expected response body content was not found.");
        
        //storing the object id for request correlation.
        objectId = responseContent.GetProperty("id").ToString();
    }

    [TestMethod]
    public async Task Test3()
    {
        var resonse = await Request.GetAsync("objects/"+ await TestDataUtil());

        JsonElement responseContent = (JsonElement)await resonse.JsonAsync();

        //asserting the response.
        Assert.AreEqual(resonse.Status, 200, "The expected response status was not received.");
        Assert.AreEqual("Dune", responseContent.GetProperty("data").GetProperty("title").ToString(), "The expected response body content was not found.");
    }

    [TestMethod]
    public async Task Test4()
    {
        var resonse = await Request.GetAsync("objects/"+ await TestDataUtil());
        JsonElement responseContent = (JsonElement)await resonse.JsonAsync();

        //asserting the response.
        Assert.AreEqual(resonse.Status, 200, "The expected response status was not received.");
        Assert.AreEqual("Dune", responseContent.GetProperty("data").GetProperty("title").ToString(), "The expected response body content was not found.");
    }

    [TestMethod]
    public async Task Test5()
    {
        var resonse = await Request.DeleteAsync("objects/"+ await TestDataUtil());
        JsonElement responseContent = (JsonElement)await resonse.JsonAsync();

        //asserting the response.
        Assert.AreEqual(resonse.Status, 200, "The expected response status was not received.");
        Assert.AreEqual("Object with id = "+await TestDataUtil()+" has been deleted.",responseContent.GetProperty("message").ToString(), "The expected response body content was not found.");
    }

    //test utilities. 
    private async Task<string> TestDataUtil(){
        return objectId;
    }
    private async Task CreateAPIRequestContext()
    {
        var playwright = await Playwright.CreateAsync();
        Request = await playwright.APIRequest.NewContextAsync(new()
        {
            // All requests we send go to this API endpoint.
            BaseURL = "https://api.restful-api.dev/",
        });
    }
}