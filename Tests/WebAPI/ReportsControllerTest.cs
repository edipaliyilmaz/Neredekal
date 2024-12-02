using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Helpers;
using Tests.Helpers.Tests.Helpers;
using Tests.Helpers.Token;

namespace Tests.WebAPI
{
    [TestFixture]
    public class ReportsControllerTests : BaseIntegrationTest
    {
        [Test]
        public async Task GetList_Should_Return_OK()
        {
            const string requestUri = "api/reports/getall";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetById_Should_Return_OK()
        {
            const string requestUri = "api/reports/getbyid?id=sample-guid-id";

            // Act
            var response = await HttpClient.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task RequestReport_Should_Return_OK()
        {
            const string requestUri = "api/reports/RequestReport";

            // Act
            var response = await HttpClient.PostAsync(requestUri, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Add_Should_Return_OK()
        {
            const string requestUri = "api/reports";

            var createReportCommand = new
            {
                Title = "Test Report",
                Content = "Sample report content",
                CreatedDate = "2024-12-01"
            };

            var content = TestHelper.GetJsonContent(createReportCommand);

            // Act
            var response = await HttpClient.PostAsync(requestUri, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Update_Should_Return_OK()
        {
            const string requestUri = "api/reports";

            var updateReportCommand = new
            {
                Id = "sample-guid-id",
                Title = "Updated Report",
                Content = "Updated report content",
                UpdatedDate = "2024-12-01"
            };

            var content = TestHelper.GetJsonContent(updateReportCommand);

            // Act
            var response = await HttpClient.PutAsync(requestUri, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Delete_Should_Return_OK()
        {
            const string requestUri = "api/reports";

            var deleteReportCommand = new
            {
                Id = "sample-guid-id"
            };

            var content = TestHelper.GetJsonContent(deleteReportCommand);

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
