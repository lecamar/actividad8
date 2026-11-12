using System;
using System.Collections.Generic;

namespace Hotel
{
    // Recepcionista hereda de Persona
    public class Recepcionista : Persona
    {
        private readonly string turno;
        private readonly string codigoEmpleado;

        // Lista privada con los huéspedes que ESTE recepcionista ha registrado
        private readonly List<Huesped> huespedesRegistrados = new List<Huesped>();

        // Llamamos al constructor de Persona con : base(...)
        public Recepcionista(string nombre, string documento, string turno, string codigoEmpleado)
            : base(nombre, documento)
        {
            if (string.IsNullOrWhiteSpace(turno))
                throw new ArgumentException("El turno no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(codigoEmpleado))
                throw new ArgumentException("El código de empleado no puede estar vacío.");

            this.turno = turno;
            this.codigoEmpleado = codigoEmpleado;
        }

        public string Turno { get { return turno; } }
        public string CodigoEmpleado { get { return codigoEmpleado; } }

        // Se devuelve una IReadOnlyList (solo lectura) para proteger la lista interna
        public IReadOnlyList<Huesped> HuespedesRegistrados
        {
            get { return huespedesRegistrados.AsReadOnly(); }
        }

        // Polimorfismo: implementación propia del método abstracto de Persona
        public override string IdentificacionIndividuo()
        {
            return $"Recepcionista {Nombre}, código {codigoEmpleado}, turno de {turno}";
        }

        // Registra un huésped. Devuelve true si se pudo y false si lo rechazó.
        public bool RegistrarHuesped(Huesped huesped)
        {
            // Regla 1: se rechaza null
            if (huesped == null)
            {
                Console.WriteLine($"  [RECHAZADO] {Nombre}: no se puede registrar un huésped nulo (null).");
                return false;
            }

            // Regla 2: se rechazan duplicados (mismo documento)
            foreach (Huesped registrado in huespedesRegistrados)
            {
                if (registrado.Documento == huesped.Documento)
                {
                    Console.WriteLine($"  [RECHAZADO] {Nombre}: el huésped {huesped.Nombre} ya estaba registrado (documento {huesped.Documento}).");
                    return false;
                }
            }

            huespedesRegistrados.Add(huesped);
            Console.WriteLine($"  [OK] {Nombre} registró al huésped {huesped.Nombre}.");
            return true;
        }

        // Asigna una habitación a un huésped. Devuelve true si se pudo y false si lo rechazó.
        public bool AsignarHabitacion(Huesped huesped, Habitacion habitacion)
        {
            if (huesped == null || habitacion == null)
            {
                Console.WriteLine($"  [RECHAZADO] {Nombre}: el huésped y la habitación no pueden ser null.");
                return false;
            }

            // Regla 3: solo se asigna a huéspedes que este mismo recepcionista registró
            if (!huespedesRegistrados.Contains(huesped))
            {
                Console.WriteLine($"  [RECHAZADO] {Nombre}: {huesped.Nombre} no fue registrado por este recepcionista.");
                return false;
            }

            // Regla 4: la habitación no puede estar ocupada
            if (habitacion.Ocupada)
            {
                Console.WriteLine($"  [RECHAZADO] La habitación {habitacion.Numero} ya está ocupada, no se puede asignar a {huesped.Nombre}.");
                return false;
            }

            // Regla 5: la capacidad debe alcanzar para el huésped y sus acompañantes
            if (habitacion.Capacidad < huesped.TotalPersonas)
            {
                Console.WriteLine($"  [RECHAZADO] La habitación {habitacion.Numero} tiene capacidad para {habitacion.Capacidad} " +
                                  $"y {huesped.Nombre} viaja con {huesped.TotalPersonas} persona(s).");
                return false;
            }

            // Si pasó todas las validaciones, se hace la asignación
            habitacion.Ocupar();
            huesped.AgregarHabitacion(habitacion);
            Console.WriteLine($"  [OK] {Nombre} asignó la habitación {habitacion.Numero} a {huesped.Nombre}.");
            return true;
        }
    }
}
