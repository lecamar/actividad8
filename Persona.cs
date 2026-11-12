using System;

namespace Hotel
{
    // Clase abstracta: no se pueden crear objetos Persona directamente,
    // solo sirve como base para Recepcionista y Huesped.
    public abstract class Persona
    {
        // Campos protected: las clases hijas pueden usarlos, pero nadie más desde afuera.
        protected string nombre;
        protected string documento;

        // Constructor protegido: lo llaman los hijos con : base(...)
        protected Persona(string nombre, string documento)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El documento no puede estar vacío.");

            this.nombre = nombre;
            this.documento = documento;
        }

        // Propiedades de solo lectura (solo tienen get, no tienen set)
        public string Nombre
        {
            get { return nombre; }
        }

        public string Documento
        {
            get { return documento; }
        }

        // Método abstracto: cada clase hija lo implementa a su manera (polimorfismo)
        public abstract string IdentificacionIndividuo();

        // Sobrescribimos ToString() para mostrar la persona de forma legible
        public override string ToString()
        {
            return $"{nombre} (Documento: {documento})";
        }
    }
}
