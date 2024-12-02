using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Helpers.Tests.Helpers;
using Tests.Helpers.Token;

namespace Tests.WebAPI
{
    [TestFixture]
    public class HotelsControllerTests : BaseIntegrationTest
    {
        [Test]
        public async Task GetList_Should_Return_OK()
        {
            const string requestUri = "api/hotels/getall";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetHotelsManagers_Should_Return_OK()
        {
            const string requestUri = "api/hotels/gethotelsmanagers";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetById_Should_Return_OK()
        {
            const string requestUri = "api/hotels/getbyid?id=sample-guid-id";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Add_Should_Return_OK()
        {
            const string requestUri = "api/hotels";

            // Arrange
            var createHotelCommand = new
            {
                Name = "Sample Hotel",
                Address = "Sample Address"
            };

            var content = TestHelper.GetJsonContent(createHotelCommand);

            // Act
            var response = await HttpClient.PostAsync(requestUri, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Update_Should_Return_OK()
        {
            const string requestUri = "api/hotels";

            // Arrange
            var updateHotelCommand = new
            {
                Id = "sample-guid-id",
                Name = "Updated Hotel Name",
                Address = "Updated Address"
            };

            var content = TestHelper.GetJsonContent(updateHotelCommand);

            // Act
            var response = await HttpClient.PutAsync(requestUri, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Delete_Should_Return_OK()
        {
            const string requestUri = "api/hotels";

            // Arrange
            var deleteHotelCommand = new
            {
                Id = "sample-guid-id"
            };

            var content = TestHelper.GetJsonContent(deleteHotelCommand);

            // Act
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative),
                Content = content
            };
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
