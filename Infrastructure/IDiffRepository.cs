using StringDiff.Domain;
using StringDiff.Infrastructure.Exceptions;

namespace StringDiff.Infrastructure;

/// <summary>
/// Repository with persistance operations on <see cref="DiffModel"/>
/// </summary>
public interface IDiffRepository
{
    /// <summary>
    /// Retrieve <see cref="DiffModel"/> by Id 
    /// </summary>
    /// <param name="id">Id of the model</param>
    /// <returns>
    ///     null => when model with specified Id does not exist
    ///     <see cref="DiffModel"/> => when model successfully loaded
    /// </returns>
    Task<DiffModel?> GetById(int id);
    
    /// <summary>
    /// Update specified <see cref="DiffModel"/> 
    /// </summary>
    /// <param name="model"><see cref="DiffModel"/> to be updated</param>
    /// <returns>Updated <see cref="DiffModel"/></returns>
    /// <exception cref="NotFoundException">Thrown when the model with id does not exist.</exception>
    Task<DiffModel> Update(DiffModel model);
    
    /// <summary>
    /// Create specified <see cref="DiffModel"/> 
    /// </summary>
    /// <param name="model"><see cref="DiffModel"/> to be created</param>
    /// <returns>Created <see cref="DiffModel"/></returns>
    /// <exception cref="ConflictException">Thrown when the model with specified id already exists.</exception>
    Task<DiffModel> Create(DiffModel model);
}
