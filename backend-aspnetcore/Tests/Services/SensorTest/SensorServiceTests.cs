// using BackendAspNetCore.Dtos.Response;
// using BackendAspNetCore.Dtos.Sensor;
// using BackendAspNetCore.Models;
// using BackendAspNetCore.Repositories.SensorRepo;
// using BackendAspNetCore.RequestBody.Sensor;
// using BackendAspNetCore.Services.SensorServices;
// using FluentAssertions;
// using Moq;
// using Xunit;

// namespace BackendAspNetCore.Tests.Services.Sensor;

// public class SensorServiceTests
// {
//     private readonly Mock<ISensorRepository> _repoMock = new();
//     private readonly SensorService _sut;

//     public SensorServiceTests()
//     {
//         _sut = new SensorService(_repoMock.Object);
//     }

//     [Fact]
//     public async Task GetAllSensorAsync_WhenDataExists_ReturnsSuccessWithDtoList()
//     {
//         // Arrange
//         var sensors = new List<Sensor>
//         {
//             new() { MacAddress = "A1", Temperature = "20", Humidity = "50", LastUpdatedUtc = DateTimeOffset.UtcNow },
//             new() { MacAddress = "B2", Temperature = "21", Humidity = "60", LastUpdatedUtc = DateTimeOffset.UtcNow }
//         };

//         _repoMock.Setup(r => r.GetAllSensorAsync()).ReturnsAsync(sensors);
//         var request = new GetAllSensorRequestBody { Limit = 10 };

//         // Act
//         var response = await _sut.GetAllSensorAsync(request);

//         // Assert
//         response.Should().BeOfType<ApiResponse<List<SensorDto>>>();
//         response.IsSuccess.Should().BeTrue();
//         response.StatusCode.Should().Be(200);
//         response.Message.Should().Be("Sensor list has been retrieved");

//         var typedData = response.As<ApiResponse<List<SensorDto>>>().Data;
//         typedData.Should().NotBeNull().And.HaveCount(2);

//         typedData![0].MacAddress.Should().Be("A1");

//         _repoMock.Verify(r => r.GetAllSensorAsync(), Times.Once);
//     }

//     [Fact]
//     public async Task GetAllSensorAsync_WhenNoData_ReturnsFailResponse()
//     {
//         // Arrange
//         _repoMock.Setup(r => r.GetAllSensorAsync()).ReturnsAsync(new List<Sensor>());
//         var request = new GetAllSensorRequestBody { Limit = 10 };

//         // Act
//         var response = await _sut.GetAllSensorAsync(request);

//         // Assert
//         response.IsSuccess.Should().BeFalse();
//         response.StatusCode.Should().Be(400);
//         response.Message.Should().Be("No sensor is found");

//         _repoMock.Verify(r => r.GetAllSensorAsync(), Times.Once);
//     }

//     [Fact]
//     public async Task GetAllSensorAsync_WhenRepositoryThrows_ShouldBubbleException()
//     {
//         // Arrange
//         _repoMock
//             .Setup(r => r.GetAllSensorAsync())
//             .ThrowsAsync(new InvalidOperationException("DB failed"));

//         var request = new GetAllSensorRequestBody { Limit = 10 };

//         // Act
//         Func<Task> act = async () => await _sut.GetAllSensorAsync(request);

//         // Assert
//         await act.Should().ThrowAsync<InvalidOperationException>()
//             .WithMessage("DB failed");

//         _repoMock.Verify(r => r.GetAllSensorAsync(), Times.Once);
//     }
// }