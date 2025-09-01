using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StringDiff.Application;
using StringDiff.Application.Models;
using StringDiff.Application.Services;
using StringDiff.Domain;
using StringDiff.Infrastructure;
using StringDiff.Infrastructure.Exceptions;
using StringDiff.Models;
using Xunit;

namespace StringDiff.Tests.ApplicationTests;

public class DiffServiceTests
{
    private readonly Mock<IDiffRepository> _diffRepositoryMock;
    private readonly Mock<IDiffCalculator> _diffCalculatorMock;
    private readonly IDiffService _diffService;
    
    public DiffServiceTests()
    {
        var logger = new Mock<ILogger<DiffService>>();
        _diffRepositoryMock = new Mock<IDiffRepository>();
        _diffCalculatorMock = new Mock<IDiffCalculator>();

        _diffService = new DiffService(_diffRepositoryMock.Object, _diffCalculatorMock.Object, logger.Object);
    }
    
    [Fact]
    public async Task GetDiff_ModelNotFound_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _diffService.GetDiff(1));
    }
    
    [Fact]
    public async Task GetDiff_DiffNotFinished_ReturnsNotFinished()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync(() => new DiffModel());

        var result = await _diffService.GetDiff(1);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.NotFinished);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public async Task GetDiff_DiffFinished_ReturnsResult()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync(() => new DiffModel
        {
            DiffResult = new DiffResult(DiffResultType.Equals)
        });

        var result = await _diffService.GetDiff(1);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.Equal);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public async Task UpsertLeft_ModelUpdated_ReturnsFalse()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync((int id) => new DiffModel
        {
            Id = id
        });
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertLeft(1, request);

        result.Should().BeFalse();
        _diffRepositoryMock.Verify(o => o.Create(It.IsAny<DiffModel>()), Times.Never);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertLeft_ModelCreated_ReturnsTrue()
    {
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertLeft(1, request);

        result.Should().BeTrue();
        _diffRepositoryMock.Verify(o => o.Create(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input)));
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input))); 
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertLeft_ModelFullyFilled_CalculatesDiffAndReturnsTrue()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync((int id) => new DiffModel
        {
            Id = id,
            Right = "string"
        });
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertLeft(1, request);

        result.Should().BeFalse();
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input && model.Right == "string")), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input)), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Left == request.Input)), Times.Once);
    }
    
    [Fact]
    public async Task UpsertRight_ModelUpdated_ReturnsFalse()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync((int id) => new DiffModel
        {
            Id = id
        });
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertRight(1, request);

        result.Should().BeFalse();
        _diffRepositoryMock.Verify(o => o.Create(It.IsAny<DiffModel>()), Times.Never);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertRight_ModelCreated_ReturnsTrue()
    {
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertRight(1, request);

        result.Should().BeTrue();
        _diffRepositoryMock.Verify(o => o.Create(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input)));
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertRight_ModelFullyFilled_CalculatesDiffAndReturnsTrue()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<int>())).ReturnsAsync((int id) => new DiffModel
        {
            Id = id,
            Left = "string"
        });
        var request = new DiffRequest
        {
            Input = "string"
        };

        var result = await _diffService.UpsertRight(1, request);

        result.Should().BeFalse();
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input && model.Left == "string")), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input)), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == 1 && model.Right == request.Input)), Times.Once);
    }
}