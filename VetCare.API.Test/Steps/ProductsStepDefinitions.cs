using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using SpecFlow.Internal.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using VetCare.API.Store.Resources;
using Xunit;

namespace VetCare.API.Test.Steps;

[Binding]
public sealed class ProductStepDefinitions : WebApplicationFactory<Program>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ProductStepDefinitions(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient Client { get; set; }
    private Uri BaseUri { get; set; }
    private Task<HttpResponseMessage> Response { get; set; }

    [Given(@"the Endpoint https://localhost:(.*)/api/v(.*)/products is available")]
    public void GivenTheEndpointHttpsLocalhostApiVProductsIsAvailable(int port, int version)
    {
        BaseUri = new Uri($"https://localhost:{port}/api/v{version}/products");
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions { 
            BaseAddress = BaseUri });
    }
    
    [When(@"a Post Request is sent")]
    public void WhenAPostRequestIsSent(Table saveProductResource)
    {
        var resource = saveProductResource.CreateSet<SaveProductResource>().First();
        var content = new StringContent(resource.ToJson(), Encoding.UTF8, 
            MediaTypeNames.Application.Json);
        Response = Client.PostAsync(BaseUri, content);
    }

    [Then(@"A Response is received with Status (.*)")]
    public void ThenAResponseIsReceivedWithStatus(int expectedStatus)
    {
        var expectedStatusCode = ((HttpStatusCode)expectedStatus).ToString();
        var actualStatusCode = Response.Result.StatusCode.ToString();
 
        Assert.Equal(expectedStatusCode, actualStatusCode);
    }
    
    [Then(@"a Product Resource is included in Response Body")]
    public async Task ThenAProductResourceIsIncludedInResponseBody(Table 
        expectedTutorialResource)
    {
        var expectedResource = expectedTutorialResource.CreateSet<ProductResource>().First();
        var responseData = await Response.Result.Content.ReadAsStringAsync();
        var resource = JsonConvert.DeserializeObject<ProductResource>(responseData);
        Assert.Equal(expectedResource.Name, resource.Name);
    }
    
    [Given(@"A Product is already stored")]
    public async void GivenAProductIsAlreadyStored(Table 
        saveProductResource)
    {
        var resource = saveProductResource.CreateSet<SaveProductResource>().First();
        var content = new StringContent(resource.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = Client.PostAsync(BaseUri, content);
        var responseData = await Response.Result.Content.ReadAsStringAsync();
        var responseResource = JsonConvert.DeserializeObject<ProductResource>(responseData);
        Assert.Equal(resource.Name, responseResource.Name);
    }

    [Then(@"An Error Message is returned in Response Body, with value ""(.*)""")]
    public void ThenAnErrorMessageIsReturnedWithValue(string 
        expectedMessage)
    {
        var message = Response.Result.Content.ReadAsStringAsync().Result;
        Assert.Equal(expectedMessage, message);
    }

}