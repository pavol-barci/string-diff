using FluentAssertions;
using StringDiff.Domain.Exceptions;
using StringDiff.Domain.Models;
using StringDiff.Domain.Repositories;
using StringDiff.Infrastructure.Repositories.InMemory;
using Xunit;

namespace StringDiff.Tests.InfrastructureTests;

public class DiffRepositoryTests
{
    private readonly IDiffRepository _diffRepository = new DiffInMemoryRepository();

    [Fact]
    public async Task GetById_ModelNotFound_ReturnsNull()
    {
        var model = await _diffRepository.GetById(Guid.NewGuid());

        model.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ModelFound_ReturnsModel()
    {
        var id = Guid.NewGuid();
        var diffModel = new DiffModel{Id = id, Left = "string"};
        await _diffRepository.Create(diffModel);
        
        var model = await _diffRepository.GetById(id);

        model.Should().NotBeNull();
        model.Id.Should().Be(diffModel.Id);
        model.Left.Should().Be(diffModel.Left);
        model.Right.Should().Be(diffModel.Right);
        model.DiffResult.Should().Be(diffModel.DiffResult);
    }

    [Fact]
    public async Task Update_ModelNotFound_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _diffRepository.Update(new DiffModel()));
    }

    [Fact]
    public async Task Update_ModelFound_UpdateModelAndReturnUpdated()
    {
        var id = Guid.NewGuid();
        var diffModel = new DiffModel{Id = id, Left = "string"};
        await _diffRepository.Create(diffModel);
        
        var modelToUpdate = new DiffModel
        {
            Id = id,
            Left = "new_string",
            Right = "string"
        };
        
        var updatedModel = await _diffRepository.Update(modelToUpdate);

        updatedModel.Should().NotBeNull();
        updatedModel.Id.Should().Be(modelToUpdate.Id);
        updatedModel.Left.Should().Be(modelToUpdate.Left);
        updatedModel.Right.Should().Be(modelToUpdate.Right);
        updatedModel.DiffResult.Should().Be(modelToUpdate.DiffResult);
    }

    [Fact]
    public async Task Create_ModelAlreadyExists_ThrowsConflictException()
    {
        var model = new DiffModel
        {
            Id = Guid.NewGuid()
        };

        await _diffRepository.Create(model);
        
        await Assert.ThrowsAsync<ConflictException>(() => _diffRepository.Create(model));
    }

    [Fact]
    public async Task Create_ModelNotExists_CreateAndReturnCreated()
    {
        var diffModel = new DiffModel{Id = Guid.NewGuid(), Left = "string"};
        
        var createdModel = await _diffRepository.Create(diffModel);

        createdModel.Should().NotBeNull();
        createdModel.Id.Should().Be(diffModel.Id);
        createdModel.Left.Should().Be(diffModel.Left);
        createdModel.Right.Should().Be(diffModel.Right);
        createdModel.DiffResult.Should().Be(diffModel.DiffResult);
    }
}