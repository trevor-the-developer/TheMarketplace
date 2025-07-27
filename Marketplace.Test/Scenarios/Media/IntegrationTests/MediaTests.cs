using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Marketplace.Core.Models;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.IntegrationTests;

[Collection("scenarios")]
public class MediaTests : ScenarioContext, IAsyncLifetime
{
    private S3TestFixture? _s3Fixture;

    public MediaTests(WebAppFixture fixture) : base(fixture)
    {
    }

    public async Task InitializeAsync()
    {
        await DatabaseResetService.ResetDatabaseAsync();

        // Get dependencies from the Alba host's service provider
        var s3Configuration = Host.Services.GetRequiredService<IOptions<S3Configuration>>().Value;
        var logger = Host.Services.GetRequiredService<ILogger<S3TestFixture>>();

        _s3Fixture = new S3TestFixture(s3Configuration, logger);
        await _s3Fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (_s3Fixture != null)
        {
            await _s3Fixture.DisposeAsync();
        }
    }

    [Fact]
    public async Task CreateMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Title = "New Media",
                    Description = "A new media description.",
                    FilePath = "/path/to/file.mp4",
                    DirectoryPath = "/path/to/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Media", responseText);
    }

    [Fact]
    public async Task UpdateMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Title = "Media to Update",
                    Description = "A media that will be updated.",
                    FilePath = "/path/to/original.mp4",
                    DirectoryPath = "/path/to/original/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        using var jsonDoc = JsonDocument.Parse(createResponseText);
        var mediaId = jsonDoc.RootElement.TryGetProperty("id", out var idElement)
            ? idElement.GetInt32().ToString()
            : "1";

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new
                {
                    Id = int.Parse(mediaId),
                    Title = "Updated Media",
                    Description = "An updated media description.",
                    FilePath = "/path/to/updated.mp4",
                    DirectoryPath = "/path/to/updated/",
                    MediaType = "Audio",
                    ProductDetailId = 1
                })
                .ToUrl($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Media", responseText);
    }

    [Fact]
    public async Task DeleteMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Title = "Media to Delete",
                    Description = "A media that will be deleted.",
                    FilePath = "/path/to/delete.mp4",
                    DirectoryPath = "/path/to/delete/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        using var jsonDoc = JsonDocument.Parse(createResponseText);
        var mediaId = jsonDoc.RootElement.TryGetProperty("id", out var idElement)
            ? idElement.GetInt32().ToString()
            : "2";

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }

    #region S3 Integration Tests

    [Fact]
    public async Task UploadMedia_WithValidFile_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const string testContent = "Test file content for upload testing";

        using var fileStream = S3TestFixture.CreateTestFileStream(testContent);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(fileStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "file", "test-upload.txt");
        multipartContent.Add(new StringContent("Uploaded Test File"), "title");
        multipartContent.Add(new StringContent("File uploaded via integration test"), "description");
        multipartContent.Add(new StringContent("Document"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Uploaded Test File", responseText);

        // Verify the response contains S3 path information
        var mediaResponse = JsonConvert.DeserializeObject<dynamic>(responseText);
        Assert.NotNull(mediaResponse?.media?.filePath);
        Assert.StartsWith("products/1/media/", mediaResponse?.media.filePath.ToString());
    }

    [Fact]
    public async Task UploadMedia_WithBinaryFile_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        using var binaryStream = S3TestFixture.CreateTestBinaryFileStream(2048);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(binaryStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("image/jpeg") }
        }, "file", "test-image.jpg");
        multipartContent.Add(new StringContent("Binary Test File"), "title");
        multipartContent.Add(new StringContent("Binary file for testing"), "description");
        multipartContent.Add(new StringContent("Image"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.Contains("Binary Test File", responseText);
    }

    [Fact]
    public async Task UploadMedia_WithoutFile_BadRequest()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        using var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("title", "File Without Upload"),
            new KeyValuePair<string, string>("description", "This should fail"),
            new KeyValuePair<string, string>("mediaType", "Document"),
            new KeyValuePair<string, string>("productDetailId", "1")
        });

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .FormData(formContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact]
    public async Task DownloadMedia_ExistingFile_ReturnsFileStream()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const string testContent = "Test file content for download testing";

        // First upload a file
        using var uploadStream = S3TestFixture.CreateTestFileStream(testContent);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(uploadStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "file", "download-test.txt");
        multipartContent.Add(new StringContent("Download Test File"), "title");
        multipartContent.Add(new StringContent("File for download testing"), "description");
        multipartContent.Add(new StringContent("Document"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var uploadResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var uploadResponseText = await uploadResponse.ReadAsTextAsync();
        var uploadMediaResponse = JsonConvert.DeserializeObject<dynamic>(uploadResponseText);
        var mediaId = uploadMediaResponse?.media?.id;

        Assert.NotNull(mediaId);

        // Now download the file
        var downloadResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{mediaId}/download");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var downloadedContent = await downloadResponse.ReadAsTextAsync();
        Assert.Equal(testContent, downloadedContent);
    }

    [Fact]
    public async Task DownloadMedia_NonExistentFile_NotFound()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const int nonExistentMediaId = 99999;

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{nonExistentMediaId}/download");
            _.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }

    [Fact]
    public async Task GetMediaUrl_ExistingFile_ReturnsValidUrl()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const string testContent = "Test file content for URL testing";

        // First upload a file
        using var uploadStream = S3TestFixture.CreateTestFileStream(testContent);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(uploadStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "file", "url-test.txt");
        multipartContent.Add(new StringContent("URL Test File"), "title");
        multipartContent.Add(new StringContent("File for URL testing"), "description");
        multipartContent.Add(new StringContent("Document"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var uploadResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var uploadResponseText = await uploadResponse.ReadAsTextAsync();
        var uploadMediaResponse = JsonConvert.DeserializeObject<dynamic>(uploadResponseText);
        var mediaId = uploadMediaResponse?.media?.id;

        Assert.NotNull(mediaId);

        // Now get presigned URL
        var urlResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{mediaId}/url");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var urlResponseText = await urlResponse.ReadAsTextAsync();
        var urlData = JsonConvert.DeserializeObject<dynamic>(urlResponseText);

        Assert.NotNull(urlData?.url);
        Assert.NotNull(urlData?.expiresIn);

        var presignedUrl = urlData?.url.ToString();
        Assert.StartsWith("http://localhost:3900", presignedUrl);
        Assert.Contains("the-marketplace", presignedUrl); // bucket name
    }

    [Fact]
    public async Task GetMediaUrl_WithCustomExpiration_ReturnsValidUrl()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const string testContent = "Test file content for custom expiration URL testing";

        // First upload a file
        using var uploadStream = S3TestFixture.CreateTestFileStream(testContent);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(uploadStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "file", "custom-expiry-test.txt");
        multipartContent.Add(new StringContent("Custom Expiry Test File"), "title");
        multipartContent.Add(new StringContent("File for custom expiration URL testing"), "description");
        multipartContent.Add(new StringContent("Document"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var uploadResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var uploadResponseText = await uploadResponse.ReadAsTextAsync();
        var uploadMediaResponse = JsonConvert.DeserializeObject<dynamic>(uploadResponseText);
        var mediaId = uploadMediaResponse?.media?.id;

        Assert.NotNull(mediaId);

        // Now get presigned URL with custom expiration (2 hours)
        var urlResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{mediaId}/url?expirationHours=2");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var urlResponseText = await urlResponse.ReadAsTextAsync();
        var urlData = JsonConvert.DeserializeObject<dynamic>(urlResponseText);

        Assert.NotNull(urlData?.url);
        Assert.NotNull(urlData?.expiresIn);

        // Verify expiration is approximately 2 hours (7200 seconds)
        var expiresIn = (double)urlData!.expiresIn;
        Assert.True(expiresIn is > 7100 and < 7300, $"Expected ~7200 seconds, got {expiresIn}");
    }

    [Fact]
    public async Task GetMediaUrl_NonExistentFile_NotFound()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const int nonExistentMediaId = 99999;

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{nonExistentMediaId}/url");
            _.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }

    [Fact]
    public async Task DeleteMedia_WithS3File_DeletesFileAndRecord()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        var testContent = "Test file content for deletion testing";

        // First upload a file
        using var uploadStream = S3TestFixture.CreateTestFileStream(testContent);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(uploadStream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
        }, "file", "delete-test.txt");
        multipartContent.Add(new StringContent("Delete Test File"), "title");
        multipartContent.Add(new StringContent("File for deletion testing"), "description");
        multipartContent.Add(new StringContent("Document"), "mediaType");
        multipartContent.Add(new StringContent("1"), "productDetailId");

        var uploadResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .MultipartFormData(multipartContent)
                .ToUrl("/api/media/upload");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var uploadResponseText = await uploadResponse.ReadAsTextAsync();
        var uploadMediaResponse = JsonConvert.DeserializeObject<dynamic>(uploadResponseText);
        var mediaId = uploadMediaResponse?.media?.id;
        var filePath = uploadMediaResponse?.media?.filePath?.ToString();

        Assert.NotNull(mediaId);
        Assert.NotNull(filePath);

        // Verify file exists in S3
        var fileExists = await _s3Fixture!.DoesS3ObjectExistAsync(_s3Fixture.TestS3Config.BucketName, filePath);
        Assert.True(fileExists, "File should exist in S3 before deletion");

        // Now delete the media
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Verify file is deleted from S3
        var fileExistsAfterDelete = await _s3Fixture.DoesS3ObjectExistAsync(_s3Fixture.TestS3Config.BucketName, filePath);
        Assert.False(fileExistsAfterDelete, "File should be deleted from S3");

        // Verify record is deleted from database
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }

    [Fact]
    public async Task GetMediaByProductDetail_WithUploadedFiles_ReturnsMediaList()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        const int productDetailId = 123;

        // Upload multiple files for the same product detail
        var files = new[]
        {
            ("file1.txt", "Content for file 1", "Document 1"),
            ("file2.txt", "Content for file 2", "Document 2")
        };

        foreach (var (fileName, content, title) in files)
        {
            using var uploadStream = S3TestFixture.CreateTestFileStream(content);

            using var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(uploadStream)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("text/plain") }
            }, "file", fileName);
            multipartContent.Add(new StringContent(title), "title");
            multipartContent.Add(new StringContent($"Test file: {fileName}"), "description");
            multipartContent.Add(new StringContent("Document"), "mediaType");
            multipartContent.Add(new StringContent(productDetailId.ToString()), "productDetailId");

            await Host.Scenario(_ =>
            {
                _.WithBearerToken(token);
                _.Post
                    .MultipartFormData(multipartContent)
                    .ToUrl("/api/media/upload");
                _.StatusCodeShouldBe(HttpStatusCode.Created);
            });
        }

        // Now get media by product detail ID
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Get.Url($"/api/media/product/{productDetailId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        var mediaResponse = JsonConvert.DeserializeObject<dynamic>(responseText);

        Assert.NotNull(mediaResponse?.mediaList);
        Assert.True(mediaResponse?.mediaList.Count >= 2, "Should return at least 2 media items");

        // Verify both files are in the response
        Assert.Contains("Document 1", responseText);
        Assert.Contains("Document 2", responseText);
    }

    #endregion
}