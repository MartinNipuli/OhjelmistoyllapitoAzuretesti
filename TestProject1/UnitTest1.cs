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
    private StudentContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StudentContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique name per test
            .Options;
        return new StudentContext(options);
    }
    
    [Fact]
    public async Task GetStudents_ReturnsEmptyList_WhenNoStudents()
    {
        
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new StudentsController(context);

        // Act
        var result = await controller.GetStudents();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
        var students = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);
        Assert.Empty(students);
    }

    [Fact]
    public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
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
        var context = GetInMemoryDbContext();
        var controller = new StudentsController(context);
        var newStudent = new Student { FirstName = "John", LastName = "Doe", Age = 22 };

        // Act
        var result = await controller.PostStudent(newStudent);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var student = Assert.IsType<Student>(createdResult.Value);
        Assert.Equal("John", student.FirstName);
        Assert.Equal(22, student.Age);

        var studentsInDb = await context.Students.ToListAsync();
        Assert.Single(studentsInDb);
    }

    [Fact]
    public async Task PutStudent_UpdatesExistingStudent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new StudentsController(context);
        var student = new Student { FirstName = "John", LastName = "Doe", Age = 22 };
        context.Students.Add(student);
        await context.SaveChangesAsync();

        var studentId = student.Id;
        var existingStudent = await context.Students.FindAsync(studentId);
        existingStudent.FirstName = "Updated Name";
        existingStudent.LastName = "Updated Surname";
        existingStudent.Age = 25;


        // Act
        var result = await controller.PutStudent(studentId, existingStudent);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var updatedStudentFromDb = await context.Students.FindAsync(studentId);
        Assert.Equal("Updated Name", updatedStudentFromDb.FirstName);
        Assert.Equal(25, updatedStudentFromDb.Age);
    }

    [Fact]
    public async Task DeleteStudent_RemovesStudentFromDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new StudentsController(context);
        var student = new Student { FirstName = "John", LastName = "Doe", Age = 22 };
        context.Students.Add(student);
        await context.SaveChangesAsync();

        var studentId = student.Id;

        // Act
        var result = await controller.DeleteStudent(studentId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedStudent = await context.Students.FindAsync(studentId);
        Assert.Null(deletedStudent);
    }
}
