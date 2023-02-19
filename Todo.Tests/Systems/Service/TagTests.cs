using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Todo.Database;
using Todo.Database.Entity;
using Todo.Service.Models;
using Todo.Service.Services;
using Todo.Tests.Mock;

namespace Todo.Tests;

public class TagTests :IDisposable
{
    protected readonly DatabaseContext _dbContext;

    public TagTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DatabaseContext(options);

        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task Get_OnSuccess_ReturnsTag()
    {
        // Arrange
        var sut = new Tags(_dbContext);
        
        // Act

        var result = await sut.AddTag(new AddTagModel() { Tag = "This is test tag" });

        // Assert
        result.Should().BeOfType<AddTagResult>();
        var tagResult = (AddTagResult)result;
        tagResult.Tag.Should().BeOfType<TagEntity>();
    }

    [Fact]
    public async Task GetTagById_OnSuccess_ReturnsTag()
    {
        // Assert
        _dbContext.Tags.AddRange(TagMockData.Tags());
        await _dbContext.SaveChangesAsync();

        var sut = new Tags(_dbContext);
        
        // Act
        var result = await sut.GetTagById(1);

        // Arrange
        result.Should().BeOfType<GetTagByIdResult>();
        var tagResult = (GetTagByIdResult)result;
        tagResult.Tag.Should().BeOfType<TagEntity>();
        
    }

    [Fact]
    public async Task GetTagByName_OnSuccess_ReturnsTag()
    {
        // Assert
        _dbContext.Tags.AddRange(TagMockData.Tags());
        await _dbContext.SaveChangesAsync();

        var sut = new Tags(_dbContext);
        
        // Act
        var result = await sut.GetTagByName("this is test tag");

        // Arrange
        result.Should().BeOfType<GetTagByNameResult>();
        var tagResult = (GetTagByNameResult)result;
        tagResult.Tag.Should().BeOfType<TagEntity>();

    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}