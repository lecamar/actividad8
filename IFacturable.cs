namespace Hotel
{
    // Interfaz que define el comportamiento de cualquier cosa que se pueda cobrar.
    // La implementan Huesped y Habitacion, que no tienen relación de herencia entre sí.
    public interface IFacturable
    {
        // Devuelve el valor total que hay que pagar.
        decimal TotalAPagar();

        // Devuelve un texto con el detalle de la factura.
        string Factura();
    }
}
