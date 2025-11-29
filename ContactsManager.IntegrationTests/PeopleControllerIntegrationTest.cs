using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace ContactsManager.Test
{
    public class PeopleControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PeopleControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            // Act
            HttpResponseMessage httpResponse = await _client.GetAsync("/people/");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            
            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            var document = htmlDocument.DocumentNode;
            document.QuerySelector(".people").Should().NotBeNull();
        }
        #endregion
    }
}
