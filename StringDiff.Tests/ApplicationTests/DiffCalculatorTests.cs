using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StringDiff.Application;
using StringDiff.Application.Services;
using StringDiff.Domain;
using Xunit;

namespace StringDiff.Tests.ApplicationTests;

public class DiffCalculatorTests
{
    private readonly IDiffCalculator _diffCalculator;
    
    public DiffCalculatorTests()
    {
        var logger = new Mock<ILogger<DiffCalculator>>();
        
        _diffCalculator = new DiffCalculator(logger.Object);
    }
    
    [Theory]
    [InlineData(null, null)]
    [InlineData("test", null)]
    [InlineData(null, "test")]
    public async Task CalculateDiff_NotCompleteModel_ReturnsNull(string? left, string? right)
    {
        var model = new DiffModel(left, right);

        var result = await _diffCalculator.CalculateDiff(model);

        result.Should().BeNull();
    }
    
    [Fact]
    public async Task CalculateDiff_NotSameLength_ReturnsNotEquals()
    {
        var model = new DiffModel("abc", "abdefg");

        var result = await _diffCalculator.CalculateDiff(model);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultType.DifferentSizes);
        result.Length.Should().Be(0);
        result.Offset.Should().Be(0);
    }
    
    [Fact]
    public async Task CalculateDiff_SameStrings_ReturnsEquals()
    {
        var model = new DiffModel("abc", "abc");

        var result = await _diffCalculator.CalculateDiff(model);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultType.Equals);
        result.Length.Should().Be(0);
        result.Offset.Should().Be(0);
    }
    
    [Fact]
    public async Task CalculateDiff_DifferentStrings_ReturnsDiffWithOffset()
    {
        var model = new DiffModel("abc123", "abc456");

        var result = await _diffCalculator.CalculateDiff(model);

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultType.NotEquals);
        result.Length.Should().Be(3);
        result.Offset.Should().Be(3);
    }
}