using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StringDiff.Application.Abstraction.Services;
using StringDiff.Application.Services;
using StringDiff.Contracts;
using StringDiff.Domain.Exceptions;
using StringDiff.Domain.Models;
using StringDiff.Domain.Repositories;
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
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<NotFoundException>(() => _diffService.GetDiff(id));
    }
    
    [Fact]
    public async Task GetDiff_DiffNotFinished_ReturnsNotFinished()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync(() => new DiffModel());
        var id = Guid.NewGuid();

        var result = await _diffService.GetDiff(id);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.NotFinished);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public async Task GetDiff_DiffFinished_ReturnsResult()
    {
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync(() => new DiffModel
        {
            DiffResult = new DiffResult(DiffResultType.Equals)
        });
        var id = Guid.NewGuid();

        var result = await _diffService.GetDiff(id);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.Equal);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public async Task UpsertLeft_ModelUpdated_ReturnsFalse()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new DiffModel
        {
            Id = id
        });
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertLeft(id, request);

        result.Should().BeFalse();
        _diffRepositoryMock.Verify(o => o.Create(It.IsAny<DiffModel>()), Times.Never);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Left == input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertLeft_ModelCreated_ReturnsTrue()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertLeft(id, request);

        result.Should().BeTrue();
        _diffRepositoryMock.Verify(o => o.Create(It.Is<DiffModel>(model => model.Id == id && model.Left == input)));
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Left == input))); 
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertLeft_ModelFullyFilled_CalculatesDiffAndReturnsTrue()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new DiffModel
        {
            Id = id,
            Right = input
        });
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertLeft(id, request);

        result.Should().BeFalse();
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.Is<DiffModel>(model => model.Id == id && model.Left == input && model.Right == "string")), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Left == input)), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Left == input)), Times.Once);
    }
    
    [Fact]
    public async Task UpsertRight_ModelUpdated_ReturnsFalse()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new DiffModel
        {
            Id = id
        });
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertRight(id, request);

        result.Should().BeFalse();
        _diffRepositoryMock.Verify(o => o.Create(It.IsAny<DiffModel>()), Times.Never);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Right == input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertRight_ModelCreated_ReturnsTrue()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertRight(id, request);

        result.Should().BeTrue();
        _diffRepositoryMock.Verify(o => o.Create(It.Is<DiffModel>(model => model.Id == id && model.Right == input)));
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Right == input)));
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.IsAny<DiffModel>()), Times.Never);
    }
    
    [Fact]
    public async Task UpsertRight_ModelFullyFilled_CalculatesDiffAndReturnsTrue()
    {
        var input = "string";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        _diffRepositoryMock.Setup(o => o.GetById(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new DiffModel
        {
            Id = id,
            Left = input
        });
        var request = new DiffRequest
        {
            Input = encoded
        };
        var id = Guid.NewGuid();

        var result = await _diffService.UpsertRight(id, request);

        result.Should().BeFalse();
        _diffCalculatorMock.Verify(o => o.CalculateDiff(It.Is<DiffModel>(model => model.Id == id && model.Right == input && model.Left == "string")), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Right == input)), Times.Once);
        _diffRepositoryMock.Verify(o => o.Update(It.Is<DiffModel>(model => model.Id == id && model.Right == input)), Times.Once);
    }
}