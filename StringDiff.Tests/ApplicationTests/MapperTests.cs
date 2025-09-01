using FluentAssertions;
using StringDiff.Application.Helpers;
using StringDiff.Application.Models;
using StringDiff.Domain;
using Xunit;

namespace StringDiff.Tests.ApplicationTests;

public class MapperTests
{
    [Fact]
    public void ToDiffResultResponse_NullResult_ReturnsNotFinished()
    {
        DiffResult? model = null;

        var result = model.ToDiffResultResponse();

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.NotFinished);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public void ToDiffResultResponse_EqualResult_ReturnsEquals()
    {
        var model = new DiffResult(DiffResultType.Equals);

        var result = model.ToDiffResultResponse();

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.Equal);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public void ToDiffResultResponse_DifferentSizeResult_ReturnsDifferentSize()
    {
        var model = new DiffResult(DiffResultType.DifferentSizes);

        var result = model.ToDiffResultResponse();

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.DifferentSize);
        result.Difference.Should().BeNull();
    }
    
    [Fact]
    public void ToDiffResultResponse_NotEqualsResult_ReturnsNotEqualsWithOffset()
    {
        var model = new DiffResult(DiffResultType.NotEquals, 10, 20);

        var result = model.ToDiffResultResponse();

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.NotEquals);
        result.Difference.Should().NotBeNull();
        result.Difference.Offset.Should().Be(model.Offset);
        result.Difference.Length.Should().Be(model.Length);
    }
    
    [Fact]
    public void ToDiffResultResponse_WrongEnum_ReturnsDifferentSize()
    {
        var model = new DiffResult((DiffResultType)10);

        var result = model.ToDiffResultResponse();

        result.Should().NotBeNull();
        result.Result.Should().Be(DiffResultResponseType.NotFinished);
        result.Difference.Should().BeNull();
    }
}