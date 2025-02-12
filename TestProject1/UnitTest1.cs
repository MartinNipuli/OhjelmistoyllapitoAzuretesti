using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Controllers;
using StudentEfCoreDemo.Data;
using StudentEfCoreDemo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class StudentsControllerTests
{
    private StudentContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<StudentContext>()
            .UseInMemoryDatabase(databaseName: "StudentTestDb")
            .Options;
        return new StudentContext(options);
    }

    [Fact]
    public async Task GetStudents_ReturnsEmptyList_WhenNoStudents()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new StudentsController(context);

        // Act
        var result = await controller.GetStudents();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
        var students = Assert.IsType<List<Student>>(actionResult.Value);
        Assert.Empty(students);
    }

    [Fact]
    public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new StudentsController(context);

        // Act
        var result = await controller.GetStudent(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostStudent_AddsStudentToDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new StudentsController(context);
        var newStudent = new Student { FirstName = "John Doe", Age = 22 };

        // Act
        var result = await controller.PostStudent(newStudent);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var student = Assert.IsType<Student>(createdResult.Value);
        Assert.Equal("John Doe", student.FirstName);
        Assert.Equal(22, student.Age);

        var studentsInDb = await context.Students.ToListAsync();
        Assert.Single(studentsInDb);
    }

    [Fact]
    public async Task PutStudent_UpdatesExistingStudent()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new StudentsController(context);
        var student = new Student { FirstName = "John Doe", Age = 22 };
        context.Students.Add(student);
        await context.SaveChangesAsync();

        student.FirstName = "Updated Name";
        student.Age = 25;

        // Act
        var result = await controller.PutStudent(student.Id, student);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var updatedStudent = await context.Students.FindAsync(1);
        Assert.Equal("Updated Name", updatedStudent.FirstName);
        Assert.Equal(25, updatedStudent.Age);
    }

    [Fact]
    public async Task DeleteStudent_RemovesStudentFromDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new StudentsController(context);
        var student = new Student { FirstName = "John Doe", Age = 22 };
        context.Students.Add(student);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.DeleteStudent(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedStudent = await context.Students.FindAsync(student.Id);
        Assert.Null(deletedStudent);
    }
}
