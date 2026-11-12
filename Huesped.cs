using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Hotel
{
    // Huesped hereda de Persona e implementa la interfaz IFacturable
    public class Huesped : Persona, IFacturable
    {
        private const decimal IVA = 0.19m; // 19 % de IVA

        private int numeroAcompanantes;
        private int noches;

        // Lista privada: nadie de afuera puede modificarla directamente
        private readonly List<Habitacion> habitaciones = new List<Habitacion>();

        // Llamamos al constructor de la clase padre con : base(...)
        public Huesped(string nombre, string documento, int numeroAcompanantes, int noches)
            : base(nombre, documento)
        {
            if (numeroAcompanantes < 0)
                throw new ArgumentException("El número de acompañantes no puede ser negativo.");

            this.numeroAcompanantes = numeroAcompanantes;
            Noches = noches; // usamos la propiedad para que se aplique la validación
        }

        public int NumeroAcompanantes
        {
            get { return numeroAcompanantes; }
        }

        // El huésped más sus acompañantes
        public int TotalPersonas
        {
            get { return 1 + numeroAcompanantes; }
        }

        // Propiedad con validación en el set: mínimo 1 noche
        public int Noches
        {
            get { return noches; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("El número de noches no puede ser menor que 1.");
                noches = value;
            }
        }

        // Se devuelve una IReadOnlyList para que no puedan modificar la lista original
        public IReadOnlyList<Habitacion> Habitaciones
        {
            get { return habitaciones.AsReadOnly(); }
        }

        // internal: solo se puede usar dentro del proyecto, o sea, desde el Recepcionista
        internal void AgregarHabitacion(Habitacion habitacion)
        {
            habitaciones.Add(habitacion);
        }

        // Polimorfismo: implementación propia del método abstracto de Persona
        public override string IdentificacionIndividuo()
        {
            return $"Huésped {Nombre}, documento {Documento}, viaja con {TotalPersonas} persona(s)";
        }

        // IFacturable: suma de precios por noche * noches + 19 % de IVA
        public decimal TotalAPagar()
        {
            decimal sumaPrecios = 0;
            foreach (Habitacion h in habitaciones)
            {
                sumaPrecios += h.PrecioPorNoche;
            }

            decimal subtotal = sumaPrecios * noches;
            return subtotal + (subtotal * IVA);
        }

        public string Factura()
        {
            decimal sumaPrecios = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- FACTURA de {Nombre} (Doc: {Documento}) ---");

            foreach (Habitacion h in habitaciones)
            {
                sb.AppendLine($"  Habitación {h.Numero} ({h.Tipo}): ${h.PrecioPorNoche.ToString("N0", CultureInfo.InvariantCulture)} por noche");
                sumaPrecios += h.PrecioPorNoche;
            }

            decimal subtotal = sumaPrecios * noches;
            decimal valorIva = subtotal * IVA;

            sb.AppendLine($"  Noches: {noches}");
            sb.AppendLine($"  Subtotal: ${subtotal.ToString("N0", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"  IVA (19%): ${valorIva.ToString("N0", CultureInfo.InvariantCulture)}");
            sb.Append($"  TOTAL A PAGAR: ${TotalAPagar().ToString("N0", CultureInfo.InvariantCulture)}");
            return sb.ToString();
        }
    }
}
