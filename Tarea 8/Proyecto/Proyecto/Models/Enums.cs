namespace Proyecto.Models
{
    public enum TipoVehiculo
    {
        Automovil = 1,
        SUV = 2,
        Minivan = 3,
        Minibus = 4,
        Autobus = 5,
        Camion = 6
    }

    public enum UnidadCombustible
    {
        Galon = 1,
        Litro = 2
    }

    public enum TipoCosto
    {
        Fijo = 1,        // No depende de la distancia (periodo: mensual/semanal/diaria)
        Variable = 2     // Depende de la distancia (por kilometraje)
    }

    public enum PeriodicidadCosto
    {
        Mensual = 1,
        Semanal = 2,
        Diario = 3,
        PorKilometro = 4
    }

    public enum CategoriaCosto
    {
        Mantenimiento = 1,
        Seguro = 2,
        Peajes = 3,
        Otros = 4
    }

    public enum TipoServicio
    {
        Pasajeros = 1,
        Escolar = 2,
        Turistico = 3,
        Empresarial = 4,
        Mercancias = 5,
        Privado = 6,
        PorHora = 7,
        PorKilometro = 8
    }
}