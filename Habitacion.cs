using System;
using System.Globalization;

namespace Hotel
{
    // Los tres tipos de habitación posibles
    public enum TipoHabitacion
    {
        Sencilla,
        Doble,
        Suite
    }

    // Habitacion NO hereda de Persona, pero sí implementa IFacturable
    public class Habitacion : IFacturable
    {
        private readonly int numero;
        private readonly TipoHabitacion tipo;
        private readonly int capacidad;
        private readonly decimal precioPorNoche;
        private bool ocupada;

        public Habitacion(int numero, TipoHabitacion tipo, int capacidad, decimal precioPorNoche)
        {
            if (numero <= 0)
                throw new ArgumentException("El número de la habitación debe ser mayor que 0.");
            if (capacidad < 1)
                throw new ArgumentException("La capacidad debe ser al menos 1 persona.");
            if (precioPorNoche <= 0)
                throw new ArgumentException("El precio por noche debe ser mayor que 0.");

            this.numero = numero;
            this.tipo = tipo;
            this.capacidad = capacidad;
            this.precioPorNoche = precioPorNoche;
            this.ocupada = false; // al crearla siempre está libre
        }

        // Propiedades de solo lectura
        public int Numero { get { return numero; } }
        public TipoHabitacion Tipo { get { return tipo; } }
        public int Capacidad { get { return capacidad; } }
        public decimal PrecioPorNoche { get { return precioPorNoche; } }

        // Cualquiera puede consultar si está ocupada...
        public bool Ocupada { get { return ocupada; } }

        // ...pero solo el código del mismo proyecto (el recepcionista) puede ocuparla
        internal void Ocupar()
        {
            ocupada = true;
        }

        // IFacturable: para la habitación el total es el valor de una noche
        public decimal TotalAPagar()
        {
            return precioPorNoche;
        }

        public string Factura()
        {
            return $"Habitación {numero} ({tipo}) - 1 noche: ${TotalAPagar().ToString("N0", CultureInfo.InvariantCulture)}";
        }

        public override string ToString()
        {
            string estado = ocupada ? "Ocupada" : "Libre";
            return $"Habitación {numero} | {tipo} | Capacidad: {capacidad} | " +
                   $"${precioPorNoche.ToString("N0", CultureInfo.InvariantCulture)} por noche | {estado}";
        }
    }
}
