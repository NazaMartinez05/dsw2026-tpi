using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Patient : EntityBase
    {
        public string Dni { get; init; }
        public string Email { get; init; }
        public string? FullName { get; set; }
        // Id del ApplicationUser (Identity), es el creado en el primer login (RN06)
        public string? UserId { get; set; }

        #pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
        private Patient() { }
        #pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
        public Patient(string dni, string email, string? fullName = null, string? userId = null, Guid? id = null) : base(id)
        {
            Dni = dni;
            Email = email;
            FullName = fullName;
            UserId = userId;
        }
    }
}
