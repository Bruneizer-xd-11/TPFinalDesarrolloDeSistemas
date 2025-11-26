using System;
using Xunit;
using Persistencia; // <- este es el namespace de tu DaoDappers
using Models;       // <- si vas a usar tus clases Tarea, Usuario, etc.

public class UnitTest1
{
    [Fact]
    public void Test_ConexionDapper()
    {
        string connectionString = "TU_CONNECTION_STRING";
        var dao = new DaoDappers(connectionString);
        Assert.NotNull(dao);
    }
}

