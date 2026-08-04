using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace Dsw2026Tpi.Tests;
public class SpecialityServiceTests
{
    private readonly IPersistence _mockPersistence;
    private readonly SpecialityService _service;

    public SpecialityServiceTests()
    {
        _mockPersistence = Substitute.For<IPersistence>();
        _service = new SpecialityService(_mockPersistence);
    }
    //1era prueba
    [Fact]
    public async Task Agregar_CuandoNombreEsValidoYNoExiste_EntoncesDevuelveEspecialidadCreada()
    {
        //Arrange
        var request = new SpecialityModel.Request("Cardiologia", "Enfermedades del corazon");
        _mockPersistence
             .First(Arg.Any<Expression<Func<Speciality, bool>>>(), Arg.Any<string[]>())
             .Returns((Speciality?)null);

        //Act
        var result = await _service.Add(request);
        //Assert
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.NotEqual(Guid.Empty, result.Id);

        await _mockPersistence.Received(1).Add(Arg.Any<Speciality>());
    }
    //2da prueba
    [Fact]
    public async Task Agregar_CuandoNombreEsInvalido_EntoncesLanzaValitionException()
    {
        //Arrange
        var request = new SpecialityModel.Request("AB", "Descripcion valida de prueba");
        //Act y Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.Add(request));
        await _mockPersistence.DidNotReceive().Add(Arg.Any<Speciality>());
    }
    //3ra prueba
    [Fact]
    public async Task Agregar_CuandoNombreYaExiste_EntoncesLanzaConflictoException()
    {
        //Arrange
        var request = new SpecialityModel.Request("Pediatria", "Atencion de niños");
        var existente = new Speciality("Pediatria", "Ya registrada previamente");
        _mockPersistence 
            .First(Arg.Any<Expression<Func<Speciality, bool>>>(), Arg.Any<string[]>())
            .Returns(existente);

        //Act y Assert
        await Assert.ThrowsAsync<ConflictException>(() => _service.Add(request));
        await _mockPersistence.DidNotReceive().Add(Arg.Any<Speciality>());
    }
    //4ta prueba 
    [Fact]
    public async Task Delete_CuandoIdNoExiste_EntoncesLanzaEntityNotFoundException()
    { 
        //Arrange
        var id = Guid.NewGuid();    
        _mockPersistence
            .GetById<Speciality>(id, Arg.Any<string[]>())
            .Returns((Speciality?)null);
        //Act y Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.Delete(id));
        await _mockPersistence.DidNotReceive().Update(Arg.Any<Speciality>());
    }
}

