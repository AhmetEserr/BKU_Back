using BKU.Data;
using BKU.Models;
using BKU.Repository;
using BKU.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BKU.Tests;

public class RepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task QuestionRepository_AddAndGetById()
    {
        using var context = CreateContext();
        IQuestionRepository repo = new QuestionRepository(context);
        var question = new Question { Text = "Sample", Year = 2020 };

        await repo.AddAsync(question);
        var fetched = await repo.GetByIdAsync(question.Id);

        Assert.NotNull(fetched);
        Assert.Equal("Sample", fetched!.Text);
    }

    [Fact]
    public async Task QuestionRepository_GetByYear()
    {
        using var context = CreateContext();
        IQuestionRepository repo = new QuestionRepository(context);
        var q2019 = new Question { Text = "Old", Year = 2019 };
        var q2020 = new Question { Text = "New", Year = 2020 };

        context.Questions.AddRange(q2019, q2020);
        await context.SaveChangesAsync();

        var list = await repo.GetByYearAsync(2020);

        Assert.Single(list);
        Assert.Equal("New", list[0].Text);
    }

    [Fact]
    public async Task AnswerRepository_AddAndGetById()
    {
        using var context = CreateContext();
        // Setup a question first because answer requires foreign key
        var q = new Question { Text = "Q" };
        context.Questions.Add(q);
        await context.SaveChangesAsync();

        IAnswerRepository repo = new AnswerRepository(context);
        var answer = new Answer { Text = "A", IsCorrect = true, QuestionId = q.Id };

        await repo.AddAsync(answer);
        var fetched = await repo.GetByIdAsync(answer.Id);

        Assert.NotNull(fetched);
        Assert.Equal("A", fetched!.Text);
        Assert.Equal(q.Id, fetched.QuestionId);
    }

    [Fact]
    public async Task KullanicilarRepository_AddAndGetById()
    {
        using var context = CreateContext();
        IKullanicilarRepository repo = new KullanicilarRepository(context);
        var user = new Kullanicilar { Username = "user", Email = "e@mail" };

        await repo.AddAsync(user);
        var fetched = await repo.GetByIdAsync(user.Id);

        Assert.NotNull(fetched);
        Assert.Equal("user", fetched!.Username);
    }
}
